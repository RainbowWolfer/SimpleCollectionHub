using RW.Common.Helpers;
using System;
using System.Globalization;
using System.Windows.Media;

namespace SimpleCollectionHub.Framework.Helpers;

/// <summary>Fluent 码位与颜色字符串转换。界面图标只用 Segoe Fluent Icons。</summary>
public static class FluentIconHelper
{
	/// <summary>把 E7FC 这样的十六进制码位转成 FontIcon.Text。</summary>
	public static string ToGlyph(string code)
	{
		if (code.IsBlank())
		{
			return "\uE8B7";
		}

		string hex = code.Trim().TrimStart('#');
		if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int value)
			&& value >= 0
			&& value <= 0x10FFFF)
		{
			return char.ConvertFromUtf32(value);
		}

		return "\uE8B7";
	}

	/// <summary>解析 #RRGGBB，失败时返回灰色。</summary>
	public static Color ParseColor(string hex)
	{
		string text = (hex ?? string.Empty).Trim();
		if (text.StartsWith('#') && text.Length == 7
			&& byte.TryParse(text.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r)
			&& byte.TryParse(text.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g)
			&& byte.TryParse(text.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b))
		{
			return Color.FromRgb(r, g, b);
		}

		return Color.FromRgb(0x5C, 0x5C, 0x5C);
	}

	/// <summary>冻结的画刷，可直接绑到 FontIcon.Foreground。</summary>
	public static SolidColorBrush ToBrush(string hex)
	{
		SolidColorBrush brush = new(ParseColor(hex));
		brush.Freeze();
		return brush;
	}
}
