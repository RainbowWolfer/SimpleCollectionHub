---
name: reference-enum-display
description: EnumDisplayService 内部实现和自定义枚举显示逻辑完整指南。当用户需要扩展枚举显示、添加自定义枚举翻译、在 EnumDisplayService 中注册枚举显示处理器、或了解 EnumDisplayManager 机制时自动加载。
disable-model-invocation: false
---

# EnumDisplayService 完整参考

## 服务初始化

`EnumDisplayService` 在应用启动时自动初始化（通过 `IAppInitialize`），并将 `EnumDisplayManager` 注入到 `EnumDisplayConverter` 中。

```csharp
// Framework/Services/EnumDisplayService.cs
internal class EnumDisplayService : IEnumDisplayService, IAppInitialize
{
    public static EnumDisplayManagerBase EnumDisplayManager { get; } = new _EnumDisplayManager();
    
    void IAppInitialize.AppInitialize(IStatusReport statusReport)
    {
        // 将 EnumDisplayManager 注入到通用转换器中
        EnumDisplayConverter.EnumDisplayManager = EnumDisplayManager;
    }

    private class _EnumDisplayManager() : EnumDisplayManagerWPF(typeof(AppStrings))
    {
        protected override Dictionary<Type, IEnumDisplayHandler> GetDefaultHandlers()
        {
            Dictionary<Type, IEnumDisplayHandler> dict = base.GetDefaultHandlers();
            
            // 为特定枚举类型添加自定义显示逻辑
            dict[typeof(MyStatus)] = L<MyStatus>(e => e switch
            {
                MyStatus.Active => "活动",
                MyStatus.Inactive => "非活动",
                _ => e.ToString()
            });

            return dict;
        }
    }
}
```

## 扩展枚举显示服务

有两种方式为自定义枚举添加显示逻辑。

### 方式 1：在 EnumDisplayService 中扩展（推荐）

在 `_EnumDisplayManager` 类中添加自定义处理器：

```csharp
private class _EnumDisplayManager() : EnumDisplayManagerWPF(typeof(AppStrings))
{
    protected override Dictionary<Type, IEnumDisplayHandler> GetDefaultHandlers()
    {
        Dictionary<Type, IEnumDisplayHandler> dict = base.GetDefaultHandlers();

        // 添加枚举显示处理器
        dict[typeof(DeviceState)] = L<DeviceState>(e => e switch
        {
            DeviceState.Online => "在线",
            DeviceState.Offline => "离线",
            DeviceState.Connecting => "连接中",
            _ => e.ToString()
        });

        return dict;
    }
}
```

### 方式 2：使用本地化字符串资源

如果枚举显示文本已定义在 `AppStrings` 中，`EnumDisplayManagerWPF` 会自动处理（按 `{EnumType}_{Value}` 命名约定映射）。

```csharp
// AppStrings.cs
public class AppStrings
{
    public static string MyStatus_Active => "活动";
    public static string MyStatus_Inactive => "非活动";
}

// 枚举值 MyStatus.Active 会自动映射到 AppStrings.MyStatus_Active
```

## C# 中使用

```csharp
// 获取枚举的显示文本
string displayText = MyStatus.Active.GetEnumDisplay(); // "活动"

// 在 ViewModel 中使用
public string StatusText => Status.GetEnumDisplay();
```
