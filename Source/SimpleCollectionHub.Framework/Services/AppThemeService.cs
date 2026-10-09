using RW.Base.WPF.DependencyInjections;
using RW.Base.WPF.Interfaces;
using SimpleCollectionHub.Framework;

namespace SimpleCollectionHub.Framework.Services;

/// <summary>浅色/深色主题。选择写在本机设置里，下次启动沿用。</summary>
public interface IAppThemeService
{
	/// <summary>当前是否为深色主题。</summary>
	bool IsDarkTheme { get; }

	/// <summary>在浅色和深色之间切换，并保存。</summary>
	void ToggleTheme();
}

/// <summary>
/// 启动时按设置套用主题。设置本身在 App 构造函数里已经读过，这里只负责再套一次并响应切换。
/// </summary>
internal class AppThemeService(IAppSettingsService settingsService) : IAppThemeService, ISingletonDependency, IAppInitialize
{
	string IAppInitialize.Description => "应用主题";

	// 早于界面服务，避免主窗口先用系统主题画一帧
	int IPriority.Priority => IntPriority.Higher;

	void IAppInitialize.AppInitialize(IStatusReport statusReport)
	{
		// App.BeforeLoadingModules 已经套过一次，这里再套是为了模块初始化之后窗口标题栏也能跟上
		AppTheme.Apply(settingsService.Model.IsDarkTheme);
	}

	public bool IsDarkTheme => settingsService.Model.IsDarkTheme;

	public void ToggleTheme()
	{
		// 先落盘再改界面，避免切换成功但下次启动又回到旧主题
		bool isDark = !settingsService.Model.IsDarkTheme;
		settingsService.Model.IsDarkTheme = isDark;
		settingsService.SaveSettings();
		AppTheme.Apply(isDark);
	}
}
