using RW.Base.WPF.Configs;
using RW.Base.WPF.Interfaces;
using System.IO;

namespace SimpleCollectionHub.Framework.Configs;

/// <summary>
/// 应用程序目录配置。继承自框架的 <see cref="FolderConfig"/>，
/// 由 App 通过 <c>GetFolderConfig()</c> 创建，构造时框架会调用 <c>Initialize()</c> 建好各级目录。
/// </summary>
/// <remarks>
/// 基类已提供的目录：
/// <c>AppFolder</c>（exe 所在目录）、<c>LocalFolder</c>（AppData/Local）、
/// <c>TargetFolder</c>（AppData/Local/公司名/软件名，即数据根目录）、
/// <c>LoggingFolder</c>、<c>DataFolder</c>、<c>DebugFolder</c>。
/// </remarks>
public class AppFolderConfig : FolderConfig
{
	private static AppFolderConfig? instance;

	/// <summary> 全局唯一实例，在构造函数中赋值 </summary>
	public static AppFolderConfig Instance => instance!;

	public AppFolderConfig(IAppManager appManager) : base(appManager)
	{
		instance = this;
	}

	/// <summary> exe 同级的 Properties 目录 </summary>
	public string ExePropertiesFolder => Path.Combine(AppFolder, "Properties");

	/// <summary> 随 exe 一起发布的初始配置文件 </summary>
	public string ExeAppConfigFile => Path.Combine(ExePropertiesFolder, "AppConfig.json");

	/// <summary> 软件设置文件 </summary>
	public string AppSettingsFilePath => Path.Combine(DataFolder, "AppSettings.json");

	/// <summary> 用户配置文件 </summary>
	public string AppProfilesFilePath => Path.Combine(DataFolder, "AppProfiles.json");

	/// <summary> SQLite 数据库文件 </summary>
	public string AppDatabaseFilePath => Path.Combine(DataFolder, "AppData.db");

	/// <summary>
	/// 条目图片副本目录。只写软件自己的 LocalAppData，不写用户绑定的原路径。
	/// </summary>
	public string MediaFolder => Path.Combine(DataFolder, "Media");
}
