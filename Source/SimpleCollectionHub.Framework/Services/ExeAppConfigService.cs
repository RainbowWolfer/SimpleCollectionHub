using SimpleCollectionHub.Framework.Configs;
using Newtonsoft.Json.Linq;
using RW.Base.WPF.Extensions;
using RW.Base.WPF.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SimpleCollectionHub.Framework.Services;

/// <summary> 读取 bin 目录下的 Properties/AppConfig.json 的内容 </summary>
public interface IExeAppConfigService
{

}

/// <inheritdoc cref="IExeAppConfigService"/>
internal class ExeAppConfigService(AppFolderConfig folderConfig) : IExeAppConfigService, IAppInitializeAsync
{
	string IAppInitializeAsync.Description { get; } = "读取初始配置中";

	// 随 exe 发布的配置是其它初始化的前提，优先级设为最高
	int IPriority.Priority { get; } = int.MaxValue;

	async Task IAppInitializeAsync.AppInitializeAsync(IStatusReport statusReport)
	{
		try
		{
			string filePath = folderConfig.ExeAppConfigFile;

			// 配置文件是可选的，缺失时走默认值即可
			if (!File.Exists(filePath))
			{
				return;
			}

			string json = await File.ReadAllTextAsync(filePath);

			JObject obj = JObject.Parse(json);
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			DebugLog.LogHandledException(ex, "ExeAppConfigService");
		}
	}
}
