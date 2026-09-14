using SimpleCollectionHub.Framework.Configs;
using SimpleCollectionHub.Framework.ViewModelServices;
using RW.Base.WPF.Extensions;
using RW.Base.WPF.Interfaces;
using RW.Base.WPF.Services;
using RW.Common.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SimpleCollectionHub.Framework.Utilities;

/// <summary>
/// 诊断信息生成类，把数据、日志、调试目录连同一份运行环境快照打包成 zip。
/// 实际打包逻辑由框架的 <see cref="DiagnosticsService"/> 完成，这里只负责决定收集哪些目录。
/// </summary>
public class DiagnosticsGenerator(IApplication application, AppFolderConfig folderConfig)
{
	/// <summary> 日志与调试文件的收集天数上限，超过则不打包 </summary>
	public int MaxLogDays { get; } = 7;

	/// <summary> 生成诊断压缩包并在资源管理器中定位 </summary>
	public async Task GenerateDiagnostics(string zipFilePath)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();

		try
		{
			await Task.Run(() =>
			{
				TimeSpan cutOff = TimeSpan.FromDays(Math.Abs(MaxLogDays));

				// 数据目录全量收集，日志和调试目录只取最近 MaxLogDays 天
				List<DiagnosticsService.Folder> folders = [
					new(folderConfig.DataFolder, SearchOption: SearchOption.AllDirectories),
					new(folderConfig.LoggingFolder, SearchOption: SearchOption.AllDirectories, CutOffTimeSpan: cutOff),
					new(folderConfig.DebugFolder, SearchOption: SearchOption.AllDirectories, CutOffTimeSpan: cutOff),
				];

				DiagnosticsService.CreateDiagnosticsZip(
					application,
					stopwatch,
					new DiagnosticsService.Parameter(zipFilePath, folders)
				);

				zipFilePath.ShowInExplorerWithMessage();
			});
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			DebugLog.LogHandledException(ex, "GenerateDiagnostics");

			// 可能在后台线程抛出，弹窗必须回到 UI 线程
			_ = DispatcherHelper.AppDispatcher.BeginInvoke(() =>
			{
				new MessageBoxServiceEx().ShowError("生成诊断信息发生错误", ex);
			});
		}
		finally
		{
			stopwatch.Stop();
			Debug.WriteLine(stopwatch.ElapsedMilliseconds);
		}
	}
}
