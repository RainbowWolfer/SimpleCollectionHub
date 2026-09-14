---
name: reference-timer
description: >-
  RW 内部定时器工具类完整 API 参考和使用指南。当用户编写定时任务、周期轮询、延时执行、
  使用 CompensatingTimer/DispatcherTimerController/SystemTimerController/ThreadingTimerController
  时自动加载。
disable-model-invocation: false
---

# RW 定时器工具类参考

项目提供了 4 个定时器工具类，分布在 `RW.Common` 和 `RW.Common.WPF` 中，封装了不同底层定时器并统一了反重入和异常处理。

---

## 选择决策树

```
需要定时触发某操作？
├── 需要在 UI 线程执行（操作 WPF 控件/绑定属性）
│   └── DispatcherTimerController ✅
│
├── 需要自校准（消除累积漂移，如精确的周期采样）
│   └── CompensatingTimer ✅
│
├── 需要首次触发延迟（due time）+ 固定周期
│   └── ThreadingTimerController ✅
│
├── 简单的线程池周期执行，不需要特殊功能
│   ├── SystemTimerController（有 Start/Stop，需手动控制）
│   └── ThreadingTimerController（构造即启动，dispose 即停）
```

---

## 1. CompensatingTimer — 自校准定时器

**命名空间：** `RW.Common.Utilities`（位于 `RW.Common` 库）
**底层：** `System.Timers.Timer`（`AutoReset = false`，手动调度）
**执行线程：** 线程池

### 核心特性

- **自校准**：每次 tick 基于"预期执行时间"累加 interval，而非"实际经过时间"。这消除了累积漂移——即使单次回调执行时间有波动，长期也精确保持 interval 间隔。
- **跳过错过的时间槽**：如果回调执行太久，错过了下一次触发时间，它会快进到未来的下一个时间槽，丢弃已错过的 tick，防止连续触发导致 CPU 满载。
- **无异步支持**：回调是 `Action`（同步），不支持 `Func<Task>`。
- **异常通过 `OnTickException` 事件外抛**（不会被吞）。

### 构造函数

```csharp
public CompensatingTimer(TimeSpan interval, Action tickCallback)
```

| 参数 | 说明 |
|---|---|
| `interval` | 触发间隔，必须 > 0ms |
| `tickCallback` | 每次触发时执行的委托 |

### API

```csharp
public class CompensatingTimer : IDisposable
{
    public event Action<Exception>? OnTickException;

    public CompensatingTimer(TimeSpan interval, Action tickCallback);

    public void Start();   // 启动（已启动则忽略）
    public void Stop();    // 停止
    public void Dispose(); // 停止并释放底层 Timer
}
```

### 使用示例

```csharp
using RW.Common.Utilities;

// 每 100ms 采集一次数据，精确间隔，无累积漂移
CompensatingTimer timer = new(TimeSpan.FromMilliseconds(100), () =>
{
    double value = sensor.Read();
    buffer.Add(value);
});
timer.OnTickException += ex => Debug.WriteLine(ex);
timer.Start();

// 停止时
timer.Dispose();
```

### 适用场景

- 数据采集（需要精确采样间隔）
- 心跳检测
- 需要长时间稳定周期、不随回调执行时间偏移的定时任务

---

## 2. DispatcherTimerController — UI 线程定时器

**命名空间：** `RW.Common.WPF.Utilities.Timers`（位于 `RW.Common.WPF` 库）
**底层：** WPF `DispatcherTimer`
**执行线程：** **UI 线程**（可直接操作 WPF 控件和绑定属性）

### 核心特性

- **UI 线程执行**，可直接更新绑定的 MVVM 属性。
- 支持 **`Action`（同步）** 和 **`Func<Task>`（异步）** 两种回调。
- **防重入**：如果上一次回调仍在执行，新的 tick 会被跳过（`isTickRunning` 门控）。
- 可通过 `DispatcherPriority` 控制调度优先级。
- 异常通过 `ExceptionOccurred` 事件外抛。

