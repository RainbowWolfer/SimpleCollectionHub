using RW.Base.WPF.ViewModelServices;
using SimpleCollectionHub.Framework.Database;
using SimpleCollectionHub.Framework.Models;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModelServices;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SimpleCollectionHub.ViewModels;

/// <summary>
/// 分组 / 条目写入本机库的共用入口。侧栏和中间工具栏都走这里，避免两套 try/catch。
/// </summary>
internal static class CollectionMutations
{
	/// <summary>新建分组并选中。图标默认文件夹码位，颜色由调用方传入（通常继承父级）。</summary>
	public static async Task<CollectionGroup> TryAddGroupAsync(
		ILibraryService library,
		ICollectionWorkspace workspace,
		IMessageBoxServiceEx messages,
		string name,
		Guid? parentId,
		string kind,
		string iconColor
	)
	{
		try
		{
			CollectionGroup created = await library.AddGroupAsync(
				name,
				parentId,
				kind,
				"E8B7",
				iconColor
			);
			workspace.SelectedGroupId = created.Id;
			await workspace.ReloadAsync();
			workspace.ShowToast($"已创建分组「{created.Name}」");
			return created;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			messages.ShowError(ex.Message);
			return null;
		}
	}

	/// <summary>绑定已有路径。只插 SQLite，不改 Path 指向的磁盘。</summary>
	public static async Task<CollectionItem> TryAddItemAsync(
		ILibraryService library,
		ICollectionWorkspace workspace,
		IMessageBoxServiceEx messages,
		string name,
		string path,
		Guid groupId
	)
	{
		try
		{
			CollectionItem created = await library.AddItemAsync(name, path, groupId);
			workspace.SelectedGroupId = created.GroupId;
			await workspace.ReloadAsync();
			workspace.SelectedItem = FindVisible(workspace, created.Id);
			workspace.IsEditing = true;
			workspace.ShowToast("已写入本机库，原目录没有改动");
			return created;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			messages.ShowError(ex.Message);
			return null;
		}
	}

	private static ItemCardModel FindVisible(ICollectionWorkspace workspace, Guid id)
	{
		foreach (ItemCardModel item in workspace.FlatItems)
		{
			if (item.Id == id)
			{
				return item;
			}
		}

		foreach (LibrarySectionModel section in workspace.Sections)
		{
			foreach (ItemCardModel item in section.Items)
			{
				if (item.Id == id)
				{
					return item;
				}
			}
		}

		return null;
	}
}
