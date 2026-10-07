using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Database;
using SimpleCollectionHub.Framework.Helpers;
using SimpleCollectionHub.Framework.Models;
using SimpleCollectionHub.Framework.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleCollectionHub.ViewModels;

/// <summary>
/// 把剪贴板图片写入本机 Media，再挂到当前条目的图片字段。Ctrl+V 和「粘贴」按钮共用。
/// </summary>
internal static class CollectionImagePaste
{
	/// <summary>
	/// 输入框里贴文字时不要抢走 Ctrl+V。截图通常没有文本，即使焦点在备注框也照样入库。
	/// </summary>
	public static bool ShouldLeaveTextPaste()
	{
		if (Keyboard.FocusedElement is not (TextBox or PasswordBox))
		{
			return false;
		}

		try
		{
			if (ClipboardImageHelper.HasImage() && !Clipboard.ContainsText())
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			return true;
		}

		return true;
	}

	/// <summary>
	/// 粘贴到指定图片字段；<paramref name="targetField"/> 为空时用第一条图片字段，没有就新建。
	/// 剪贴板没图时返回 false，调用方不要把按键标成已处理。
	/// </summary>
	public static bool TryPaste(
		ICollectionWorkspace workspace,
		IMediaStore mediaStore,
		FieldEditorModel targetField,
		out string message
	)
	{
		message = string.Empty;
		IReadOnlyList<StoredImage> imported = mediaStore.ImportFromClipboard();
		if (imported.IsEmpty())
		{
			return false;
		}

		if (!workspace.HasDetail)
		{
			message = "先选择一条";
			return true;
		}

		if (!workspace.IsEditing)
		{
			workspace.IsEditing = true;
		}

		FieldEditorModel field = targetField;
		if (field is null || !field.IsImages)
		{
			field = FindFirstImageField(workspace);
		}

		if (field is null)
		{
			field = new FieldEditorModel
			{
				Type = FieldTypes.Images,
				Label = "图片",
			};
			workspace.DetailFields.Add(field);
		}

		foreach (StoredImage stored in imported)
		{
			field.Images.Add(new ImageEditorModel
			{
				Id = stored.Id,
				RelativePath = stored.RelativePath,
				Caption = stored.Caption,
				Preview = mediaStore.LoadPreview(stored.RelativePath),
			});
		}

		message = "已粘贴";
		return true;
	}

	private static FieldEditorModel FindFirstImageField(ICollectionWorkspace workspace)
	{
		foreach (FieldEditorModel field in workspace.DetailFields)
		{
			if (field.IsImages)
			{
				return field;
			}
		}

		return null;
	}
}
