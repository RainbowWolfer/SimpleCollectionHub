---
name: initialize-new-project
description: >-
  在基于模板创建新项目后，通过现状检查 + 两轮交互（技术栈 + 业务材料）收集需求，
  最终生成一份支持分模块多轮执行的项目初始化计划（Plan）。
  本 skill 只生成 plan，不执行任何代码或配置变更；用户确认后再由后续对话分模块逐步实现。
disable-model-invocation: true
---

# 初始化新项目（生成 Plan）

本 skill 用于在基于模板创建新项目后，通过两轮交互收集需求，最终输出一份完整、可执行的项目初始化计划（plan）。

## 核心原则

**本 skill 只生成 plan，绝不执行任何代码、配置或文件变更。**
- 不写文件、不改 csproj、不加 NuGet 包
- 不修改 App.xaml、Startup.cs 等代码
- 不创建 rules / skills / 文档
- 最终交付物是一份结构化的 plan（写入 `.cursor/plans/initialize-plan.md`）

执行阶段由用户确认 plan 后，在后续对话中单独进行（可使用 Cursor 的 Plan 模式或 Agent 模式）。

## 最终目标

**本 skill 的最终目标是产出一份可执行的项目初始化计划（Plan），该计划经过后续多轮对话逐一实现后，应能得到一个完整可用、与原型图高度一致的 WPF 软件。**

这意味着 Plan 必须：

- **以原型图为唯一真相源**：Plan 中的所有 UI 描述、模块拆分、页面清单必须直接来自用户提供的设计原型，而不是猜测。
- **可迭代执行**：Plan 不是"一次性把所有代码写完"的指令，而是按模块、按页面拆分为多个独立可执行的任务块，每块可由后续对话单独实现并验证。
- **充分暴露不确定性**：原型图中不清晰的部分、技术决策中未决的部分，必须在 Plan 中明确标注「待确认」，让用户参与决策——而不是 AI 自己编造。

## 整体流程

1. **Step 0 — 项目现状检查**（检查项目已有的配置、模块、模板代码）
2. **Step 1 — 第一轮：收集技术栈需求**（AskQuestion，10 个问题）
3. **Step 2 — 生成初步计划（Plan v1）**（仅基于技术栈 + 现状检查结果）
4. **Step 3 — 第二轮：索要业务材料与设计原型**（AskQuestion + 自由对话）
5. **Step 4 — 生成完整计划（Plan v2）**（最终交付，写入文件）

---

## Step 0 — 项目现状检查

在收集用户需求之前，必须先了解项目模板的当前状态：**哪些已经配置好了、哪些可以直接用**。这个检查应在 Step 1 之前完成，检查结果会影响后续的技术栈询问策略（例如，已配置好的选项可以自动跳过或在问题中标注「已默认配置」）。

### 0.1 检查内容

| 检查项 | 检查方法 | 作用 |
|--------|---------|------|
| **解决方案结构** | 读取 `.slnx` 文件，列出所有已存在的项目 | 了解当前项目列表，后续新增模块时知道要往哪个 slnx 添加 |
| **Framework 项目** | 检查 `Source/SimpleCollectionHub.Framework/` 下的 csproj，列出已有 NuGet 包 | 了解已配置的基础设施（RW.*、FreeSql、DevExpressMvvm 等），避免重复添加 |
| **主项目** | 检查 `Source/SimpleCollectionHub/SimpleCollectionHub.csproj` 已有 NuGet 包 | 了解主项目已引用的 UI 库 |
| **Resources 项目** | 检查 `Source/SimpleCollectionHub.Resources/` | 了解已有的资源结构（图标、字体、样式） |
| **Strings 项目** | 检查 `Source/SimpleCollectionHub.Strings/` | 了解多语言机制的现状 |
| **现有模块** | 检查 `Source/Modules/` 下是否已有模块 | 了解模块化架构的已有实现 |
| **App.xaml** | 检查 `Source/SimpleCollectionHub/App.xaml` | 了解已注册的资源字典 |
| **Startup / DI 配置** | 检查 `Source/SimpleCollectionHub.Framework/` 下的依赖注入配置 | 了解已注册的服务 |
| **现有 rules** | 检查 `.cursor/rules/` 下的文件 | 了解已有规范，生成 plan 时知道哪些需要更新 |
| **现有 skills** | 检查 `.cursor/skills/` 下的文件 | 了解已有技能 |

### 0.2 将检查结果融入 Plan

在后续 Step 2 和 Step 4 生成 Plan 时：

- **标注「已有」**：所有已存在的包、配置、代码在 Plan 中明确标注为「已有」，不要出现在「需新增」清单中。
- **标注「需移除」**：如果用户选择了与已有配置冲突的选项（如选择 DevExpress 完整版时需移除 DevExpressMvvm 包），在 Plan 中列出。
- **复用已有模块**：如果 `Source/Modules/` 下已有相关模块，Plan 中应优先建议扩展而非重复创建。

---

## Step 1 — 第一轮：收集技术栈需求

使用 `AskQuestion` 工具询问用户以下问题。**必须先收集完所有回答再进入 Step 2**。

可以在一次 `AskQuestion` 调用中提交多个问题（推荐分组提交，避免一次问太多）。

### 问题 1：UI 样式库选择

