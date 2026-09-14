using MapsterMapper;
using RW.Base.WPF.Events;
using RW.Base.WPF.Extensions;
using System;
using System.Collections.Concurrent;

namespace SimpleCollectionHub.Framework.Services;

/// <summary>
/// 服务池。为不方便走构造函数注入的场景（静态方法、附加属性、转换器等）
/// 提供一个带缓存的服务定位入口。
/// </summary>
/// <remarks>
/// 常规业务代码应优先使用构造函数注入，只有拿不到容器的地方才用这里。
/// </remarks>
public static class ServicePool
{
	/// <summary> 事件聚合器，用于跨模块的发布订阅 </summary>
	public static IEventAggregator EventAggregator => GetService<IEventAggregator>();

	/// <summary> Mapster 映射器 </summary>
	public static IMapper Mapper => GetService<IMapper>();

	// 解析结果缓存，避免每次访问都走一遍容器
	private static readonly ConcurrentDictionary<Type, object> cache = [];

	/// <summary> 获取注册或缓存的服务；容器尚未建立时返回 default </summary>
	public static T GetService<T>() where T : notnull
	{
		if (cache.TryGetValue(typeof(T), out object service) && service is T t)
		{
			return t;
		}

		// 容器在 ApplicationBase.LoadModules 中建立，早于此的调用只能拿到 null
		if (IoC.Container is null)
		{
			return default;
		}

		T resolved = IoC.Resolve<T>();
		cache[typeof(T)] = resolved;
		return resolved;
	}
}
