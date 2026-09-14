using RW.Base.WPF.Interfaces;
using RW.Base.WPF.ViewModels;

namespace SimpleCollectionHub.ViewModels;

/// <summary>
/// 启动页视图模型。继承 <see cref="StatusReport"/>，
/// 框架初始化各模块时会把进度文本写进来。
/// </summary>
public class AppSplashScreenViewModel(IAppManager appManager) : StatusReport
{
	/// <summary> 启动页上显示的标题，可在此展示公司、版本、版权等信息 </summary>
	public string TitleName
	{
		get => GetProperty(() => TitleName);
		set => SetProperty(() => TitleName, value);
	}

	public IAppManager AppManager { get; } = appManager;
}
