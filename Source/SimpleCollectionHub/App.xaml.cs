using Autofac;
using SimpleCollectionHub.Framework.Configs;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Resources;
using SimpleCollectionHub.ViewModels;
using SimpleCollectionHub.Views;
using RW.Base.WPF;
using RW.Base.WPF.Configs;
using RW.Base.WPF.Extensions;
using RW.Base.WPF.Interfaces;
using RW.Base.WPF.ViewModels;
using RW.Common.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace SimpleCollectionHub;

public partial class App : ApplicationBase
{
	static App()
	{
		// 框架内部的调试输出统一转到 Output 窗口
		DebugConfig.Print = o => Debug.WriteLine(o);
		FileActionHelper.OnException = FileActionHelperOnException;
	}

	private AppSplashScreen? AppSplashScreen { get; set; }
	private AppSplashScreenViewModel? AppSplashScreenViewModel { get; set; }

	/// <summary>
	/// 软件设置与用户配置在容器建立之前就要读取（<see cref="GetCultureInfo"/> 会用到），
	/// 因此在这里手工构造，稍后由 <see cref="_IoCInitializer"/> 注册为单例。
	/// </summary>
	private AppSettingsService AppSettingsService { get; }
	private AppProfilesService AppProfilesService { get; }

	public App()
	{
		// 以 exe 所在目录为工作目录，避免从快捷方式启动时相对路径出错
		Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

		AppFolderConfig folderConfig = (AppFolderConfig)FolderConfig;

		AppSettingsService = new AppSettingsService(folderConfig);
		AppSettingsService.LoadSettings();

		AppProfilesService = new AppProfilesService(folderConfig);
		AppProfilesService.LoadSettings();

		Debug.WriteLine(AppManager.GetDebugInfo());
	}

	#region 启动流程

	/// <summary>
	/// 框架在加载模块前调用。此时容器尚未建立，只能做纯 UI / 静态资源的准备。
	/// </summary>
	protected override void BeforeLoadingModules()
	{
		base.BeforeLoadingModules();

		ResourceKeys.Initialize();
	}

	/// <summary>
	/// 创建启动页并立即显示，返回的对象会作为模块初始化的进度回报接收方。
	/// </summary>
	protected override IStatusReport InitializeStatusReport()
	{
		AppSplashScreenViewModel = new AppSplashScreenViewModel(AppManager);

		AppSplashScreen = new AppSplashScreen()
		{
			DataContext = AppSplashScreenViewModel,
		};
		AppSplashScreen.Show();

		return AppSplashScreenViewModel;
	}

	/// <summary> 所有 IAppInitialize / IAppInitializeAsync 执行完毕，主窗口尚未创建 </summary>
	protected override void AfterLoadingModules()
	{
		base.AfterLoadingModules();
	}

	/// <summary> 主窗口已显示，收起启动页 </summary>
	protected override void Loaded()
	{
		base.Loaded();

		AppSplashScreen?.Close();
		AppSplashScreen = null;
	}

	/// <summary> 设置软件语言。此时容器尚未建立，只能读取已手工构造的设置服务 </summary>
	protected override CultureInfo? GetCultureInfo()
	{
		AppSettingsModel model = AppSettingsService.Model;

		return new CultureInfo("zh-CN");
	}

	#endregion

	#region 基本上每一个项目的固定项

	protected override Window? GetMainWindow() => IoC.Resolve<MainWindow>();

	protected override AppManager GetAppManager() => new AppManagerEx();

	protected override FolderConfig GetFolderConfig(IAppManager appManager) => new AppFolderConfig(appManager);

	protected override DllLoader GetDllLoader() => new _DllLoader();

	protected override IoCInitializer GetIoCInitializer(IApplication application) => new _IoCInitializer(application);

	// 如果不需要 Pipe 功能，设置为 false 可以略微提升启动速度
	protected override bool EnablePipeServerStream => true;

	protected override string GetPipeName() => $"{AppConfig.CompanyName}.{AppConfig.AppName}";
	protected override string GetMutexName() => $"{AppConfig.CompanyName}.{AppConfig.AppName}";

	protected override void LogException(Exception exception, string flag)
	{
		Debug.WriteLine($"{flag} - {exception}");
		base.LogException(exception, flag);
	}

	protected override void ShowFatalDialog(Exception exception)
	{
		MessageBox.Show(exception.ToString(), "发生致命性错误！", MessageBoxButton.OK, MessageBoxImage.Error);
	}

	private static void FileActionHelperOnException(Exception exception)
	{
		Debug.WriteLine(exception);
		DebugLog.LogHandledException(exception, "ShowInExplorer");
		MessageBox.Show(exception.ToString(), "发生错误，无法打开资源管理器", MessageBoxButton.OK, MessageBoxImage.Error);
	}

	#endregion

	/// <summary> 程序集扫描器，在这里排除不需要反射的程序集以缩短启动时间 </summary>
	private class _DllLoader : DllLoader
	{
		private readonly HashSet<string> skipSet = [

		];

		protected override IEnumerable<string> AdditionalSkipSet() => skipSet;

		protected override bool MatchAssembly(AssemblyFileInfo fileInfo)
		{
			if (!base.MatchAssembly(fileInfo))
			{
				return false;
			}

			Debug.WriteLine($"Match Assembly: {fileInfo.FileName}");
			return true;
		}
	}

	/// <summary> 容器初始化器，用于注册无法被自动发现的实例 </summary>
	private class _IoCInitializer(IApplication application) : IoCInitializer(application)
	{
		private readonly App application = (App)application;

		protected override void InitializeDependencies()
		{
			base.InitializeDependencies();

			builder.RegisterInstance(application.AppSettingsService).As<IAppSettingsService>().AsSelf();
			builder.RegisterInstance(application.AppProfilesService).As<IAppProfilesService>().AsSelf();
		}
	}
}