- prompt: "这个项目需要使用哪种 UI 样式库？"
- options:
  - `no-ui-lib`: "不需要额外的 UI 样式库（使用默认样式）"
  - `devexpress-full`: "DevExpress WPF（完整版）- 企业级 UI 控件库"
  - `handycontrol`: "HandyControl - 现代化 WPF 控件库"
  - `material-design`: "MaterialDesignInXaml - Material Design 风格"
  - `panuon`: "Panuon.UI.Silver - 现代化 UI 库"
  - `other-ui`: "其他 UI 样式库"

### 问题 2：图标库配置

- prompt: "项目内置 Segoe Fluent Icons 字体图标（`wpf:FontIcon`）+ T4 位图资源管线（`Resources/Icons/`）。是否需要额外配置图标？"
- options:
  - `default-icons`: "使用内置图标方案（推荐）"
  - `fontawesome-add`: "额外添加 FontAwesome 图标库"
  - `material-icons-add`: "额外添加 Material Icons 图标库"
  - `other-icons`: "其他图标库"

### 问题 3：图表库选择

- prompt: "需要使用图表库吗？（模板默认不含图表库）"
- options:
  - `no-charts`: "不需要（默认）"
  - `scottplot`: "ScottPlot 5 - 高性能波形/科学绘图"
  - `devexpress-charts`: "DevExpress Charts（如果已选择 DevExpress WPF）"
  - `livecharts`: "LiveCharts - 简单易用的图表库"
  - `oxyplot`: "OxyPlot - 跨平台图表库"
  - `other-charts`: "其他图表库"

### 问题 4：数据库选择

- prompt: "项目需要使用数据库吗？（默认：FreeSql + SQLite）"
- options:
  - `default-db`: "使用默认数据库（FreeSql + SQLite）"
  - `sqlite-only`: "SQLite - 仅 SQLite，不使用 FreeSql"
  - `sql-server`: "SQL Server - 企业级数据库"
  - `postgresql`: "PostgreSQL - 开源关系数据库"
  - `mysql`: "MySQL - 流行关系数据库"
  - `redis-only`: "仅需要 Redis 缓存"
  - `other-db`: "其他数据库"

### 问题 5：ORM 选择（条件触发）

仅当问题 4 选择了数据库（除 `default-db` 和 `redis-only` 外）才询问：

- prompt: "需要使用 ORM 框架吗？（FreeSql 已作为默认）"
- options:
  - `default-orm`: "使用默认的 FreeSql"
  - `ef-core`: "Entity Framework Core - 微软官方 ORM"
  - `dapper`: "Dapper - 轻量级 ORM"
  - `no-orm`: "不需要 ORM，使用原生 SQL"
  - `other-orm`: "其他 ORM"

### 问题 6：缓存方案

- prompt: "是否需要缓存方案？"
- options:
  - `no-cache`: "不需要"
  - `memory-cache`: "内存缓存（Microsoft.Extensions.Caching.Memory）"
  - `redis`: "Redis 分布式缓存"
  - `sqlite-cache`: "SQLite 缓存"
  - `other-cache`: "其他缓存方案"

### 问题 7：日志框架

- prompt: "日志框架使用什么？（模板默认用 RW.Base.WPF 的 DebugLog，只落异常日志）"
- options:
  - `default-debuglog`: "使用默认的 DebugLog（推荐）"
  - `serilog`: "额外引入 Serilog"
  - `nlog`: "额外引入 NLog"
  - `ms-logging`: "引入 Microsoft.Extensions.Logging 抽象"

### 问题 8：网络/通信需求

- prompt: "项目需要网络通信功能吗？"
- options:
  - `no-network`: "不需要"
  - `restsharp`: "HTTP 客户端（推荐 RestSharp）"
  - `websocket`: "WebSocket 实时通信"
  - `grpc-client`: "gRPC 客户端"
  - `signalr-client`: "SignalR 客户端"
  - `other-network`: "其他通信方案"

### 问题 9：其他依赖库

- prompt: "还需要其他特定的第三方库吗？（可多选）"
- allow_multiple: true
- options:
  - `no-other`: "没有其他需求"
  - `automapper`: "AutoMapper - 对象映射"
  - `newtonsoft-json`: "Newtonsoft.Json - JSON 序列化（已有）"
  - `system-text-json`: "System.Text.Json - JSON 序列化"
  - `quartz`: "Quartz.NET - 任务调度"
  - `fluentvalidation`: "FluentValidation - 数据校验"
  - `other-lib`: "其他库（手动输入）"

### 问题 10：模块化架构

- prompt: "是否使用模块化架构？"
- options:
  - `yes-modules`: "是，使用模块化架构"
  - `no-modules`: "否，单一应用程序架构"

---

## Step 2 — 生成初步计划（Plan v1）

基于 Step 1 的回答，**仅输出**（不执行）一份「技术栈初始化计划（v1）」。内容包含：

1. **技术栈总览**：本次选定的所有技术（UI / 图标 / 图表 / 数据库 / ORM / 缓存 / 日志 / 网络 / 模块化）
2. **NuGet 包清单**：按项目分组（主项目 / Framework / Resources 等），注明包名、版本、用途
3. **配置文件变更清单**：csproj 修改、App.xaml 资源字典、Directory.Build.props、nuget.config / Nugets 本地源等
4. **代码文件变更清单**：Startup.cs 注册、MessageBoxServiceEx.cs 重写等
5. **rules / skills 变更清单**：需要更新或新增的规则与技能
6. **文档清单**：README.md、架构文档等

### 参考材料：常用 NuGet 包映射表

