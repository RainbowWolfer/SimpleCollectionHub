using HandyControl.Themes;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SimpleCollectionHub.Framework.Services;

/// <summary>
/// 切换 HandyControl 的浅色/深色，并换上本项目自己的补充色、刷新窗口标题栏。
/// 启动时就能调用，不依赖依赖注入。
/// </summary>
public static class AppTheme
{
	// 补充色字典的包路径。替换时靠这段前缀认出旧字典，避免把 HandyControl 的颜色字典换掉。
	private const string ThemePrefix = "/SimpleCollectionHub.Framework;component/Resources/Themes/";

	/// <summary>最近一次套用的是不是深色。</summary>
	public static bool IsDark { get; private set; }

	/// <summary>套用浅色或深色：库主题、补充色、已打开窗口的标题栏一起改。</summary>
	public static void Apply(bool isDark)
	{
		IsDark = isDark;
		// HandyControl 自己会换 PrimaryBrush / RegionBrush 这一套
		ThemeManager.Current.ApplicationTheme = isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;
		SwapExtraColors(isDark);
		RefreshTitleBars(isDark);
	}

	/// <summary>按当前主题给一个窗口的标题栏上色。窗口句柄刚出来时调用。</summary>
	public static void ApplyTitleBar(Window window)
	{
		ApplyTitleBar(window, IsDark);
	}

	/// <summary>
	/// 换掉 Themes/Light.xaml 或 Dark.xaml。
	/// 状态标签的淡底色不在 HandyControl 里，所以单独放两本字典，跟库的主题一起换。
	/// </summary>
	private static void SwapExtraColors(bool isDark)
	{
		// 资源还没挂上时（例如设计器）直接跳过，启动流程里会再套一次
		if (Application.Current is null)
		{
			return;
		}

		string themeName = isDark ? "Dark.xaml" : "Light.xaml";
		Uri themeUri = new($"{ThemePrefix}{themeName}", UriKind.RelativeOrAbsolute);
		ResourceDictionary next = new()
		{
			Source = themeUri,
		};

		Collection<ResourceDictionary> merged = Application.Current.Resources.MergedDictionaries;
		for (int i = 0; i < merged.Count; i++)
		{
			ResourceDictionary dict = merged[i];
			if (dict.Source is null)
			{
				continue;
			}

			string source = dict.Source.OriginalString;
			if (!source.Contains(ThemePrefix))
			{
				continue;
			}

			if (source.Contains("Light.xaml") || source.Contains("Dark.xaml"))
			{
				// 原地替换，避免先删后加造成一帧闪烁
				merged[i] = next;
				return;
			}
		}

		merged.Add(next);
	}

	/// <summary>已经打开的窗口不会重新走 OnSourceInitialized，所以切换时要逐个刷标题栏。</summary>
	private static void RefreshTitleBars(bool isDark)
	{
		if (Application.Current is null)
		{
			return;
		}

		foreach (Window window in Application.Current.Windows)
		{
			ApplyTitleBar(window, isDark);
		}
	}

	/// <summary>
	/// 用 DWM 把系统标题栏切成深色或浅色。
	/// 主窗口仍是系统边框，不套 ControlzEx 的自绘标题栏。
	/// </summary>
	private static void ApplyTitleBar(Window window, bool isDark)
	{
		if (window is null)
		{
			return;
		}

		// 句柄还没创建时（窗口尚未 SourceInitialized）刷了也没用
		nint handle = new WindowInteropHelper(window).Handle;
		if (handle == 0)
		{
			return;
		}

		int useDark = isDark ? 1 : 0;
		// 20 是 20H1 及以后的沉浸式深色标题栏；旧系统再试 19
		if (DwmSetWindowAttribute(handle, 20, ref useDark, sizeof(int)) != 0)
		{
			DwmSetWindowAttribute(handle, 19, ref useDark, sizeof(int));
		}
	}

	[DllImport("dwmapi.dll")]
	private static extern int DwmSetWindowAttribute(nint hwnd, int attr, ref int attrValue, int attrSize);
}
