---
name: reference-typed-ui-object-service
description: TypedUIObjectService 完整 API 参考和服务对比。当用户使用 TypedUIObjectService、获取 View 中命名控件、查看 ViewModel 服务与容器服务的区别、或需要 IUIObjectService<T> 类型转换时自动加载。
disable-model-invocation: false
---

# TypedUIObjectService 完整参考

## 服务类型对比

| 类型 | 是否继承 ServiceBase | 是否实现 ISingletonDependency | 注册方式 | 获取方式 | 是否需要 .As<T>() |
|---|---|---|---|---|---|
| 容器服务 | ❌ 通常不继承 | ✅ 通常实现 | AutoFac 自动 | `GetService<IService>()` | ❌ 不需要 |
| ViewModel 服务 | ✅ 必须继承 ServiceBase | ❌ 不实现 | XAML 中手动注册（使用 Name 或 x:Name） | `GetService<IService>(nameof(Name))` 或 `GetService<IService>()`（单个实例时） | ❌ 不需要 |
| **TypedUIObjectService** | ✅ 必须继承 ServiceBase | ❌ 不实现 | XAML 中手动注册（使用 Name 或 x:Name） | `GetService<ITypedUIObjectService>(nameof(Name))` 或 `GetService<ITypedUIObjectService>()`（单个实例时） | ✅ **必需** |

## XAML 注册

### 有 Name（区分多个实例）

```xml
<dxmvvm:Interaction.Behaviors>
    <base:TypedUIObjectService Name="Grid1Service"/>
    <base:TypedUIObjectService Name="Grid2Service"/>
</dxmvvm:Interaction.Behaviors>

<DataGrid x:Name="DataGrid1"/>
<DataGrid x:Name="DataGrid2"/>
```

### 无 Name（单个实例）

```xml
<dxmvvm:Interaction.Behaviors>
    <base:TypedUIObjectService/>
</dxmvvm:Interaction.Behaviors>
```

## ViewModel 中使用

### 单个实例

```csharp
public IUIObjectService<Grid> GridService => GetService<ITypedUIObjectService>().As<Grid>();

private void ToggleGrid()
{
    Grid grid = GridService.Object;
    grid.Visibility = Visibility.Collapsed;
}
```

### 多个实例

```csharp
public IUIObjectService<Grid> Grid1Service => GetService<ITypedUIObjectService>(nameof(Grid1Service)).As<Grid>();
public IUIObjectService<Grid> Grid2Service => GetService<ITypedUIObjectService>(nameof(Grid2Service)).As<Grid>();
```

## TypedUIObjectService 与其它 ViewModel 服务的区别

| 特性 | TypedUIObjectService | 其它 ViewModel 服务（如 ILoadedService） |
|---|---|---|
| 获取方式 | `GetService<ITypedUIObjectService>(nameof(Name)).As<T>()` 或 `GetService<ITypedUIObjectService>().As<T>()` | `GetService<IService>(nameof(Name))` 或 `GetService<IService>()` |
| 是否需要 `nameof(Name)` | 多个实例时需要，单个实例时不需要 | 多个实例时需要，单个实例时不需要 |
| 是否需要 `.As<T>()` | ✅ 必需 | ❌ 不需要 |
| 用途 | 访问 View 中的命名元素，支持多种控件类型 | 控件生命周期管理、命令绑定等 |
| XAML 注册 | 使用 `Name` 或 `x:Name` 属性，如 `<base:TypedUIObjectService Name="Grid1Service"/>`；单个实例时可省略 Name | 使用 `Name` 或 `x:Name` 属性，如 `<base:LoadedService Name="MyLoadedService"/>`；单个实例时可省略 Name |

## 常见 ViewModel 服务

| 服务 | 类型 | 用途 | 获取方式 |
|---|---|---|---|
| `ILoadedService` | ViewModel 服务 | 控件加载后执行命令 | 单个实例：`GetService<ILoadedService>()`；多个实例：`GetService<ILoadedService>(nameof(Name))` |
| `ITypedUIObjectService` | ViewModel 服务 | 访问 View 中的命名元素 | 单个实例：`GetService<ITypedUIObjectService>().As<T>()`；多个实例：`GetService<ITypedUIObjectService>(nameof(Name)).As<T>()` |

## 关键点

1. **TypedUIObjectService 是 ViewModel 服务**，不是容器服务
2. **必须在 XAML 中通过 `<dxmvvm:Interaction.Behaviors>` 注册**，使用 `Name` 或 `x:Name` 属性
3. **如果只有一个实例，XAML 中可以不写 Name**，ViewModel 中直接 `GetService<ITypedUIObjectService>()` 获取
4. **如果有多个实例，XAML 中需指定 Name**，ViewModel 中通过 `GetService<ITypedUIObjectService>(nameof(Name))` 获取
5. **必须使用 `.As<T>()` 转换成特定类型**（这是与其它 ViewModel 服务的核心区别）
6. **不要在构造函数中注入**（会报错，因为未在 AutoFac 中注册）
