---
name: add-module
description: >-
  在 SimpleCollectionHub 客户端中添加一个新的功能模块，包含完整的目录结构、AppModule 元数据、
  主视图和 ViewModel。当用户需要新建客户端模块时使用。
disable-model-invocation: false
---

# 添加客户端功能模块

## 目录结构

在 `Source/Modules/<模块名>/` 下创建：

```
SimpleCollectionHub.<模块名>/
├── SimpleCollectionHub.<模块名>.csproj
├── AppModule.cs
├── Views/
│   ├── <模块名>MainView.xaml
│   └── <模块名>MainView.xaml.cs
└── ViewModels/
    └── <模块名>MainViewModel.cs
```

## Step 1 — 创建 .csproj

参考现有模块的 `.csproj`，核心是引用 Framework 项目。

## Step 2 — AppModule.cs

实现 `SimpleCollectionHub.Framework.Interfaces.IAppModule` 接口：

```csharp
using SimpleCollectionHub.<模块名>.Views;
using SimpleCollectionHub.Framework.Configs;
using SimpleCollectionHub.Framework.Helpers;
using SimpleCollectionHub.Framework.Interfaces;
using SimpleCollectionHub.Resources.Icons;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleCollectionHub.<模块名>;

public class AppModule : IAppModule
{
    public string Key { get; } = ConstKeys.SK_XXX;
    public string DisplayName { get; } = "XXX（简短说明）";
    public string FullName { get; } = "模块完整中文名";
    public string EnglishFullName { get; } = "Module English Name";
    public string Description { get; } = "模块功能描述。";

    public ImageSource Icon { get; } = IconResources.Icon_XXX_PNG.ToImageSource();
    public ContentControl Content { get; } = new <模块名>MainView();

    public bool IsEnabled { get; } = true;
}
```

- `Key`：从 Framework 的常量类中取对应的 `SK_XXX` 常量（项目里**没有** `SimpleCollectionHub.Shared` 程序集）
- `Icon`：图标文件放进 `SimpleCollectionHub.Resources/Icons/`，跑一次 T4 后用
  `IconResources.Icon_XXX_PNG` 取 pack URI，再用 `IconHelper.ToImageSource()` 转换；详见 `icon-usage` 规则

## Step 3 — 主 ViewModel

```csharp
using SimpleCollectionHub.Framework.ViewModels;

namespace SimpleCollectionHub.<模块名>.ViewModels;

internal class <模块名>MainViewModel : ViewModelBaseEx
{
    protected override void LoadedOnce()
    {
        // 首次加载初始化
    }
}
```

## Step 4 — 主 View（XAML）

先读取 `Source/XAML_Preset.txt` 中 `View` 模板，复制 xmlns 引用，然后：

```xml
<UserControl
    x:Class="SimpleCollectionHub.<模块名>.Views.<模块名>MainView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:SimpleCollectionHub.<模块名>.Views"
    xmlns:base="RW.Base.WPF"
    ...（其余 xmlns 从 XAML_Preset.txt 复制）
    d:DataContext="{d:DesignInstance Type=local:<模块名>MainViewModel}"
    Style="{DynamicResource UserControlStyle}"
    DataContext="{base:DISource Type=local:<模块名>MainViewModel}">
    <Grid>
    </Grid>
</UserControl>
```

## Step 5 — 添加到主程序

在 `Source/SimpleCollectionHub/SimpleCollectionHub.csproj` 中添加项目引用。
`AppModule` 会被主程序自动扫描，无需额外注册。

## 检查清单

- [ ] `AppModule` 实现 `IAppModule`，包含所有必要属性
- [ ] `Key` 使用 `ConstKeys.SK_XXX` 常量
- [ ] ViewModel 继承 `ViewModelBaseEx`
- [ ] View 使用 `DISource` 绑定 ViewModel，设置 `UserControlStyle`
- [ ] 主程序 `.csproj` 已添加项目引用