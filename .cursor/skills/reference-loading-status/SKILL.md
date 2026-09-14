---
name: reference-loading-status
description: LoadingStatus 完整 API 参考和使用指南。当用户管理异步加载状态、显示加载进度、处理加载错误、绑定 LoadingCover 或 LoadingStatusIndicator 控件时自动加载。
disable-model-invocation: false
---

# LoadingStatus 完整参考

`LoadingStatus` 位于 `SimpleCollectionHub.Framework.ViewModels` 命名空间。

## API 概览

```csharp
namespace SimpleCollectionHub.Framework.ViewModels;

public class LoadingStatus : BindableBase
{
    // 核心属性
    public double? Progress { get; set; }       // 进度（null = 不确定）
    public string Message { get; set; }         // 加载提示文字
    public string ErrorMessage { get; set; }    // 错误信息
    public bool HasError { get; set; }          // 是否发生错误
    public bool IsLoading { get; set; }         // 是否正在加载

    // 计算属性
    public bool IsLoadedSuccess => !IsLoading && !HasError;
    public bool IsLoadingActive => IsLoading || HasError;

    // 状态切换
    public void Initialize(string message = "")  // 开始加载，IsLoading = true
    public void Done(string message = "")        // 加载完成，IsLoading = false
    public void Error(string errorMessage)       // 发生错误，HasError = true
    public void SetProgress(double progress)     // 更新进度
    public void Reset()                          // 重置到初始状态
    public void CopyErrorMessage()               // 复制错误信息到剪贴板
}
```

## ViewModel 使用模式

```csharp
internal class XxxViewModel : ViewModelBaseEx
{
    public LoadingStatus LoadingStatus { get; } = new();

    public IDelegateCommand LoadCommand => field ??= new AsyncCommand(Load);
    private async Task Load()
    {
        try
        {
            LoadingStatus.Initialize("读取文件...");
            await DoStuff();
            LoadingStatus.Done();
        }
        catch (OperationCanceledException)
        {
            LoadingStatus.Reset();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            LoadingStatus.Error(ex.Message);
        }
    }
}
```

## View 绑定 — LoadingCover

在内容 Grid 之上放置 `LoadingCover` 作为覆盖层，自动处理三态显示：

```xml
<Grid>
    <!-- 实际内容 -->
    <ContentControl Content="{Binding ...}"/>

    <!-- 加载覆盖层 -->
    <f:LoadingCover
        LoadingStatus="{Binding LoadingStatus}"
        RefreshCommand="{Binding LoadCommand}"/>
</Grid>
```

`LoadingCover` 三态：
- **加载中**：半透明遮罩 + 环形进度 + `Message` 文字
- **错误**：半透明遮罩 + 警告图标 + `ErrorMessage` + 刷新按钮（绑定 `RefreshCommand`）+ 复制错误按钮
- **成功**：完全隐藏

## View 绑定 — LoadingStatusIndicator

轻量级内联状态指示：

```xml
<f:LoadingStatusIndicator LoadingStatus="{Binding LoadingStatus}"/>
```
