using DevExpress.Mvvm;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Database;
using SimpleCollectionHub.Framework.Models;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModels;
using SimpleCollectionHub.Framework.ViewModelServices;
using SimpleCollectionHub.Views.Dialogs;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SimpleCollectionHub.ViewModels;

/// <summary>左侧筛选 + 树。搜索和标签必须待在同一块，不要拆到树的上下两端。</summary>
internal class CollectionSidebarViewModel(
	ICollectionWorkspace workspace,
	ILibraryService libraryService,
	IAppSettingsService settingsService
) : ViewModelBaseEx
{
	public ICollectionWorkspace Workspace => workspace;

	public IDialogServiceEx AddGroupDialog => GetService<IDialogServiceEx>(nameof(AddGroupDialog));

	public IDialogServiceEx AddItemDialog => GetService<IDialogServiceEx>(nameof(AddItemDialog));

	public IDialogServiceEx GroupIconDialog => GetService<IDialogServiceEx>(nameof(GroupIconDialog));

	public IDialogServiceEx AboutDialog => GetService<IDialogServiceEx>(nameof(AboutDialog));

	public IDelegateCommand SelectNodeCommand => field ??= new DelegateCommand<object>(SelectNode);
	private void SelectNode(object parameter)
	{
		if (parameter is GroupNode node)
		{
			workspace.SelectedGroupId = node.Id;
		}
	}

	public IDelegateCommand ClearFiltersCommand => field ??= new DelegateCommand(workspace.ClearSidebarFilters);

	public IDelegateCommand ToggleTagCommand => field ??= new DelegateCommand<TagChipModel>(ToggleTag);
	private void ToggleTag(TagChipModel chip)
	{
		if (chip is null)
		{
			return;
		}

		workspace.ToggleTag(chip.Tag);
	}

	public IDelegateCommand EditIconCommand => field ??= new AsyncCommand<GroupNode>(EditIcon);
	private async Task EditIcon(GroupNode node)
	{
		if (node is null)
		{
			return;
		}

		GroupIconDialogParameter parameter = new(node.Id, node.Name, node.Icon, node.IconColor);
		if (!GroupIconDialog.ShowOKCancel(this, parameter))
		{
			return;
		}

		try
		{
			if (node.IsAll)
			{
				settingsService.Model.AllIcon = parameter.ResultIcon;
				settingsService.Model.AllIconColor = parameter.ResultIconColor;
				settingsService.SaveSettings();
			}
			else
			{
				CollectionGroup group = workspace.FindGroup(node.Id);
				if (group is null)
				{
					return;
				}

				group.Icon = parameter.ResultIcon;
				group.IconColor = parameter.ResultIconColor;
				await libraryService.UpdateGroupAsync(group);
			}

			await workspace.ReloadAsync();
			workspace.ShowToast("已更新图标");
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			MessageBoxService.ShowError(ex.Message);
		}
	}

	public IDelegateCommand AddChildGroupCommand => field ??= new AsyncCommand<GroupNode>(AddChildGroup);
	private async Task AddChildGroup(GroupNode node)
	{
		Guid? parent = node is null || node.IsAll ? null : node.Id;
		await ShowAddGroup(parent);
	}

	public IDelegateCommand AddItemHereCommand => field ??= new AsyncCommand<GroupNode>(AddItemHere);
	private async Task AddItemHere(GroupNode node)
	{
		Guid? groupId = node is null || node.IsAll ? null : node.Id;
		await ShowAddItem(groupId);
	}

	public IDelegateCommand OpenAddGroupCommand => field ??= new AsyncCommand(
		() => ShowAddGroup(workspace.SelectedGroupId == Guid.Empty ? null : workspace.SelectedGroupId)
	);

	public IDelegateCommand OpenAboutCommand => field ??= new DelegateCommand(() => AboutDialog.ShowDialog(this));

	public IDelegateCommand DeleteGroupCommand => field ??= new AsyncCommand<GroupNode>(DeleteGroup);
	private async Task DeleteGroup(GroupNode node)
	{
		if (node is null || node.IsAll)
		{
			return;
		}

		try
		{
			if (node.Children.IsNotEmpty())
			{
				workspace.ShowToast("先删掉或移走子分组");
				return;
			}

			CollectionGroup group = workspace.FindGroup(node.Id);
			if (group is null)
			{
				return;
			}

			try
			{
				await libraryService.DeleteGroupAsync(node.Id, moveItemsToParent: false);
			}
			catch (InvalidOperationException ex)
			{
				Debug.WriteLine(ex);
				if (group.ParentId is null)
				{
					workspace.ShowToast("顶级分组里还有条目，先移走再删");
					return;
				}

				if (!MessageBoxService.ShowOkCancelQuestion($"「{group.Name}」里还有条目。\n删除分组后，它们会移到上一层。继续？"))
				{
					return;
				}

				await libraryService.DeleteGroupAsync(node.Id, moveItemsToParent: true);
			}

			if (workspace.SelectedGroupId == node.Id)
			{
				workspace.SelectedGroupId = group.ParentId ?? Guid.Empty;
			}

			await workspace.ReloadAsync();
			workspace.ShowToast("分组已删除");
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			MessageBoxService.ShowError(ex.Message);
		}
	}

	internal async Task ShowAddGroup(Guid? parentId)
	{
		AddGroupDialogParameter parameter = new(parentId);
		if (!AddGroupDialog.ShowOKCancel(this, parameter))
		{
			return;
		}

		await CollectionMutations.TryAddGroupAsync(
			libraryService,
			workspace,
			MessageBoxService,
			parameter.ResultName,
			parameter.ResultParentId,
			parameter.ResultKind,
			parameter.ResultIconColor
		);
	}

	internal async Task ShowAddItem(Guid? groupId)
	{
		AddItemDialogParameter parameter = new(groupId);
		if (!AddItemDialog.ShowOKCancel(this, parameter))
		{
			return;
		}

		await CollectionMutations.TryAddItemAsync(
			libraryService,
			workspace,
			MessageBoxService,
			parameter.ResultName,
			parameter.ResultPath,
			parameter.ResultGroupId
		);
	}
}