| 用户选择 | NuGet 包名 | 适用项目 | 说明 |
|---------|----------|---------|------|
| **DevExpress WPF（完整版）** | DevExpress.Wpf.Core | 主项目 | 核心 MVVM 框架（含 DevExpress.Mvvm） |
| | DevExpress.Wpf.Themes.Win11Light | 主项目 | Win11 浅色主题 |
| | DevExpress.Wpf.Grid | 主项目 | DataGrid/TreeList |
| | DevExpress.Wpf.LayoutControl | 主项目 | 布局控件 |
| | DevExpress.Wpf.Docking | 主项目 | 可停靠面板 |
| | DevExpress.Wpf.Ribbon | 主项目 | Ribbon 工具栏 |
| HandyControl | HandyControl | 主项目 | 现代化 WPF 控件库，需配置 App.xaml |
| MaterialDesignInXaml | MaterialDesignThemes | 主项目 | Material Design 风格 |
| Panuon.UI.Silver | Panuon.UI.Silver | 主项目 | |
| FontAwesome | FontAwesome.WPF | 主项目 | 额外添加（内置方案为 Segoe Fluent Icons + T4 位图） |
| Material Icons | MaterialDesignThemes.Mdix | 主项目 | 额外添加（内置方案为 Segoe Fluent Icons + T4 位图） |
| **RW 内部库** | RW.Common / RW.Common.WPF / RW.Base.WPF | Framework | 已包含，版本由 `RW_Common_Version` 统一控制 |
| ScottPlot | ScottPlot.WPF | Framework | 需要图表时手动添加（模板默认不含） |
| LiveCharts | LiveCharts.Wpf | 主项目 | |
| OxyPlot | OxyPlot.Wpf | 主项目 | |
| **默认数据库** | FreeSql (v3.5.309) | Framework | 已包含，核心库 |
| | FreeSql.Extensions.JsonMap (v3.5.309) | Framework | 已包含，JSON 映射 |
| | FreeSql.Provider.Sqlite (v3.5.309) | Framework | 已包含，SQLite 驱动 |
| | FreeSql.Repository (v3.5.309) | Framework | 已包含，Repository 模式 |
| SQL Server | Microsoft.Data.SqlClient | Framework | |
| PostgreSQL | Npgsql | Framework | |
| MySQL | MySql.Data | Framework | |
| Redis | StackExchange.Redis | Framework | |
| EF Core | Microsoft.EntityFrameworkCore.Sqlite | Framework | |
| Dapper | Dapper | Framework | 轻量级 ORM |
| RestSharp | RestSharp | Framework | |
| AutoMapper | AutoMapper | Framework | 对象映射（项目默认已用 Mapster） |
| Quartz.NET | Quartz | Framework | 任务调度 |
| FluentValidation | FluentValidation | Framework | 数据校验 |

### 参考材料：DevExpress WPF（完整版）配置要点

如果用户选择 DevExpress WPF（完整版），生成 plan 时需包含以下要点：

1. **添加 DevExpress NuGet 包到主项目**（`Source/SimpleCollectionHub/SimpleCollectionHub.csproj`）：

   ```xml
   <ItemGroup>
     <!-- 核心 MVVM 框架（包含 DevExpress.Mvvm） -->
     <PackageReference Include="DevExpress.Wpf.Core" Version="22.2.11" />

     <!-- 主题（Win11 浅色主题，可根据需要选择其他主题） -->
     <PackageReference Include="DevExpress.Wpf.Themes.Win11Light" Version="22.2.11" />

     <!-- 常用控件 -->
     <PackageReference Include="DevExpress.Wpf.Grid" Version="22.2.11" />
     <PackageReference Include="DevExpress.Wpf.LayoutControl" Version="22.2.11" />
     <PackageReference Include="DevExpress.Wpf.Docking" Version="22.2.11" />
     <PackageReference Include="DevExpress.Wpf.Ribbon" Version="22.2.11" />
   </ItemGroup>
   ```

   **可选的 DevExpress 包**（在 plan 中根据 UI 原型决定是否启用）：

   ```xml
   <ItemGroup>
     <!-- 图表相关 -->
     <PackageReference Include="DevExpress.Wpf.Charts" Version="22.2.11" />

     <!-- RichEdit 文档编辑 -->
     <PackageReference Include="DevExpress.Wpf.RichEdit" Version="22.2.11" />
     <PackageReference Include="DevExpress.RichEdit.Export" Version="22.2.11" />

     <!-- PDF 查看器 -->
     <PackageReference Include="DevExpress.Wpf.PdfViewer" Version="22.2.11" />

     <!-- 打印功能 -->
     <PackageReference Include="DevExpress.Wpf.Printing" Version="22.2.11" />

     <!-- 其他可选控件 -->
     <PackageReference Include="DevExpress.Wpf.NavBar" Version="22.2.11" />
     <PackageReference Include="DevExpress.Wpf.Gauges" Version="22.2.11" />
   </ItemGroup>
   ```

2. **移除 DevExpressMvvm 引用**（因为 DevExpress.Wpf.Core 已包含）：

   从 `Source/SimpleCollectionHub.Framework/SimpleCollectionHub.Framework.csproj` 中移除：

   ```xml
   <PackageReference Include="DevExpressMvvm" Version="24.1.6" />
   ```

3. **App.xaml 无需修改**：DevExpress 主题通过 NuGet 包自动加载，无需在 App.xaml 中手动添加资源字典。

