using FreeSql;
using RW.Base.WPF.DependencyInjections;
using RW.Base.WPF.Interfaces;
using SimpleCollectionHub.Framework.Configs;
using System.IO;
using System.Threading.Tasks;

namespace SimpleCollectionHub.Framework.Database;

/// <summary>收藏库数据库启动：建媒体目录、同步表结构。</summary>
public interface IDatabaseService
{
	/// <summary>当前 SQLite 实例。</summary>
	IFreeSql FreeSql { get; }
}

/// <summary>
/// 在应用初始化阶段把 LocalAppData 里的库准备好。
/// 不触碰用户绑定的原目录。
/// </summary>
internal class DatabaseService(
	IFreeSql freeSql,
	AppFolderConfig folderConfig
) : IDatabaseService, IAppInitializeAsync
{
	string IAppInitializeAsync.Description { get; } = "初始化收藏库";

	int IPriority.Priority { get; } = IntPriority.High;

	public IFreeSql FreeSql => freeSql;

	async Task IAppInitializeAsync.AppInitializeAsync(IStatusReport statusReport)
	{
		Directory.CreateDirectory(folderConfig.DataFolder);
		Directory.CreateDirectory(folderConfig.MediaFolder);

		freeSql.CodeFirst.SyncStructure(
			typeof(CollectionGroup),
			typeof(CollectionItem),
			typeof(CollectionField)
		);

		await Task.CompletedTask;
	}
}
