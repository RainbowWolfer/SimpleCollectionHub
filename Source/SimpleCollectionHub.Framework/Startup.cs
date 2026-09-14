using Autofac;
using FreeSql;
using RW.Base.WPF.Interfaces;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Configs;
using System.IO;

namespace SimpleCollectionHub.Framework;

/// <summary>
/// Framework 层的容器注册入口。框架在建立容器时会自动发现并调用所有 <see cref="IStartup"/>。
/// </summary>
/// <remarks>
/// 以下内容框架已经自动处理，不需要在这里重复：
/// <list type="bullet">
/// <item>实现 <c>ISingletonDependency</c> / <c>ITransientDependency</c> 的类型</item>
/// <item>Mapster 的 <c>IRegister</c> 与 <c>IMapper</c></item>
/// <item><c>IApplication</c>、<c>AppManager</c>、<c>FolderConfig</c>、<c>IEventAggregator</c></item>
/// </list>
/// 只有需要手工注册的实例（第三方对象、工厂、按名注册等）才写在这里。
/// </remarks>
internal class Startup : IStartup
{
	// 数值越大越先执行，Framework 层需要早于业务模块
	int IStartup.Priority { get; } = int.MaxValue;

	void IStartup.Initialize(ContainerBuilder builder)
	{
		// IFreeSql 不是标记接口，必须手工注册。解析发生在容器建好之后，此时 AppFolderConfig 已在。
		builder.Register(CreateFreeSql).As<IFreeSql>().SingleInstance();
	}

	private static IFreeSql CreateFreeSql(IComponentContext context)
	{
		AppFolderConfig folder = context.Resolve<AppFolderConfig>();
		string directory = Path.GetDirectoryName(folder.AppDatabaseFilePath);
		if (directory.IsNotBlank())
		{
			Directory.CreateDirectory(directory);
		}

		IFreeSql sql = new FreeSqlBuilder()
			.UseConnectionString(DataType.Sqlite, $"Data Source={folder.AppDatabaseFilePath}")
			.UseAutoSyncStructure(true)
			.Build();
		sql.UseJsonMap();
		return sql;
	}
}