4. **DevExpress 常用控件**：

   | 控件 | NuGet 包 | 用途 |
   |------|---------|------|
   | DataGrid | DevExpress.Wpf.Grid | 数据表格，支持分组、过滤、排序、虚拟化 |
   | TreeList | DevExpress.Wpf.Grid | 树形列表（与 DataGrid 同一包） |
   | LayoutControl | DevExpress.Wpf.LayoutControl | 布局容器，自动排列子元素 |
   | DockLayoutManager | DevExpress.Wpf.Docking | 可停靠面板，类似 VS 的窗口布局 |
   | RibbonControl | DevExpress.Wpf.Ribbon | 功能区工具栏，类似 Office 的 Ribbon |
   | ChartControl | DevExpress.Wpf.Charts | 图表控件 |
   | RichEditControl | DevExpress.Wpf.RichEdit | 富文本编辑器 |

5. **DevExpress 主题选择**：

   可用主题包（选择一个即可）：
   - `DevExpress.Wpf.Themes.Win11Light` - Win11 浅色主题
   - `DevExpress.Wpf.Themes.Win11Dark` - Win11 深色主题
   - `DevExpress.Wpf.Themes.Office2019White` - Office 2019 白色主题
   - `DevExpress.Wpf.Themes.Office2019Colorful` - Office 2019 彩色主题
   - `DevExpress.Wpf.Themes.VS2019Blue` - Visual Studio 2019 蓝色主题
   - `DevExpress.Wpf.Themes.VS2019Dark` - Visual Studio 2019 深色主题

   主题切换代码：

   ```csharp
   using DevExpress.Xpf.Core;

   // 在应用启动时设置主题
   ApplicationThemeHelper.UpdateApplicationThemeName("Win11.Light");
   ```

6. **plan 中应包含的 rules 更新**：
   - 创建 `.cursor/rules/devexpress-wpf.mdc`
   - 说明常用控件的使用方法（DataGrid、TreeList、LayoutControl、Docking、Ribbon 等）
   - 说明主题选择方式
   - 更新 `.cursor/rules/architecture.mdc`，在"核心技术组件"部分添加 DevExpress WPF 说明

### 参考材料：HandyControl 配置要点

如果用户选择 HandyControl，生成 plan 时需包含以下要点：

1. **添加 HandyControl NuGet 包**（`Source/SimpleCollectionHub/SimpleCollectionHub.csproj`）：

   ```xml
   <ItemGroup>
     <PackageReference Include="HandyControl" />
   </ItemGroup>
   ```

2. **在 App.xaml 中添加 HandyControl 资源字典**（`Source/SimpleCollectionHub/App.xaml`）：

   ```xml
   <Application.Resources>
       <ResourceDictionary>
           <ResourceDictionary.MergedDictionaries>
               <ResourceDictionary Source="pack://application:,,,/HandyControl;component/Themes/SkinDefault.xaml"/>
               <ResourceDictionary Source="pack://application:,,,/HandyControl;component/Themes/Theme.xaml"/>
           </ResourceDictionary.MergedDictionaries>
       </ResourceDictionary>
   </Application.Resources>
   ```

3. **在需要使用 HandyControl 控件的 XAML 文件中添加命名空间**：

   ```xml
   xmlns:hc="https://handyorg.github.io/handycontrol"
   ```

4. **HandyControl 常用控件示例**：

   | 控件 | XAML 标签 | 说明 |
   |------|----------|------|
   | Button | `<hc:Button>` | 带动画效果的按钮 |
   | TextBox | `<hc:TextBox>` | 带水印效果的文本框 |
   | NumericUpDown | `<hc:NumericUpDown>` | 数值输入框 |
   | PasswordBox | `<hc:PasswordBox>` | 密码输入框 |
   | ComboBox | `<hc:ComboBox>` | 下拉选择框 |
   | CheckBox | `<hc:CheckBox>` | 复选框 |
   | RadioButton | `<hc:RadioButton>` | 单选按钮 |
   | DatePicker | `<hc:DatePicker>` | 日期选择器 |
   | TimePicker | `<hc:TimePicker>` | 时间选择器 |
   | DateTimePicker | `<hc:DateTimePicker>` | 日期时间选择器 |
   | ToggleSwitch | `<hc:ToggleSwitch>` | 开关控件 |
   | ProgressBar | `<hc:ProgressBar>` | 进度条 |
   | Slider | `<hc:Slider>` | 滑块 |
   | ToggleButton | `<hc:ToggleButton>` | 切换按钮 |
   | ProgressButton | `<hc:ProgressButton>` | 带进度按钮 |
   | GroupBox | `<hc:GroupBox>` | 分组框 |
   | Card | `<hc:Card>` | 卡片容器 |
   | Divider | `<hc:Divider>` | 分隔线 |
   | ScrollViewer | `<hc:ScrollViewer>` | 滚动视图 |
   | MessageBox | `HandyControl.Controls.MessageBox` | 消息对话框 |

