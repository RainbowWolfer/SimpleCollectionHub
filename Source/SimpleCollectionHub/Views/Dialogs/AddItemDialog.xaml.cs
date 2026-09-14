using DevExpress.Mvvm;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Database;
using SimpleCollectionHub.Framework.Helpers;
using SimpleCollectionHub.Framework.Models;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace SimpleCollectionHub.Views.Dialogs;

public partial class AddItemDialog : UserControl
{
	public AddItemDialog()
	{
		InitializeComponent();
	}
}

internal record AddItemDialogParameter(Guid? GroupId)
{
	public string ResultName { get; set; } = string.Empty;

	public string ResultPath { get; set; } = string.Empty;

	public Guid ResultGroupId { get; set; }
}

/// <summary>绑定已有文件夹。只记录路径字符串，不扫描、不写入该目录。</summary>
internal class AddItemDialogViewModel(
	ICollectionWorkspace workspace,
	IPathProbeService pathProbe
) : DialogViewModelOkCancel<AddItemDialogParameter>
{
	public ObservableCollection<GroupPickOption> Groups { get; } = [];

	public string ItemName
	{
		get => GetProperty(() => ItemName);
		set => SetProperty(() => ItemName, value);
	}

	public string ItemPath
	{
		get => GetProperty(() => ItemPath);
		set
		{
			SetProperty(() => ItemPath, value);
			RaisePropertyChanged(() => PathExists);
		}
	}

	public GroupPickOption SelectedGroup
	{
		get => GetProperty(() => SelectedGroup);
		set => SetProperty(() => SelectedGroup, value);
	}

	public bool PathExists => pathProbe.Exists(ItemPath);

	protected override void OnInitialized()
	{
		DialogTitle = "添加条目";
		ConfirmDialogCommand.Content = "确定";
		CancelDialogCommand.Content = "取消";
		foreach (GroupPickOption option in workspace.GroupPicks)
		{
			Groups.Add(option);
		}

		Guid? preferred = Parameter.GroupId;
		SelectedGroup = null;
		foreach (GroupPickOption option in Groups)
		{
			if (option.Id == preferred)
			{
				SelectedGroup = option;
				break;
			}
		}

		SelectedGroup ??= Groups.Count > 0 ? Groups[0] : null;
	}

	public IDelegateCommand PickFolderCommand => field ??= new DelegateCommand(PickFolder);
	private void PickFolder()
	{
		string initial = ItemPath.IsNotBlank() ? ItemPath : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		if (!AppFileHelper.PickFolder("选择已有文件夹", initial, out string path))
		{
			return;
		}

		ItemPath = path;
		if (ItemName.IsBlank())
		{
			ItemName = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
		}
	}

	protected override bool Validate(out string message)
	{
		if (Groups.IsEmpty())
		{
			message = "请先新建分组";
			return false;
		}

		if (ItemPath.IsBlank())
		{
			message = "请选择已经存在的文件夹";
			return false;
		}

		if (!pathProbe.Exists(ItemPath))
		{
			message = "路径不存在。本软件不会替你创建目录。";
			return false;
		}

		if (ItemName.IsBlank())
		{
			message = "请填写显示名";
			return false;
		}

		if (SelectedGroup?.Id is not Guid groupId || groupId == Guid.Empty)
		{
			message = "请选择分组";
			return false;
		}

		message = string.Empty;
		return true;
	}

	protected override bool OnConfirmed()
	{
		Parameter.ResultName = ItemName.Trim();
		Parameter.ResultPath = ItemPath.Trim();
		Parameter.ResultGroupId = SelectedGroup.Id.Value;
		return true;
	}

	protected override void ShowErrorDialog(string message)
	{
		MessageBoxService.ShowError(message);
	}
}
