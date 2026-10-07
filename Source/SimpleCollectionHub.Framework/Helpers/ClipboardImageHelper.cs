using RW.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleCollectionHub.Framework.Helpers;

/// <summary>剪贴板里待入库的一张图：本地文件路径，或已经解码的位图。</summary>
public sealed class ClipboardImagePayload
{
	/// <summary>资源管理器复制的文件路径。有值时优先按文件导入。</summary>
	public string FilePath { get; init; }

	/// <summary>截图解码后的位图。和 FilePath 二选一。</summary>
	public BitmapSource Bitmap { get; init; }

	/// <summary>写入库时的说明，截图默认「截图」。</summary>
	public string Caption { get; init; } = "截图";
}

/// <summary>
/// 读取剪贴板图片。Win+Shift+S 通常只给 PNG，WPF 的 ContainsImage 经常为 false，所以优先读 PNG。
/// </summary>
public static class ClipboardImageHelper
{
	private static readonly string[] PngFormats = ["PNG", "image/png"];
	private static readonly string[] ImageExtensions = [".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp"];

	/// <summary>剪贴板是否带图（截图 PNG、CF_BITMAP，或资源管理器复制的图片文件）。</summary>
	public static bool HasImage()
	{
		try
		{
			IDataObject data = Clipboard.GetDataObject();
			if (data is not null)
			{
				foreach (string format in PngFormats)
				{
					if (data.GetDataPresent(format))
					{
						return true;
					}
				}
			}

			if (Clipboard.ContainsImage())
			{
				return true;
			}

			if (Clipboard.ContainsFileDropList())
			{
				foreach (string path in Clipboard.GetFileDropList())
				{
					if (IsImagePath(path))
					{
						return true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}

		return false;
	}

	/// <summary>取出剪贴板图片。同一张截图只收一份，优先 PNG，其次文件列表，最后 DIB。</summary>
	public static bool TryRead(out List<ClipboardImagePayload> payloads)
	{
		payloads = [];
		try
		{
			if (TryReadFileDrops(payloads))
			{
				return true;
			}

			BitmapSource bitmap = TryReadBitmap();
			if (bitmap is not null)
			{
				payloads.Add(new ClipboardImagePayload
				{
					Bitmap = bitmap,
					Caption = "截图",
				});
				return true;
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}

		return false;
	}

	private static bool TryReadFileDrops(List<ClipboardImagePayload> payloads)
	{
		if (!Clipboard.ContainsFileDropList())
		{
			return false;
		}

		foreach (string path in Clipboard.GetFileDropList())
		{
			if (!IsImagePath(path))
			{
				continue;
			}

			payloads.Add(new ClipboardImagePayload
			{
				FilePath = path,
				Caption = Path.GetFileName(path),
			});
		}

		return payloads.Count > 0;
	}

	private static BitmapSource TryReadBitmap()
	{
		IDataObject data = Clipboard.GetDataObject();
		if (data is not null)
		{
			foreach (string format in PngFormats)
			{
				if (!data.GetDataPresent(format))
				{
					continue;
				}

				BitmapSource png = DecodePng(data.GetData(format));
				if (png is not null)
				{
					return png;
				}
			}
		}

		if (!Clipboard.ContainsImage())
		{
			return null;
		}

		BitmapSource raw = Clipboard.GetImage();
		return raw is null ? null : Normalize(raw);
	}

	private static BitmapSource DecodePng(object raw)
	{
		byte[] bytes = raw switch
		{
			MemoryStream memory => memory.ToArray(),
			byte[] array => array,
			Stream stream => ReadStream(stream),
			_ => null,
		};
		if (bytes is null || bytes.Length == 0)
		{
			return null;
		}

		BitmapImage image = new();
		image.BeginInit();
		image.CacheOption = BitmapCacheOption.OnLoad;
		image.StreamSource = new MemoryStream(bytes);
		image.EndInit();
		image.Freeze();
		return image;
	}

	private static byte[] ReadStream(Stream stream)
	{
		using MemoryStream copy = new();
		stream.CopyTo(copy);
		return copy.ToArray();
	}

	private static BitmapSource Normalize(BitmapSource source)
	{
		if (source.Format == PixelFormats.Bgra32 || source.Format == PixelFormats.Pbgra32)
		{
			if (source.CanFreeze && !source.IsFrozen)
			{
				source.Freeze();
			}

			return source;
		}

		FormatConvertedBitmap converted = new(source, PixelFormats.Bgra32, null, 0);
		converted.Freeze();
		return converted;
	}

	private static bool IsImagePath(string path)
	{
		if (path.IsBlank() || !File.Exists(path))
		{
			return false;
		}

		string ext = Path.GetExtension(path);
		foreach (string allowed in ImageExtensions)
		{
			if (string.Equals(ext, allowed, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}
}
