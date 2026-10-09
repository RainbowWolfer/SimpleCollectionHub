using SimpleCollectionHub.Framework.Configs;
using Newtonsoft.Json;
using RW.Base.WPF.Services;

namespace SimpleCollectionHub.Framework.Services;

/// <summary> 软件设置服务，读写路径为 LocalAppData 下的 AppSettings.json </summary>
public interface IAppSettingsService : ISettingsServiceBase<AppSettingsModel>
{

}

/// <summary>
/// 软件设置服务。不标记 <c>ISingletonDependency</c>，
/// 因为实例在 App 构造函数中就要读取（早于容器建立），
/// 之后由 App 的 IoCInitializer 注册为单例。
/// </summary>
public class AppSettingsService(AppFolderConfig folderConfig)
	: JsonSettingsServiceBase<AppSettingsModel>, IAppSettingsService
{
	public override string FilePath => folderConfig.AppSettingsFilePath;
	public override AppSettingsModel GetDefaultModel() => new();
}

[JsonObject]
public class AppSettingsModel
{
	/// <summary>虚拟根「全部收藏」的 Fluent 码位，不含 &#x 前缀。</summary>
	public string AllIcon { get; set; } = "E8F1";

	/// <summary>虚拟根「全部收藏」的图标颜色。</summary>
	public string AllIconColor { get; set; } = "#326cf3";

	/// <summary>界面是否使用深色主题。默认浅色，和原先的界面一致。</summary>
	public bool IsDarkTheme { get; set; }
}
