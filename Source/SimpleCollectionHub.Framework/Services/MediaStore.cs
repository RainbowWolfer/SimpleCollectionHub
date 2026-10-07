using RW.Base.WPF.DependencyInjections;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Configs;
using SimpleCollectionHub.Framework.Database;
using SimpleCollectionHub.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleCollectionHub.Framework.Services;

/// <summary>把用户选择的图片复制进软件 Media 目录，原目录保持只读。</summary>
public interface IMediaStore
{
	/// <summary>复制一张图到 MediaFolder，返回库内相对路径。</summary>
	StoredImage ImportFile(string sourcePath, string caption);

	/// <summary>把位图存成 PNG 到 MediaFolder。截图粘贴走这里。</summary>
	StoredImage ImportBitmap(BitmapSource source, string caption);

	/// <summary>从剪贴板导入图片（PNG / 位图 / 复制的图片文件）。没有图片时返回空列表。</summary>
	IReadOnlyList<StoredImage> ImportFromClipboard();

	/// <summary>把相对路径拼成绝对路径。不存在时返回 null。</summary>
	string? GetAbsolutePath(string relativePath);

	/// <summary>读取库内副本为可绑定图像。源文件只读。</summary>
	ImageSource LoadPreview(string relativePath);

	/// <summary>删除媒体文件本身。调用方负责先改库记录。</summary>
	void TryDelete(string relativePath);
}

/// <summary>
/// 媒体只落在 LocalAppData。导入时只读取源文件再复制，不改源文件。
/// </summary>
internal class MediaStore(AppFolderConfig folderConfig) : IMediaStore, ISingletonDependency
{
	public StoredImage ImportFile(string sourcePath, string caption)
	{
		Directory.CreateDirectory(folderConfig.MediaFolder);
		string ext = Path.GetExtension(sourcePath);
		if (ext.IsBlank())
		{
			ext = ".img";
		}

		string relative = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
		string dest = Path.Combine(folderConfig.MediaFolder, relative);
		File.Copy(sourcePath, dest, overwrite: false);
		return new StoredImage
		{
			Id = Guid.NewGuid(),
			RelativePath = relative,
			Caption = caption.NotBlankCheck() ?? Path.GetFileName(sourcePath),
		};
	}

	public StoredImage ImportBitmap(BitmapSource source, string caption)
	{
		if (source is null)
		{
			throw new ArgumentNullException(nameof(source));
		}

		Directory.CreateDirectory(folderConfig.MediaFolder);
		string relative = $"{Guid.NewGuid():N}.png";
		string dest = Path.Combine(folderConfig.MediaFolder, relative);
		BitmapSource pixels = source;
		if (source.Format != PixelFormats.Bgra32 && source.Format != PixelFormats.Pbgra32)
		{
			pixels = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
		}

		if (pixels.CanFreeze && !pixels.IsFrozen)
		{
			pixels.Freeze();
		}

		PngBitmapEncoder encoder = new();
		encoder.Frames.Add(BitmapFrame.Create(pixels));
		using FileStream file = File.Create(dest);
		encoder.Save(file);
		return new StoredImage
		{
			Id = Guid.NewGuid(),
			RelativePath = relative,
			Caption = caption.NotBlankCheck() ?? "截图",
		};
	}

	public IReadOnlyList<StoredImage> ImportFromClipboard()
	{
		List<StoredImage> imported = [];
		if (!ClipboardImageHelper.TryRead(out List<ClipboardImagePayload> payloads))
		{
			return imported;
		}

		foreach (ClipboardImagePayload payload in payloads)
		{
			try
			{
				if (payload.FilePath.IsNotBlank())
				{
					imported.Add(ImportFile(payload.FilePath, payload.Caption));
					continue;
				}

				if (payload.Bitmap is not null)
				{
					imported.Add(ImportBitmap(payload.Bitmap, payload.Caption));
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		return imported;
	}

	public string? GetAbsolutePath(string relativePath)
	{
		if (relativePath.IsBlank())
		{
			return null;
		}

		string full = Path.GetFullPath(Path.Combine(folderConfig.MediaFolder, relativePath));
		string root = Path.GetFullPath(folderConfig.MediaFolder);
		if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		return File.Exists(full) ? full : null;
	}

	public ImageSource LoadPreview(string relativePath)
	{
		string path = GetAbsolutePath(relativePath);
		if (path.IsBlank())
		{
			return null;
		}

		try
		{
			BitmapImage image = new();
			image.BeginInit();
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.UriSource = new Uri(path);
			image.EndInit();
			image.Freeze();
			return image;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			return null;
		}
	}

	public void TryDelete(string relativePath)
	{
		string? full = GetAbsolutePath(relativePath);
		if (full is null)
		{
			return;
		}

		try
		{
			File.Delete(full);
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}
	}
}
