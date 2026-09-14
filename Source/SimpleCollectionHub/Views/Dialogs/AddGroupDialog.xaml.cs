using DevExpress.Mvvm;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Database;
using SimpleCollectionHub.Framework.Models;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace SimpleCollectionHub.Views.Dialogs;

public partial class AddGroupDialog : UserControl
{
	public AddGroupDialog()
	{
		InitializeComponent();
	}
}

internal record AddGroupDialogParameter(Guid? ParentId)
{
	public string ResultName { get; set; } = string.Empty;

	public Guid? ResultParentId { get; set; }

	public string ResultKind { get; set; } = GroupKinds.Games;

	public string ResultIconColor { get; set; } = "#5c5c5c";
}

/// <summary>新建分组。顶级默认 games；子分组继承祖先 Kind 和颜色。</summary>
internal class AddGroupDialogViewModel(ICollectionWorkspace workspace) : DialogViewModelOkCancel<AddGroupDialogParameter>
{
	public ObservableCollection<GroupPickOption> Parents { get; } = [];

	public string GroupName
	{
		get => GetProperty(() => GroupName);
		set => SetProperty(() => GroupName, value);
	}

	public GroupPickOption SelectedParent
	{
		get => GetProperty(() => SelectedParent);
		set => SetProperty(() => SelectedParent, value);
	}

	protected override void OnInitialized()
	{
		DialogTitle = "新建分组";
		ConfirmDialogCommand.Content = "确定";
		CancelDialogCommand.Content = "取消";
		Parents.Add(new GroupPickOption
		{
			Id = null,
			Label = "全部收藏（顶级）",
		});
		foreach (GroupPickOption option in workspace.GroupPicks)
		{
			Parents.Add(option);
		}

		SelectedParent = Parents[0];
		Guid? preferred = Parameter.ParentId;
		if (preferred is Guid id)
		{
			foreach (GroupPickOption option in Parents)
			{
				if (option.Id == id)
				{
					SelectedParent = option;
					break;
				}
			}
		}
	}

	protected override bool Validate(out string message)
	{
		if (GroupName.IsBlank())
		{
			message = "请填写分组名称";
			return false;
		}

		message = string.Empty;
		return true;
	}

	protected override bool OnConfirmed()
	{
		Guid? parentId = SelectedParent?.Id;
		Parameter.ResultName = GroupName.Trim();
		Parameter.ResultParentId = parentId;
		if (parentId is Guid id)
		{
			CollectionGroup parent = workspace.FindGroup(id);
			Parameter.ResultKind = workspace.ResolveKind(parent);
			Parameter.ResultIconColor = parent?.IconColor.NotBlankCheck() ?? "#5c5c5c";
		}
		else
		{
			Parameter.ResultKind = GroupKinds.Games;
			Parameter.ResultIconColor = "#5c5c5c";
		}

		return true;
	}

	protected override void ShowErrorDialog(string message)
	{
		MessageBoxService.ShowError(message);
	}
}
