using SimpleCollectionHub.Framework;
using SimpleCollectionHub.Strings;
using RW.Base.WPF.Interfaces;
using RW.Common.Interfaces;
using RW.Common.Utilities;
using RW.Common.WPF.Converters;
using RW.Common.WPF.Utilities;
using System;
using System.Collections.Generic;

namespace SimpleCollectionHub.Framework.Services;

/// <summary> 枚举显示服务，提供枚举值到本地化文本的映射 </summary>
public interface IEnumDisplayService
{
	/// <summary> 枚举显示管理器 </summary>
	EnumDisplayManagerBase EnumDisplayManager { get; }
}

/// <inheritdoc cref="IEnumDisplayService"/>
internal class EnumDisplayService : IEnumDisplayService, IAppInitialize
{
	string IAppInitialize.Description { get; } = "初始化枚举显示服务中";
	int IPriority.Priority { get; } = IntPriority.High;

	/// <summary> 内部枚举显示管理器实例，用于提供枚举到显示文本的映射 </summary>
	public static EnumDisplayManagerBase EnumDisplayManager { get; } = new _EnumDisplayManager();
	EnumDisplayManagerBase IEnumDisplayService.EnumDisplayManager => EnumDisplayManager;

	void IAppInitialize.AppInitialize(IStatusReport statusReport)
	{
		// 将本服务的 EnumDisplayManager 注入到通用的转换器中
		EnumDisplayConverter.EnumDisplayManager = EnumDisplayManager;
	}

	/// <summary> 默认按 AppStrings 资源名查找枚举文本，找不到时回退到自定义处理器 </summary>
	private class _EnumDisplayManager() : EnumDisplayManagerWPF(typeof(AppStrings))
	{
		/// <summary> 获取默认的枚举显示处理程序字典（可在此处扩展默认处理器） </summary>
		protected override Dictionary<Type, IEnumDisplayHandler> GetDefaultHandlers()
		{
			Dictionary<Type, IEnumDisplayHandler> dict = base.GetDefaultHandlers();

			// 在此处添加或替换默认处理程序，为特定枚举类型提供自定义显示逻辑，例如：
			// dict[typeof(MyEnum)] = L<MyEnum>(e => e switch
			// {
			//     MyEnum.A => "甲",
			//     _ => e.ToString(),
			// });

			return dict;
		}
	}
}

/// <summary> 枚举显示扩展方法 </summary>
public static class EnumDisplayExtensions
{
	/// <summary> 获取枚举的本地化显示文本 </summary>
	public static string GetEnumDisplay(this Enum @enum)
	{
		return EnumDisplayService.EnumDisplayManager.GetDisplayText(@enum);
	}
}