5. **HandyControl 消息框集成**（重写 `MessageBoxServiceEx`）：

   修改 `Source/SimpleCollectionHub.Framework/ViewModelServices/MessageBoxServiceEx.cs`，把内部的系统默认 MessageBox 替换为 HandyControl 的实现：

   ```csharp
   using HandyControl.Controls;

   internal class MessageBoxServiceEx : IMessageBoxService
   {
       public void ShowInfo(string message)
       {
           MessageBox.Show(message, "信息", MessageBoxButton.OK, MessageBoxImage.Information);
       }

       public bool ShowYesNoQuestion(string message)
       {
           var result = MessageBox.Show(message, "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
           return result == MessageBoxResult.Yes;
       }

       // 其他方法实现...
   }
   ```

   **在 View 中注册**（如果引用了 UserControlStyle 或 DialogStyle.Base 则不需要再额外定义）：

   ```xml
   <dxmvvm:Interaction.Behaviors>
       <f:MessageBoxServiceEx/>
   </dxmvvm:Interaction.Behaviors>
   ```

   **在 ViewModel 中使用**：

   ```csharp
   public IMessageBoxServiceEx MessageBoxService => GetService<IMessageBoxServiceEx>();
   ```

6. **HandyControl 主题切换**：

   HandyControl 支持多种主题，通过 App.xaml 中的资源字典控制：
   - `SkinDefault.xaml` + `Theme.xaml`：默认主题
   - `SkinViolet.xaml` + `Theme.xaml`：紫色主题
   - `SkinDark.xaml` + `Theme.xaml`：深色主题

   代码切换主题：

   ```csharp
   using HandyControl.Tools;

   // 切换主题
   HandyControl.Tools.ControlHelper.SetTheme(App.Current, "主题名称");
   ```

7. **plan 中应包含的 rules 更新**：
   - 创建 `.cursor/rules/handycontrol-usage.mdc`
   - 说明常用控件的使用方法
   - 说明主题切换方式
   - 提供常见场景的代码示例
   - 更新 `.cursor/rules/architecture.mdc`，在"核心技术组件"部分添加 HandyControl 说明

### 参考材料：默认数据库 / 图表说明

- **FreeSql + SQLite**：项目默认已配置，支持 Code First、自动同步表结构、Repository 模式。详见 `.cursor/rules/architecture.mdc`。
- **图表**：模板不含图表库。若业务确实需要，在 Plan 中单列一项并说明选型理由，经用户确认后再添加。

---

## Step 3 — 第二轮：索要业务材料与设计原型

向用户说明：**仅凭技术栈选择无法生成贴合业务的完整计划**，需要进一步资料才能产出可执行的最终 plan。

### 3.1 通过 AskQuestion 收集结构化信息

- prompt: "为了生成贴合业务的完整计划，请提供以下信息（可多选你需要提供的材料类型）"
- allow_multiple: true
- options:
  - `ui-mockups`: "我会提供 UI 设计原型图 / 效果图（Figma、Axure、PNG 等）"
  - `requirements-doc`: "我会提供业务需求文档 / 功能清单"
  - `data-model`: "我会提供数据模型 / 数据库设计"
  - `flow-diagrams`: "我会提供业务流程图 / 状态机图"
  - `hardware-interaction`: "涉及硬件读写操作（传感器、PLC、串口、GPIB、USB 设备等）"
  - `reference-products`: "我有参考产品 / 竞品链接"
  - `none-now`: "暂无额外材料，按通用方案生成"

### 3.2 通过自由对话收集详细材料

向用户**明确请求**以下材料（根据用户在 3.1 的选择，引导用户提供）：

1. **UI 设计原型图**：
   - 直接粘贴图片到对话，或提供图片路径
   - 说明：主窗口布局、导航结构、关键页面、配色风格、控件偏好
   - 如果使用 DevExpress/HandyControl，原型图能帮助确定具体用哪些控件

2. **业务需求文档**：
   - 核心功能列表（用一句话描述每个功能）
   - 用户角色与权限（如果有）
   - 关键业务流程（从用户操作角度描述）

3. **数据模型**：
   - 主要实体及其关系
   - 已有的数据库表结构（如果有）
   - 数据规模估算（影响选型：SQLite vs SQL Server）

4. **参考产品 / 竞品**：
   - 链接或截图
   - 希望借鉴的部分（布局、交互、功能）

5. **特殊要求**：
   - 性能要求（实时性、并发、数据量）
   - 安全 / 合规要求（数据加密、审计、合规标准）
   - 部署方式（单机、ClickOnce、内网分发）
   - 多语言需求（项目已内置 Strings 多语言机制）

