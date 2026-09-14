---
name: add-view
description: >-
  在 SimpleCollectionHub 客户端现有模块中新建一个 View + ViewModel 对。
  当用户需要在模块内添加新页面、子视图时使用。
disable-model-invocation: false
---

# 新建 View + ViewModel

## 命名约定（关键）

View 和 ViewModel 的命名遵循统一规则：

- `Xxx` = 功能名称（不含 `View` 后缀），如 `AlarmLog`、`MonitorMain`、`ParameterConfig`
- View 文件：`XxxView.xaml`（如 `AlarmLogView.xaml`）
- ViewModel 类：`XxxViewModel`（如 `AlarmLogViewModel`）
- ViewModel 文件（拆分时）：`ViewModels/XxxViewModel.cs`

**命名反例（禁止）：**

| ❌ 错误 | ✅ 正确 | 说明 |
|---|---|---|
| `AlarmLogViewViewModel.cs` | `AlarmLogViewModel.cs` | `Xxx` = `AlarmLog`，不要保留 View 后缀 |
| `MainConsoleMainViewViewModel.cs` | `MainConsoleMainViewModel.cs` | 同理 |
| `DataManagementViewViewModel.cs` | `DataManagementViewModel.cs` | 同理 |

**当用户提到的功能名已包含 "View" 后缀时，必须去掉它再添加 ViewModel 后缀。**

## Step 1 — 创建 ViewModel

**优先将 ViewModel 写在 `.xaml.cs` 文件末尾**（UserControl class 之后），视图不复杂时无需单独建文件：

```csharp
// XxxView.xaml.cs

namespace SimpleCollectionHub.<模块名>.Views;

public partial class XxxView : UserControl
{
    public XxxView() => InitializeComponent();
}

internal class XxxViewModel : ViewModelBaseEx
{
    protected override void LoadedOnce()
    {
        // 首次加载初始化
    }
}
```

ViewModel 逻辑较多导致文件过长时，再拆到 `ViewModels/XxxViewModel.cs` 单独文件。

## Step 2 — 创建 View（XAML）

先读取 `Source/XAML_Preset.txt` 中 `-------------- View --------------` 区段，复制全部 xmlns 属性，然后创建：

```xml
<UserControl
    x:Class="SimpleCollectionHub.<模块名>.Views.XxxView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:SimpleCollectionHub.<模块名>.Views"
    xmlns:base="RW.Base.WPF"
    ...（其余 xmlns 从 XAML_Preset.txt 复制）
    d:DataContext="{d:DesignInstance Type=local:XxxViewModel}"
    Style="{DynamicResource UserControlStyle}"
    DataContext="{base:DISource Type=local:XxxViewModel}"
    d:DesignHeight="450"
    d:DesignWidth="800">
    <Grid>
    </Grid>
</UserControl>
```

## Step 3 — code-behind

保持最简，不写业务逻辑：

```csharp
namespace SimpleCollectionHub.<模块名>.Views;

public partial class XxxView : UserControl
{
    public XxxView()
    {
        InitializeComponent();
    }
}
```

## 检查清单

- [ ] ViewModel 继承 `ViewModelBaseEx`
- [ ] View 设置 `Style="{DynamicResource UserControlStyle}"`
- [ ] View 使用 `DataContext="{base:DISource Type=local:XxxViewModel}"` 绑定
- [ ] `d:DataContext` 设计时数据上下文已配置
- [ ] code-behind 无业务逻辑