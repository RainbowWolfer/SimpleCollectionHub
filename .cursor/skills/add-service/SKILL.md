---
name: add-service
description: >-
  在 SimpleCollectionHub 客户端中新建一个带接口的 Service，并通过 AutoFac 自动注册。
  当用户需要添加新的业务服务、数据服务、或状态管理服务时使用。
disable-model-invocation: false
---

# 新建客户端 Service

## 根据生命周期选择模式

| 需求 | 实现接口 |
|------|---------|
| 单例，无需初始化 | `ISingletonDependency` |
| 单例，需要异步启动初始化 | `ISingletonDependency` + `IAppInitializeAsync` |
| 单例，需要同步启动初始化 | `ISingletonDependency` + `IAppInitialize` |
| 单例，需要在应用关闭时清理 | + `IAppClosedHandler` |

## 模板

### 最简单的单例 Service

```csharp
using RW.Base.WPF.DependencyInjections;

namespace SimpleCollectionHub.<模块名>.Services;

public interface IXxxService
{
    void DoSomething();
}

internal class XxxService : IXxxService, ISingletonDependency
{
    public void DoSomething() { }
}
```

### 需要启动初始化的 Service

```csharp
using RW.Base.WPF.DependencyInjections;
using RW.Base.WPF.Interfaces;

namespace SimpleCollectionHub.<模块名>.Services;

public interface IXxxService { }

internal class XxxService(IAppSettingsService settingsService) : IXxxService, IAppInitializeAsync
{
    string IAppInitializeAsync.Description { get; } = "初始化 Xxx 中";
    int IPriority.Priority { get; } = IntPriority.Normal;

    async Task IAppInitializeAsync.AppInitializeAsync(IStatusReport statusReport)
    {
        // 异步初始化逻辑
    }
}
```

### 需要初始化 + 关闭清理的 Service

```csharp
internal class XxxService(IAppSettingsService settingsService) : IXxxService, IAppInitializeAsync, IAppClosedHandler
{
    string IAppInitializeAsync.Description { get; } = "初始化 Xxx 中";
    int IPriority.Priority { get; } = IntPriority.Normal;

    int IAppClosedHandler.Priority { get; } = IntPriority.Normal;
    string IAppClosedHandler.Description { get; } = "关闭 Xxx 中";

    async Task IAppInitializeAsync.AppInitializeAsync(IStatusReport statusReport)
    {
        // 初始化
    }

    void IAppClosedHandler.AppClosed()
    {
        // 清理资源
    }
}
```

## 注意事项

- 生命周期接口来自 `RW.Base.WPF.DependencyInjections`（`ISingletonDependency`、`ITransientDependency`）
  与 `RW.Base.WPF.Interfaces`（`IAppInitialize`、`IAppInitializeAsync`、`IAppClosedHandler`、`IPriority`）
- 实现 `ISingletonDependency` 后 AutoFac **自动注册为单例**，无需手动注册；
  扫描由 `RW.Base.WPF.Extensions.IoCInitializer` 在启动时完成
- 实现 `IAppInitializeAsync` / `IAppInitialize` 时必须同时实现 `IPriority`（提供 `Priority` 属性）
- 接口定义 `public`，实现类 `internal`
- 优先级使用 `IntPriority` 常量（`VeryHigh`、`High`、`Normal`、`Lower`、`VeryLow`）

## 检查清单

- [ ] 接口 `public`，实现类 `internal`
- [ ] 根据需要选择了正确的生命周期接口组合
- [ ] 有初始化/关闭逻辑时实现了 `Description` 和 `Priority`
- [ ] 使用 Primary Constructor 注入依赖