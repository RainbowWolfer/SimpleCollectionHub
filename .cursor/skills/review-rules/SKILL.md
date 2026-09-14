---
name: review-rules
description: >-
  根据项目的所有 coding rules 检查代码合规性，列出违规项并提供修复建议。
  当用户要求检查代码是否违反 rules、做代码合规审查、或写完代码后要求自查时使用。
  触发词：review-rules、规则检查、合规审查、检查 rules、违反规则。
disable-model-invocation: true
---

# 代码规则合规审查

对照项目 `.cursor/rules/` 中的全部规则文件，逐条检查用户提供的代码，输出违规清单和修复指导。

---

## 执行流程

### Step 1 — 加载规则

读取所有 `.cursor/rules/` 下的 `.mdc` 规则文件。如果本次对话中这些规则已在上下文中，则直接引用，无需重新读取。

### Step 2 — 确认审查目标

用户通常以以下方式之一提供代码：

- 直接在消息中粘贴代码块
- 引用文件路径（`@path/to/file.cs`）
- 要求审查"刚才生成的代码"或"当前 diff"

确认目标后开始检查。如果目标不明确，追问用户。

### Step 3 — 逐规则检查

按文件逐一比对。将每个 `.mdc` 中的约束转化为可检查项，系统性地检查：

| 规则文件 | 核心检查维度 |
|----------|-------------|
| `csharp-syntax.mdc` | 大括号、`var` 禁用、null 模式匹配、字符串插值、`Debug.WriteLine(ex)`、集合表达式、`field` 关键字、namespace、JSON 库选择、await/async、接口注释 |
| `wpf-client.mdc` | MVVM 属性写法（`GetProperty`/`SetProperty`）、`AppFileHelper.PickFolder`、OneWay 绑定、`LoadingStatus` 使用、颜色 hex 写法、布局注释、XAML 头部 |
| `icon-usage.mdc` | 禁止 Windows 内置字体图标、禁止 Unicode 魔术数、必须用 `Image` 而非 `TextBlock`、必须用已有枚举 |
| `architecture.mdc` | 优先使用内部库、编译后校验、文件锁定处理 |
| `editor-conventions.mdc` | 换行符/编码（仅新建文件触发） |

### Step 4 — 输出报告

使用以下格式输出：

```markdown
## 规则合规检查报告

共发现 **N** 项违规。

| # | 严重度 | 规则文件 | 位置 | 问题 | 修复 |
|---|--------|---------|------|------|------|
| 1 | 🔴 高 | csharp-syntax | L45 | catch 中缺少 `Debug.WriteLine(ex)` | 在 `DebugLog.LogHandledException` 之前加 `Debug.WriteLine(ex);` |
| 2 | 🔴 高 | wpf-client | L55 | MVVM 属性使用了 `ref` 模式 | 改为 `GetProperty(()=>...)` / `SetProperty(()=>..., value)` |
| 3 | 🟡 中 | csharp-syntax | L38 | 日志未使用字符串插值 | 改为 `$"保存失败: {ex.Message}"` |
```

**严重度定义：**

- 🔴 **高**：编译不会报错但明显违反强制约束（如 `ref` 模式、缺少 `Debug.WriteLine(ex)`、使用被禁 API）
- 🟡 **中**：风格违规，不影响功能但不符合项目规范（如 `var`、字符串插值、null 比较）
- 🟢 **低**：代码坏味道、可优化项（如空 if 块、缺少注释、未使用集合表达式）

每个违规项都应包含**具体的修复代码**（before/after）。

### Step 5 — 询问是否修复

报告输出后，询问用户：

> 是否需要我自动修复以上违规项？

如果用户同意，逐项修复。修复过程中**严格遵守以下安全约束**：

#### 5a — 仅修改语法与格式，不得改变功能

绝大多数规则违规是纯粹的语法/格式问题（见下表），修复时**只改写法，不改行为**。这条是修复的核心铁律。