### 构造函数

```csharp
// 同步回调
public DispatcherTimerController(DispatcherPriority dispatcherPriority, TimeSpan interval, Action action, Dispatcher dispatcher = null);

// 异步回调
public DispatcherTimerController(DispatcherPriority dispatcherPriority, TimeSpan interval, Func<Task> actionAsync, Dispatcher dispatcher = null);
```

| 参数 | 说明 |
|---|---|
| `dispatcherPriority` | 调度优先级，常用 `DispatcherPriority.Background` 或 `DispatcherPriority.Normal` |
| `interval` | 触发间隔 |
| `action` / `actionAsync` | 回调委托（同步或异步） |
| `dispatcher` | 可选，指定 Dispatcher；默认当前 UI 线程 |

### API

```csharp
public class DispatcherTimerController : TimerControllerBase
{
    public event TypedEventHandler<DispatcherTimerController, Exception> ExceptionOccurred;

    public virtual void Start();    // 启动
    public virtual void Stop();     // 停止
    public override void Dispose(); // 停止、取消订阅、释放
}
```

### 使用示例

```csharp
using RW.Common.WPF.Utilities.Timers;
using System.Windows.Threading;

internal class DashboardViewModel : ViewModelBaseEx
{
    private DispatcherTimerController? _refreshTimer;

    protected override void LoadedOnce()
    {
        // 每 5 秒异步刷新仪表盘数据
        _refreshTimer = new DispatcherTimerController(
            DispatcherPriority.Background,
            TimeSpan.FromSeconds(5),
            RefreshDashboardAsync
        );
        _refreshTimer.ExceptionOccurred += (s, ex) => Debug.WriteLine(ex);
        _refreshTimer.Start();
    }

    private async Task RefreshDashboardAsync()
    {
        LatestData = await dataService.FetchLatestAsync();
    }

    protected override void Unloaded()
    {
        _refreshTimer?.Dispose();
    }
}
```

### 适用场景

- UI 定时刷新（仪表盘、状态栏、时钟）
- 轮询后更新绑定属性
- 任何需要在 UI 线程执行的周期操作

---

## 3. SystemTimerController — System.Timers 定时器

**命名空间：** `RW.Common.WPF.Utilities.Timers`（位于 `RW.Common.WPF` 库）
**底层：** `System.Timers.Timer`（`AutoReset = true`，自动重复触发）
**执行线程：** 线程池

### 核心特性

- `AutoReset = true`，自动按 interval 重复触发。
- 支持 **`Action`（同步）** 和 **`Func<Task>`（异步）** 两种回调。
- **防重入**：如果上一次回调仍在执行，新的 tick 会被跳过。
- 有 `Start()` / `Stop()` 控制生命周期。
- 异常通过 `ExceptionOccurred` 事件外抛。

### 构造函数

```csharp
// 同步回调
public SystemTimerController(TimeSpan interval, Action action);

// 异步回调
public SystemTimerController(TimeSpan interval, Func<Task> actionAsync);
```

### API

```csharp
public class SystemTimerController : TimerControllerBase
{
    public event TypedEventHandler<SystemTimerController, Exception> ExceptionOccurred;

    public void Start();           // 启动
    public void Stop();            // 停止
    public override void Dispose(); // 停止并释放底层 Timer
}
```

### 使用示例

```csharp
using RW.Common.WPF.Utilities.Timers;

// 每 30 秒在后台清理过期缓存
SystemTimerController cleaner = new(TimeSpan.FromSeconds(30), () =>
{
    cache.CleanExpired();
});
cleaner.ExceptionOccurred += (s, ex) => Debug.WriteLine(ex);
cleaner.Start();

// 停止时
cleaner.Dispose();
```

### 适用场景

- 后台定时清理/维护任务
- 不需要 UI 线程的周期性工作
- 需要 `Start`/`Stop` 显式控制（与 `ThreadingTimerController` 的区别）

---

## 4. ThreadingTimerController — System.Threading 定时器