6. **硬件读写需求**（如果用户在 Step 3.1 选择了 `hardware-interaction`）：
   - 涉及的硬件类型与型号（传感器、PLC、串口设备、GPIB 仪器、USB 设备等）
   - 通信协议（Modbus、TCP/IP、RS-232/485、GPIB、自定义协议等）
   - 数据采集频率与实时性要求（毫秒级、秒级、分钟级）
   - 硬件数量、并发连接要求
   - 是否有已有的硬件 SDK / DLL / 驱动
   - **关键决策：初期是否使用 Mock 虚拟设备开发**（详见下文）

   ### 硬件 Mock 策略（必须询问用户）
   
   当用户提供了涉及硬件读写的原型图、需求书等材料后，**必须在生成 Plan 之前向用户确认**：
   
   > "你提供的材料中涉及 [XX 硬件] 的读写操作。在项目初期构建阶段，建议先使用 **Mock 虚拟设备** 代替真实硬件进行开发和调试——这样可以脱离硬件环境独立开发、快速验证 UI 和业务流程。后续再替换为真实设备的实现。
   > 
   > 这套方案通过 **接口 + 实现类** 的架构设计：所有业务代码只依赖接口，Mock 实现和真实硬件实现各自实现同一接口。后续替换时**只需替换实现类**，引用接口的地方完全不用动。"
   
   然后使用 `AskQuestion` 让用户确认：
   
   - prompt: "硬件读写策略：初期是否使用 Mock 虚拟设备？"
   - options:
     - `mock-first`: "是，先用 Mock 虚拟设备开发，后续替换为真实硬件（推荐）"
     - `real-hardware`: "直接使用真实硬件开发"
   
   无论用户选择哪种策略，Plan 中都必须包含：
   
   - **硬件接口定义**：为每种硬件类型定义清晰的 C# 接口（`IHardwareXxx`），包含所有读写操作方法
   - **接口与实现分离**：接口放在 Framework 项目，实现类放在独立项目或模块中
   - **DI 注册**：通过 Autofac 注册接口与实现类的绑定，替换时只需改 DI 注册，无需改业务代码
   - **如果选择 mock-first**：Plan 中包含 Mock 实现类的创建任务（模拟数据返回、模拟设备状态、模拟异常场景）
   - **如果选择 real-hardware**：Plan 中包含真实硬件 SDK 集成、驱动安装、连接测试等任务
   
   ### 硬件相关 rules / skills 生成
   
   Plan 中必须包含根据实际硬件需求生成对应的 **skill** 或 **rule** 的任务：
   
   - 如果硬件有专用的通信协议（如 Modbus），在 Plan 的「rules 与 skills 更新」章节中增加创建对应 skill 的任务
   - 如果涉及通用串口通信，创建 `reference-serial-port` skill
   - 如果涉及 GPIB 仪器控制，创建 `reference-gpib` skill
   - 如果硬件有官方 SDK，创建该 SDK 的使用规范 rule
   
   Plan 应明确列出需要创建哪些 skill/rule，并在「任务执行顺序」中安排对应的轮次。

**重要：等待用户提供实际材料后再进入 Step 4。** 如果用户选择 `none-now`，可以直接进入 Step 4 基于通用方案生成 plan，但要在 plan 中明确标注「待补充业务材料」的部分。

---

## Step 4 — 生成完整计划（Plan v2 - 最终交付）

综合 Step 1 技术栈 + Step 3 业务材料，生成最终的、可执行的完整项目初始化计划。

### 4.1 Plan 必须包含的章节

最终的 plan 文件应包含以下结构（写入 `.cursor/plans/initialize-plan.md`）：

```markdown
# 项目初始化计划

## 1. 项目概述
- 项目名称、目标、核心业务（基于 Step 3 业务材料总结）
- 技术栈总览表

## 2. 环境与依赖（NuGet 包）
- 按项目分组列出所有需要添加的包（包名、版本、用途）
- 标注哪些是「已包含」、哪些是「需新增」、哪些是「需移除」

## 3. 配置文件变更
- csproj 修改清单
- App.xaml 资源字典（如适用）
- Directory.Build.props（含 `RW_Common_Version`）
- nuget.config / Nugets 本地源 / 其他配置

## 4. 项目结构与模块划分
- 基于业务需求建议的模块划分（Modules）
- 每个模块的职责
- 主要 View / ViewModel 清单（基于 UI 原型图）

## 5. 基础设施层
- 数据库初始化（FreeSql 配置、连接串、Code First 同步策略）
- 日志配置
- 缓存方案
- DI 容器注册（Autofac + RW.Base.WPF 的 `IoCInitializer` / `ISingletonDependency` 约定）

## 6. 框架层
- 需要新增/修改的容器服务（ISingletonDependency）
- 需要新增/修改的 ViewModel 服务（ServiceBase）
- 通用基础设施（消息框、对话框、加载指示器）

## 7. 硬件抽象层（如涉及硬件读写）

> **重要**：如果项目涉及硬件读写，必须先在 Step 3.2 向用户确认 Mock 策略后再填充本章节。

### 7.1 硬件接口定义

为每种硬件类型定义 C# 接口，所有业务代码通过接口访问硬件，不直接依赖具体实现类：

| 接口 | 对应硬件 | 核心方法 | 放置位置 |
|------|---------|---------|---------|
| `ISensorReader` | 温度传感器 | `ReadValueAsync()`, `CalibrateAsync()` | Framework/Interfaces/ |
| `IPlcController` | PLC 控制器 | `ConnectAsync()`, `SendCommandAsync()` | Framework/Interfaces/ |
| `ISerialPortDevice` | 串口设备 | `OpenAsync()`, `SendAsync()`, `ReceiveAsync()` | Framework/Interfaces/ |

**接口设计原则**：
- 接口只暴露硬件能力，不暴露通信细节（波特率、串口号等配置参数通过构造函数或初始化方法传入）
- 所有 I/O 操作使用 `async Task<T>`，支持取消（`CancellationToken`）
- 包含 `ConnectAsync()` / `DisconnectAsync()` 生命周期方法
- 包含健康检查方法（`IsConnected` 属性或 `PingAsync()`）
- 接口放在 Framework 项目中，便于所有模块引用（项目里没有 Shared 程序集）

### 7.2 Mock 实现类（初期开发用）

如果用户选择 `mock-first` 策略：

| Mock 类 | 实现接口 | 模拟策略 |
|---------|---------|---------|
| `MockSensorReader` | `ISensorReader` | 返回预设的模拟数据，可配置波动范围和异常概率 |
| `MockPlcController` | `IPlcController` | 内存状态机模拟，可预设响应序列 |
| `MockSerialPortDevice` | `ISerialPortDevice` | 内存队列模拟收发，支持注入测试数据 |

Mock 实现类放在 `Source/SimpleCollectionHub.Framework/Mocks/` 目录下。

### 7.3 真实硬件实现类（后续替换）

| 实现类 | 实现接口 | 依赖 |
|--------|---------|------|
| `RealSensorReader` | `ISensorReader` | 硬件 SDK/DLL |
| `RealPlcController` | `IPlcController` | PLC SDK |
| `RealSerialPortDevice` | `ISerialPortDevice` | `System.IO.Ports` |

真实实现类放在 `Source/SimpleCollectionHub.Framework/Hardware/` 或独立项目中。

### 7.4 DI 注册策略

通过 Autofac 的条件注册，替换时只需改一处：

```csharp
// 开发阶段：注册 Mock 实现
builder.RegisterType<MockSensorReader>().As<ISensorReader>().SingleInstance();

