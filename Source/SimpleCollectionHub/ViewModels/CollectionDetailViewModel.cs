using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using RW.Common.Helpers;
using RW.Common.WPF.Helpers;
using SimpleCollectionHub.Framework.Database;
using SimpleCollectionHub.Framework.Models;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimpleCollectionHub.ViewModels;

/// <summary>
/// 右侧详情。编辑时只改 LocalAppData 库；路径只读；打开原目录只唤起资源管理器。
/// </summary>
internal class CollectionDetailViewModel(
	ICollectionWorkspace workspace,
	ILibraryService libraryService,
	IMediaStore mediaStore
) : ViewModelBaseEx
{
	public ICollectionWorkspace Workspace => workspace;

	public IOpenFileDialogService OpenImageDialog => GetService<IOpenFileDialogService>(nameof(OpenImageDialog));

	public IDelegateCommand CloseDetailCommand => field ??= new DelegateCommand(CloseDetail);
	private void CloseDetail()
	{
		workspace.IsEditing = false;
		workspace.SelectedItem = null;
	}

	public IDelegateCommand BeginEditCommand => field ??= new DelegateCommand(BeginEdit);
	private void BeginEdit()
	{
		if (!workspace.HasDetail)
		{
			return;
		}

		workspace.IsEditing = true;
	}

	public IDelegateCommand CancelEditCommand => field ??= new AsyncCommand(CancelEdit);
	private async Task CancelEdit()
	{
		workspace.IsEditing = false;
		await workspace.ReloadAsync();
	}

	public IDelegateCommand SaveCommand => field ??= new AsyncCommand(Save);
	private async Task Save()
	{
		ItemCardModel card = workspace.SelectedItem;
		if (card is null)
		{
			return;
		}

		if (workspace.DetailName.IsBlank())
		{
			MessageBoxService.ShowError("名称不能为空");
			return;
		}

		try
		{
			CollectionItem item = workspace.FindItem(card.Id);
			if (item is null)
			{
				return;
			}

			item.Name = workspace.DetailName.Trim();
			item.GroupId = workspace.DetailGroupId;
			item.Tags = [.. workspace.DetailTags];
			await libraryService.UpdateItemAsync(item);

			List<CollectionField> fields = [];
			foreach (FieldEditorModel editor in workspace.DetailFields)
			{
				CollectionField field = new()
				{
					Id = editor.Id == Guid.Empty ? Guid.NewGuid() : editor.Id,
					Type = editor.Type,
					Label = editor.Label.NotBlankCheck() ?? editor.TypeLabel,
					TextValue = editor.TextValue ?? string.Empty,
					Images = [],
					Links = [],
				};
				foreach (ImageEditorModel image in editor.Images)
				{
					field.Images.Add(new StoredImage
					{
						Id = image.Id,
						RelativePath = image.RelativePath,
						Caption = image.Caption,
					});
				}

				foreach (LinkEditorModel link in editor.Links)
				{
					field.Links.Add(new StoredLink
					{
						Title = link.Title ?? string.Empty,
						Url = link.Url ?? string.Empty,
					});
				}

				fields.Add(field);
			}

			await libraryService.SaveFieldsAsync(item.Id, fields);
			workspace.IsEditing = false;
			await workspace.ReloadAsync();
			workspace.ShowToast("已保存");
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			MessageBoxService.ShowError(ex.Message);
		}
	}

	public IDelegateCommand OpenOriginalCommand => field ??= new DelegateCommand(OpenOriginal);
	private void OpenOriginal()
	{
		string path = workspace.DetailPath;
		if (path.IsBlank())
		{
			return;
		}

		if (!path.ShowInExplorerWithMessage())
		{
			workspace.ShowToast("路径不存在");
		}
	}

	public IDelegateCommand RemoveFromLibraryCommand => field ??= new AsyncCommand(RemoveFromLibrary);
	private async Task RemoveFromLibrary()
	{
		ItemCardModel card = workspace.SelectedItem;
		if (card is null)
		{
			return;
		}

		if (!MessageBoxService.ShowOkCancelQuestion($"从库中移除「{card.Name}」？"))
		{
			return;
		}

		try
		{
			await libraryService.DeleteItemAsync(card.Id, deleteImportedImages: true);
			workspace.SelectedItem = null;
			workspace.IsEditing = false;
			await workspace.ReloadAsync();
			workspace.ShowToast("已从库中移除");
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			MessageBoxService.ShowError(ex.Message);
		}
	}

	public IDelegateCommand AddTagCommand => field ??= new DelegateCommand(AddTag);
	private void AddTag()
	{
		string tag = workspace.NewTagText?.Trim();
		if (tag.IsBlank())
		{
			return;
		}

		foreach (string existing in workspace.DetailTags)
		{
			if (string.Equals(existing, tag, StringComparison.CurrentCultureIgnoreCase))
			{
				workspace.NewTagText = string.Empty;
				return;
			}
		}

		workspace.DetailTags.Add(tag);
		workspace.NewTagText = string.Empty;
	}

	public IDelegateCommand RemoveTagCommand => field ??= new DelegateCommand<string>(RemoveTag);
	private void RemoveTag(string tag)
	{
		if (tag.IsNotBlank())
		{
			workspace.DetailTags.Remove(tag);
		}
	}

	public IDelegateCommand AddTextFieldCommand => field ??= new DelegateCommand(() => AddField(FieldTypes.Text, "新文字"));

	public IDelegateCommand AddImageFieldCommand => field ??= new DelegateCommand(() => AddField(FieldTypes.Images, "新图片"));

	public IDelegateCommand AddLinkFieldCommand => field ??= new DelegateCommand(() => AddField(FieldTypes.Links, "新链接"));

	private void AddField(string type, string label)
	{
		if (!workspace.IsEditing)
		{
			return;
		}

		workspace.DetailFields.Add(new FieldEditorModel
		{
			Type = type,
			Label = label,
		});
	}

	public IDelegateCommand RemoveFieldCommand => field ??= new DelegateCommand<FieldEditorModel>(RemoveField);
	private void RemoveField(FieldEditorModel field)
	{
		if (field is null || !workspace.IsEditing)
		{
			return;
		}

		workspace.DetailFields.Remove(field);
	}

	public IDelegateCommand ImportImagesCommand => field ??= new DelegateCommand<FieldEditorModel>(ImportImages);
	private void ImportImages(FieldEditorModel field)
	{
		if (field is null || !workspace.IsEditing)
		{
			return;
		}

		if (!OpenImageDialog.ShowDialog())
		{
			return;
		}

		List<IFileInfo> files = [];
		if (OpenImageDialog.Files is not null)
		{
			foreach (IFileInfo file in OpenImageDialog.Files)
			{
				files.Add(file);
			}
		}

		if (files.IsEmpty() && OpenImageDialog.File is not null)
		{
			files.Add(OpenImageDialog.File);
		}

		foreach (IFileInfo file in files)
		{
			try
			{
				string source = file.GetFullName();
				StoredImage stored = mediaStore.ImportFile(source, file.Name);
				field.Images.Add(new ImageEditorModel
				{
					Id = stored.Id,
					RelativePath = stored.RelativePath,
					Caption = stored.Caption,
					Preview = mediaStore.LoadPreview(stored.RelativePath),
				});
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				MessageBoxService.ShowError($"导入失败：{ex.Message}");
			}
		}
	}

	public IDelegateCommand RemoveImageCommand => field ??= new DelegateCommand<ImageEditorModel>(RemoveImage);
	private void RemoveImage(ImageEditorModel image)
	{
		if (image is null || !workspace.IsEditing)
		{
			return;
		}

		foreach (FieldEditorModel field in workspace.DetailFields)
		{
			if (field.Images.Remove(image))
			{
				mediaStore.TryDelete(image.RelativePath);
				return;
			}
		}
	}

	public IDelegateCommand OpenLightboxCommand => field ??= new DelegateCommand<ImageEditorModel>(OpenLightbox);
	private void OpenLightbox(ImageEditorModel image)
	{
		if (image?.Preview is ImageSource preview)
		{
			workspace.OpenLightbox(preview, image.Caption);
		}
	}

	public IDelegateCommand AddLinkCommand => field ??= new DelegateCommand<FieldEditorModel>(AddLink);
	private void AddLink(FieldEditorModel field)
	{
		if (field is null || !workspace.IsEditing)
		{
			return;
		}

		string title = workspace.NewLinkTitle?.Trim();
		string url = workspace.NewLinkUrl?.Trim();
		if (title.IsBlank() && url.IsBlank())
		{
			return;
		}

		field.Links.Add(new LinkEditorModel
		{
			Title = title.NotBlankCheck() ?? url,
			Url = url ?? string.Empty,
		});
		workspace.NewLinkTitle = string.Empty;
		workspace.NewLinkUrl = string.Empty;
	}

	public IDelegateCommand RemoveLinkCommand => field ??= new DelegateCommand<LinkEditorModel>(RemoveLink);
	private void RemoveLink(LinkEditorModel link)
	{
		if (link is null || !workspace.IsEditing)
		{
			return;
		}

		foreach (FieldEditorModel field in workspace.DetailFields)
		{
			if (field.Links.Remove(link))
			{
				return;
			}
		}
	}

	public IDelegateCommand OpenLinkCommand => field ??= new DelegateCommand<string>(OpenLink);
	private void OpenLink(string url)
	{
		if (url.IsBlank())
		{
			return;
		}

		if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
			|| (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
		{
			MessageBoxService.ShowError("只能打开 http / https 链接");
			return;
		}

		try
		{
			Process.Start(new ProcessStartInfo(uri.AbsoluteUri)
			{
				UseShellExecute = true,
			});
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			MessageBoxService.ShowError(ex.Message);
		}
	}
}