**命名空间：** `RW.Common.WPF.Utilities.Timers`（位于 `RW.Common.WPF` 库）
**底层：** `System.Threading.Timer`
**执行线程：** 线程池

### 核心特性

- 支持首次触发延迟（`due` 参数）和周期（`period` 参数）。
- 支持 **`Action`（同步）** 和 **`Func<Task>`（异步）** 两种回调。
- **防重入**：如果上一次回调仍在执行，新的 tick 会被跳过。
- **无 `Start`/`Stop`**：构造函数创建后立即开始触发，`Dispose()` 时停止。
- 异常通过 `ExceptionOccurred` 事件外抛。

### 构造函数

```csharp
// 同步回调 — 无延迟
public ThreadingTimerController(TimeSpan period, Action action);

// 同步回调 — 有延迟
public ThreadingTimerController(TimeSpan period, TimeSpan due, Action action);

// 异步回调 — 无延迟
public ThreadingTimerController(TimeSpan period, Func<Task> actionAsync);

// 异步回调 — 有延迟
public ThreadingTimerController(TimeSpan period, TimeSpan due, Func<Task> actionAsync);
```

| 参数 | 说明 |
|---|---|
| `period` | 触发周期 |
| `due` | 首次触发前的延迟（不传则 `TimeSpan.Zero`，立即执行首次） |
| `action` / `actionAsync` | 回调委托 |

### API

```csharp
public class ThreadingTimerController : TimerControllerBase
{
    public event TypedEventHandler<ThreadingTimerController, Exception> ExceptionOccurred;

    // 无 Start / Stop
    public override void Dispose(); // 释放底层 Timer
}
```

### 使用示例

```csharp
using RW.Common.WPF.Utilities.Timers;

// 延迟 10 秒后，每 1 分钟检查一次许可证状态
ThreadingTimerController licenseChecker = new(
    period: TimeSpan.FromMinutes(1),
    due: TimeSpan.FromSeconds(10),
    actionAsync: CheckLicenseAsync
);
licenseChecker.ExceptionOccurred += (s, ex) => Debug.WriteLine(ex);

// 停止时（无需 Stop，直接 Dispose）
licenseChecker.Dispose();
```

### 适用场景

- 需要"延迟启动"的周期性任务
- 不需要显式 `Start`/`Stop`，生命周期跟随对象
- 轻量级的 fire-and-forget 定时

---

## 5. 共同模式

### 防重入机制

`DispatcherTimerController`、`SystemTimerController`、`ThreadingTimerController` 都内置了 `isTickRunning` 门控：

- 每次 tick 进入时检查，如果上一次回调还未结束，**跳过本次 tick**。
- 这防止了"回调执行慢于间隔"时积压多个并发执行。

### 异常处理

三个 `TimerControllerBase` 子类统一通过 `ExceptionOccurred` 事件外抛异常，同时会调用 `Debug.WriteLine(exception)` + `Debugger.Break()`。

`CompensatingTimer` 通过 `OnTickException` 事件外抛异常（仅 `Debug.WriteLine`，无 `Debugger.Break`）。

### Dispose 模式

所有四个类都实现 `IDisposable`。`Dispose()` 会自动停止定时器并释放底层资源。三个 `TimerControllerBase` 子类会额外调用 `GC.SuppressFinalize(this)`。

---

## 6. 对照速查

| 类 | 底层 Timer | 执行线程 | 异步支持 | Start/Stop | 首次延迟 | 自校准 |
|---|---|---|---|---|---|---|
| `CompensatingTimer` | `System.Timers` | 线程池 | ❌ 仅 Action | ✅ | ❌ | ✅ |
| `DispatcherTimerController` | WPF `DispatcherTimer` | **UI 线程** | ✅ | ✅ | ❌ | ❌ |
| `SystemTimerController` | `System.Timers` | 线程池 | ✅ | ✅ | ❌ | ❌ |
| `ThreadingTimerController` | `System.Threading` | 线程池 | ✅ | ❌ 构造即启 | ✅ `due` | ❌ |
