using RW.Base.WPF.ViewModels;

namespace SimpleCollectionHub.Framework.Configs;

/// <summary>
/// 应用程序管理器，向框架提供应用名称、构建模式等元数据。
/// 由 App 通过 <c>GetAppManager()</c> 创建，全局唯一。
/// </summary>
public class AppManagerEx : AppManager
{
	private static AppManagerEx? instance;

	/// <summary> 全局唯一实例，在构造函数中赋值 </summary>
	public static AppManagerEx Instance => instance!;

	public AppManagerEx()
	{
		instance = this;
	}

	// Author 决定 LocalAppData 下的一级目录名，见 AppFolderConfig.LocalAppFolder
	public override string Author => AppConfig.CompanyName;
	public override string AppName => AppConfig.AppName;
	public override string BuildMode => AppConfig.BuildMode;
	public override bool IsRelease => AppConfig.IsRelease;
}