// 生产阶段：替换为真实实现（只需改这一行，其余代码不动）
// builder.RegisterType<RealSensorReader>().As<ISensorReader>().SingleInstance();
```

推荐使用配置文件或编译条件（`#if DEBUG`）控制注册，避免手动改代码。

### 7.5 替换保障

- **业务代码只引用接口**：所有 ViewModel、Service 中只声明 `ISensorReader`，不引用任何 `MockXxx` 或 `RealXxx`
- **替换清单**：Plan 中明确列出替换时需修改的文件（通常只有 DI 注册文件 1 个）
- **测试覆盖**：Mock 类和真实实现类共用同一套接口测试，确保行为一致

## 8. UI 层（基于设计原型）
- 主窗口布局方案
- 导航结构
- 关键页面拆分（对照原型图逐页说明）
- 主题/样式策略

## 9. 数据模型与实体
- 主要实体类设计
- 实体关系
- FreeSql 映射约定

## 10. 业务流程实现路径
- 按业务流程列出实现步骤
- 每个步骤涉及的 View / ViewModel / Service

## 11. rules 与 skills 更新
- 需要更新的现有 rules（architecture.mdc、wpf-core.mdc、wpf-xaml.mdc 等）
- 需要新增的 rules（如 devexpress-wpf.mdc、handycontrol-usage.mdc、硬件相关 skill/rule）
- 需要新增的 skills

## 12. 文档
- README.md 更新内容
- 架构文档
- 开发指南

## 13. 任务执行顺序（分模块、多轮执行）

**核心原则：Plan 是给多轮对话逐步执行的，不允许一次性写完所有代码。** 任务拆分必须满足：

- **按模块拆分**：每个功能模块（或页面）是一个独立的执行单元，一轮对话只负责一个模块。
- **按层级拆分**：基础设施层 → 框架层 → 业务模块层，先完成底层再建上层。
- **每轮有明确的验收标准**：每轮结束必须能编译通过并对应当前阶段的功能。

### 推荐的分轮执行方案

| 轮次 | 范围 | 说明 |
|------|------|------|
| **Round 1** | 基础设施配置 | NuGet 包安装、slnx 更新、App.xaml 资源字典、DI 注册基建设置 |
| **Round 2** | 框架层完善 | Model/Entity 实体类、FreeSql CodeFirst 配置、通用服务（消息框等） |
| **Round 3-N** | 功能模块逐一实现 | 每个模块包含：Views + ViewModels + Services，一模块一轮 |
| **Round N+1** | 主窗口整合 | 组装各模块到主窗口（Shell），配置导航、菜单结构 |
| **Round 最后** | 验收与修复 | 完整编译 + 启动验证 + 原型图对比修复 |

### 任务标注格式

每个任务标注：
- 优先级（P0 必须 / P1 重要 / P2 可选）
- 所属轮次（Round X）
- 依赖关系（任务 B 依赖任务 A）
- 预计工作量（小 / 中 / 大）
- 验证标准（每个任务如何确认完成）

### 示例

```markdown
### Round 1: 基础设施配置

- [P0] 安装 NuGet 包 → 验证：dotnet restore 无报错
- [P0] 更新 slnx 添加新模块项目 → 验证：dotnet build 无报错
- [P1] 配置 App.xaml 资源字典 → 验证：启动后主题生效

### Round 3: 报警日志模块

- [P0] 创建 AlarmLogMainView.xaml + AlarmLogMainViewModel → 验证：页面可导航到
- [P0] 实现报警数据查询 Service → 验证：Service 返回测试数据
- [P1] 集成图表库显示报警趋势 → 验证：图表正确渲染
```

## 14. 验收标准
- 编译通过：dotnet build 无错误
- 启动正常：应用可正常启动并显示主窗口
- 基础流程跑通：（根据业务材料定义的最小可用流程）

## 15. 待补充 / 待确认事项
- 列出所有未确定的决策点（用户在 Step 3 未提供的部分）
- 列出需要用户进一步澄清的问题

## 16. 执行阶段代码质量准则

以下准则应作为 Plan 的一部分写入，指导后续执行对话中 AI 生成代码的质量。这些不是 Plan 生成阶段需要执行的，而是 Plan 中为执行阶段预设的约束。

### 16.1 csproj 与 slnx 管理

- 每新建一个 `.csproj` 项目，必须同时将其添加到解决方案文件（`.slnx`）中。
- Plan 中应明确列出每个新建项目对应的 slnx 节位置。

### 16.2 XAML 命名空间规范

AI 生成 View.xaml 时常见的错误是使用错误的命名空间前缀（如 `local:XXXViewModel`）。Plan 中应规定正确的 XAML 命名空间用法：

