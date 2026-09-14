using SimpleCollectionHub.Framework.Configs;
using Newtonsoft.Json;
using RW.Base.WPF.Services;

namespace SimpleCollectionHub.Framework.Services;

/// <summary> 用户配置服务，读写路径为 LocalAppData 下的 AppProfiles.json </summary>
public interface IAppProfilesService : ISettingsServiceBase<AppProfilesModel>
{

}

/// <summary>
/// 用户配置服务。不标记 <c>ISingletonDependency</c>，
/// 因为实例在 App 构造函数中就要读取（早于容器建立），
/// 之后由 App 的 IoCInitializer 注册为单例。
/// </summary>
public class AppProfilesService(AppFolderConfig folderConfig)
	: JsonSettingsServiceBase<AppProfilesModel>, IAppProfilesService
{
	public override string FilePath => folderConfig.AppProfilesFilePath;
	public override AppProfilesModel GetDefaultModel() => new();
}

[JsonObject]
public class AppProfilesModel
{

}
