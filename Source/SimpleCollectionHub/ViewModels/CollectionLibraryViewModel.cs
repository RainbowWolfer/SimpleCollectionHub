using DevExpress.Mvvm;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Models;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModels;
using SimpleCollectionHub.Framework.ViewModelServices;
using SimpleCollectionHub.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleCollectionHub.ViewModels;

/// <summary>中间库：面包屑、完整度、排序、卡片/列表。筛选结果来自 Workspace，不改树计数。</summary>
internal class CollectionLibraryViewModel(
	ICollectionWorkspace workspace,
	ILibraryService libraryService
) : ViewModelBaseEx
{
	public ICollectionWorkspace Workspace => workspace;

	public IReadOnlyList<NamedOption> IncompleteOptions { get; } =
	[
		new NamedOption { Label = "完整度：全部", Value = IncompleteFilter.All },
		new NamedOption { Label = "不完整", Value = IncompleteFilter.Any },
		new NamedOption { Label = "路径丢失", Value = IncompleteFilter.Path },
		new NamedOption { Label = "缺少图片", Value = IncompleteFilter.Images },
		new NamedOption { Label = "缺少文字", Value = IncompleteFilter.Text },
		new NamedOption { Label = "缺少链接", Value = IncompleteFilter.Links },
	];

	public IReadOnlyList<NamedOption> SortOptions { get; } =
	[
		new NamedOption { Label = "名称 A–Z", Value = ItemSortMode.NameAsc },
		new NamedOption { Label = "名称 Z–A", Value = ItemSortMode.NameDesc },
		new NamedOption { Label = "最近加入", Value = ItemSortMode.AddedDesc },
		new NamedOption { Label = "最早加入", Value = ItemSortMode.AddedAsc },
	];

	public IDialogServiceEx AddGroupDialog => GetService<IDialogServiceEx>(nameof(AddGroupDialog));

	public IDialogServiceEx AddItemDialog => GetService<IDialogServiceEx>(nameof(AddItemDialog));

	public IDelegateCommand SelectItemCommand => field ??= new DelegateCommand<ItemCardModel>(SelectItem);
	private void SelectItem(ItemCardModel item)
	{
		workspace.SelectedItem = item;
	}

	public IDelegateCommand SelectCrumbCommand => field ??= new DelegateCommand<CrumbModel>(SelectCrumb);
	private void SelectCrumb(CrumbModel crumb)
	{
		if (crumb is null || crumb.IsCurrent)
		{
			return;
		}

		workspace.SelectedGroupId = crumb.GroupId ?? Guid.Empty;
	}

	public IDelegateCommand SetCardsCommand => field ??= new DelegateCommand(() => workspace.ViewMode = LibraryViewMode.Cards);

	public IDelegateCommand SetListCommand => field ??= new DelegateCommand(() => workspace.ViewMode = LibraryViewMode.List);

	public IDelegateCommand OpenAddGroupCommand => field ??= new AsyncCommand(OpenAddGroup);
	private async Task OpenAddGroup()
	{
		Guid? parent = workspace.SelectedGroupId == Guid.Empty ? null : workspace.SelectedGroupId;
		AddGroupDialogParameter parameter = new(parent);
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

	public IDelegateCommand OpenAddItemCommand => field ??= new AsyncCommand(OpenAddItem);
	private async Task OpenAddItem()
	{
		Guid? groupId = workspace.SelectedGroupId == Guid.Empty ? null : workspace.SelectedGroupId;
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