- **不要在 XAML 中使用 `d:DataContext` 或错误的 `local:` 前缀指定 ViewModel 类型。**
- 正确做法是使用模块专用的命名空间前缀 + `base:DISource`：

```xml
<!-- 正确示例：使用 vms 命名空间 + DI Source -->
<UserControl xmlns:vms="clr-namespace:CFBG_HostSoftware.AlarmLog.ViewModels"
             DataContext="{base:DISource Type=vms:AlarmLogMainViewModel}">
```

- Plan 中应明确每个模块 View 对应的 ViewModel 命名空间和类型，作为执行阶段的参照。

### 16.3 自定义 Window 的默认行为

- 优先使用库自带或 WPF 自带的 `Window` 作为窗口基类。
- 如果确实需要自定义 `Window` 样式，必须保留标准 Windows 窗口行为：标题栏拖拽移动、双击最大化/还原、系统菜单（右键标题栏）、窗口边框调整大小等。
- 推荐：直接使用 WPF 原生 `Window` + 样式模板，而非完全重写窗口行为。

### 16.4 图表控件

- **模板默认不含任何图表库**，需要图表时才在 Plan 中列出并经用户确认后添加。
- 一旦引入（如 ScottPlot 5），不要让业务模块直接依赖第三方控件类型。
  应在 Framework 项目中包一层，业务模块只用这个包装类：

```csharp
// Source/SimpleCollectionHub.Framework/Controls/PlotControlEx.cs
using ScottPlot.WPF;

namespace SimpleCollectionHub.Framework.Controls;

/// <summary>
/// 图表控件包装类，隔离第三方图表库，便于统一样式与后续替换。
/// </summary>
public class PlotControlEx : WpfPlot
{
    // 可扩展自定义绘图逻辑
}
```

- 所有业务模块中的图表使用 `PlotControlEx`，不直接使用第三方原生控件。
```

### 4.2 生成 Plan 的注意事项

1. **基于实际材料**：UI 部分、模块划分、数据模型必须基于 Step 3 收到的材料，不要凭空编造业务细节。
2. **基于现状检查**：利用 Step 0 的检查结果，标注已有项、避免重复添加、优先复用已有模块和配置。
3. **标注不确定性**：对于材料不足的部分，在「待补充事项」中明确列出，不要假装知道。
4. **保持可执行性**：每个任务都要具体到「改哪个文件 / 加什么代码」，让用户（或后续 AI 对话）可以直接照做。
5. **遵循项目约定**：所有方案必须符合 `.cursor/rules/` 下的现有规范（MVVM 模式、DI 约定、命名空间约定等）。
6. **分模块可迭代**：任务拆分必须支持多轮对话逐步执行（详见「16. 执行阶段代码质量准则」），不允许设计为一次性的全量代码生成。
7. **写文件**：最终 plan 写入 `.cursor/plans/initialize-plan.md`（如果目录不存在，在 plan 中说明需要先创建）。
8. **输出摘要**：在对话中输出 plan 的摘要 + 文件路径，提醒用户确认后再执行。

---

## 检查清单

- [ ] Step 0：已完成项目现状检查（slnx、csproj、现有模块、App.xaml、rules、skills 等）
- [ ] Step 0：检查结果已整理，作为 Plan 中「已有」标注的依据
- [ ] Step 1：已通过 AskQuestion 收集完所有技术栈问题（10 个问题）
- [ ] Step 2：已生成并展示 Plan v1（技术栈 + 现状检查结果）
- [ ] Step 3：已通过 AskQuestion 询问材料类型，并请求用户实际提供
- [ ] Step 3：如涉及硬件读写（`hardware-interaction`），已向用户确认 Mock 策略（mock-first / real-hardware）
- [ ] Step 3：已等待用户材料（或用户明确选择「暂无」）
- [ ] Step 4：已综合所有材料生成完整 plan
- [ ] Step 4：plan 包含「分模块多轮执行」方案，任务按模块拆分、可逐轮执行
- [ ] Step 4：plan 包含「执行阶段代码质量准则」（XAML 命名空间、Window 行为、PlotControl 使用、slnx 更新）
- [ ] Step 4：如涉及硬件，plan 包含硬件抽象层（接口定义、Mock/真实实现、DI 注册策略、替换保障）及硬件相关 skill/rule 创建任务
- [ ] Step 4：plan 已写入 `.cursor/plans/initialize-plan.md`
- [ ] Step 4：已在对话中输出摘要并提醒用户确认
- [ ] 全程未执行任何实际代码 / 配置 / 文件变更

## 重要约束

1. **本 skill 是 Plan 生成器，不是执行器**：在整个过程中，禁止修改任何源码、配置、csproj、rules、skills。
2. **三步流程缺一不可**：Step 0 现状检查 → Step 1-2 技术栈 → Step 3-4 业务材料，不能跳过任何步骤。
3. **Plan 必须支持分模块多轮执行**：不允许生成"一次性全量代码写入"式的 plan。任务必须按模块/页面拆分，每轮可独立验证。
4. **不要替用户做业务决策**：如果材料不足，明确列出「待补充事项」，不要编造业务流程或数据模型。
5. **保持中立**：推荐技术方案时给出理由和备选，不要绝对化。
6. **最终交付物只有一份 plan 文件**：除了 `.cursor/plans/initialize-plan.md` 外，不应创建任何其他文件。
