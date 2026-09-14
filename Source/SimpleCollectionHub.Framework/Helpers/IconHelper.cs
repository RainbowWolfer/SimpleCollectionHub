using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleCollectionHub.Framework.Helpers;

/// <summary>
/// 图标辅助方法。图标资源由 Resources 项目的 T4 生成，
/// 在 C# 中通过 <c>IconResources.Xxx_PNG</c> 拿到 pack URI，再用这里的方法转成控件。
/// </summary>
public static class IconHelper
{
	/// <summary> 把 pack URI 字符串转成 <see cref="ImageSource"/> </summary>
	public static ImageSource ToImageSource(this string packUri)
	{
		return new BitmapImage(new Uri(packUri, UriKind.Absolute));
	}

	/// <summary> 把 pack URI 或已有的 ImageSource 统一转成 <see cref="ImageSource"/> </summary>
	public static ImageSource ToIcon(this object obj)
	{
		return obj switch
		{
			null => null,
			string packUri => packUri.ToImageSource(),
			ImageSource imageSource => imageSource,
			_ => null,
		};
	}

	/// <summary> 生成右键菜单用的 16x16 图标控件 </summary>
	public static Image ToContextMenuIcon(this ImageSource imageSource)
	{
		return new Image()
		{
			Source = imageSource,
			Height = 16,
			Width = 16,
		};
	}

	/// <summary> 用 <see cref="ImageSource"/> 生成 <see cref="Image"/> 控件 </summary>
	public static Image ToImage(this ImageSource imageSource)
	{
		return new Image()
		{
			Source = imageSource,
		};
	}
}