| 违规类型 | 纯格式？ | 示例 |
|---------|:--:|------|
| `var` → 显式类型 | ✅ | 不改变运行时行为 |
| `== null` → `is null` | ✅ | 语义等价 |
| `string.Format(...)` → `$""` | ✅ | 语义等价 |
| `SetProperty(ref _x, v)` → `SetProperty(() => X, v)` | ✅ | DevExpress 内部行为一致 |
| `new List<T>()` → `[]` | ✅ | 编译结果可能不同但语义等价 |
| 补充 `Debug.WriteLine(ex)` | ✅ | 仅增加副作用，不改控制流 |
| `Task.FromResult(x)` → `await Task.CompletedTask; return x` | ⚠️ | 方法签名从同步包装变为真正 async，需确认调用方不受影响 |
| `System.Text.Json` → `Newtonsoft.Json` | ⚠️ | 行为可能不同（默认值处理、命名策略等），需确认 |
| 补 `Mode=OneWay` | ✅ | WPF 绑定行为不因 OneWay/TwoWay 而改变已有的数据流方向 |

#### 5b — 有功能变更风险时必须先问

当一个修复**不是纯格式替换**时（上述标记为 ⚠️ 的项目，或任何你拿不准的改动），**不要直接修改**。先向用户说明：

- 当前写法与目标写法的差异
- 可能导致的功能变化（即使概率很低）
- 提供 2~3 个方案供选择（如"保持现状"、"改为推荐写法并补充测试"、"改为带兼容代码的折中写法"）

用户确认后再动手。

#### 5c — 修复后编译验证

所有修复完成后，如果改动涉及 C# 文件，按 `architecture.mdc` 的编译要求执行 `dotnet build` 验证。如遇到文件锁定按对应规则处理。

---

## 检查要点速查

### csharp-syntax.mdc 高频违规

- catch (Exception ex) 中有无 `Debug.WriteLine(ex)`（log/MessageBox 之前）
- 是否使用了 `var`（必须显式类型）
- null 检查是否用了 `is null`/`is not null`（禁用 `== null`）
- 字符串拼接/日志是否用了 `$""` 插值
- 是否使用了 `System.Text.Json`（应用 Newtonsoft.Json）
- 是否用 `Task.FromResult`（应用 `await Task.CompletedTask`）
- 集合初始化是否用了 `[]`（禁用 `new List<T>()`）
- 是否在代码中写了全限定名
- 接口成员是否有 `///` XML 文档注释
- 是否省略了 if/else/for 的大括号
- 文件级 namespace 是否正确

### wpf-client.mdc 高频违规

- MVVM 数据绑定属性是否用了 `GetProperty`/`SetProperty`（禁用 `ref` 和 `field`）
- `field` 关键字是否仅用于 Command（未滥用）
- 文件夹选择是否使用 `AppFileHelper.PickFolder`（禁用 WinForms 引用）
- IsReadOnly 控件是否加了 `Mode=OneWay`
- Run 绑定是否加了 `Mode=OneWay`
- XAML 布局是否有分块注释
- `d:DataContext` 和 `DataContext="{base:DISource...}"` 是否都存在
- 异步操作是否使用 `LoadingStatus` 管理状态

### icon-usage.mdc 高频违规

- 界面图标是否用 `wpf:FontIcon` + 项目自带 Segoe Fluent Icons 的码位
- 自定义 `wpf:FontIcon` 样式是否带 `BasedOn="{StaticResource FontIconBase}"`（漏了会丢 FontFamily）
- 是否引用了 `Segoe MDL2 Assets` / `Segoe UI Symbol` 等其它系统图标字体
- 位图资源键是否来自 T4 生成的 `Icons.xaml` / `IconResources.cs`（非自行编造）
- 是否手写了 `pack://application:,,,/...` 字符串而非用 `IconResources.Xxx_PNG`
- 是否尝试通过 `Foreground` 修改位图图标颜色（该改用 `wpf:FontIcon`）
