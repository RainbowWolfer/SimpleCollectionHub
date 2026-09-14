# 项目初始化计划

> 本文件只指导后续多轮实现，**确认前不要改源码 / csproj / rules**。  
> UI 唯一真相源：`Files/ui-prototype-4/`（`index.html` + `styles.css` + `app.js`）。  
> 产品边界来自本对话已确认规则，不是猜测。

---

## 1. 项目概述

### 1.1 产品

**SimpleCollectionHub** 是给自己用的 Windows 桌面藏品库：磁盘上的游戏 / 音乐 / 视频目录仍在原处，软件只在本机库里给每条路径贴一层封面、截图、文字和链接。主界面用可嵌套分组 + 卡片墙把它们亮出来，不必打开原文件就能看到截图和源地址。

**硬约束（已确认，执行阶段不得违反）：**

1. 元数据**只**存在软件 LocalAppData 库（SQLite + 应用数据目录）。  
2. **禁止**在目标路径（「原目录」）新建、修改、删除任何文件，包括 sidecar。  
3. 「从库中移除」只删库记录，不动磁盘。  
4. 标签**只用于筛选**，不进入分组树、不改变树的结构和计数。  
5. 搜索、标签、完整度、组内排序只影响中间列表；树的展开、嵌套、计数始终是全库。

**明确不做（用户已拒绝，不要在执行轮次里加回来）：**

- 启动游戏 / 用默认程序打开文件（「打开内容」）
- 扫描原目录自动当封面
- 路径丢失后的「重新指定」流程
- 从资源管理器拖放绑定、卡片拖到分组
- 方向键 / Enter / Delete 键盘操作（搜索框输入除外）
- 原目录 sidecar、换机同步、Steam 等网络功能
- 图表库、第三方皮肤、额外图标库、独立模块 DLL

### 1.2 技术栈总览

| 项 | 选择 | 状态 |
|---|---|---|
| 运行时 | .NET 10.0-windows、WPF、C# 14 | 已有 |
| 应用框架 | RW.Base.WPF（`ApplicationBase` + Autofac + 生命周期） | 已有 |
| MVVM | DevExpressMvvm 24.1.6（免费版） | 已有 |
| UI | 原生 WPF + `RW.Common.WPF`，不引入皮肤库 | 已有 |
| 图标 | Segoe Fluent Icons（`wpf:FontIcon`）+ T4 位图 | 已有 |
| 图表 | 不需要 | — |
| 数据库 | FreeSql 3.5.309 + SQLite | 包已有，**运行时尚未接线** |
| 缓存 | 不需要 | — |
| 日志 | `DebugLog` | 已有 |
| 网络 | 不需要 | — |
| 映射 / JSON | Mapster、Newtonsoft.Json | 已有（经 RW.Base.WPF） |
| 架构 | 单一应用程序：业务在主项目，共享能力在 Framework | 已确认 |

### 1.3 现状（Step 0）

| 位置 | 现状 |
|---|---|
| `SimpleCollectionHub.slnx` | 仅 4 个项目：主程序 / Framework / Resources / Strings。无 `Source/Modules` |
| 主窗口 | 仍是模板 HELLO WORLD + About 对话框 |
| Framework | `IAppModule`、`DialogService`、`LoadingStatus`、`EntityRepository`、`AppFolderConfig.AppDatabaseFilePath` 已有 |
| `IFreeSql` | **未注册**，库从未真正建起来 |
| 原型 | `Files/ui-prototype-1`～`4` 仅作设计参考，不要把 JS 搬进 WPF |

---

## 2. 环境与依赖（NuGet 包）

**需新增：无。需移除：无。**

已包含（`SimpleCollectionHub.Framework.csproj`）：

| 包 | 版本 | 用途 |
|---|---|---|
| RW.Common / RW.Common.WPF / RW.Base.WPF | `$(RW_Common_Version)` = 1.0.54 | 基础库、WPF 控件、应用框架 |
| DevExpressMvvm | 24.1.6 | ViewModel、命令、ViewModel 服务 |
| FolderBrowserEx | 1.0.1 | 已被 `AppFileHelper.PickFolder` 使用 |
| FreeSql / JsonMap / Provider.Sqlite / Repository | 3.5.309 | 本机库 |

主项目只引用 Framework。本地源：根目录 `nuget.config` → `./Nugets`。

---

## 3. 配置文件变更

| 文件 | 动作 |
|---|---|
| `SimpleCollectionHub.slnx` | **不新增项目**（已选非模块化） |
| `*.csproj` | 不改包引用 |
| `App.xaml` | 继续只合并 `FrameworkTheme.xaml` |
| `Directory.Build.props` | 本阶段不改 `RW_Common_Version` |
| `AppFolderConfig` | 已有 `AppDatabaseFilePath`。**新增**媒体目录属性（见 5.2），不要写到原条目路径下 |

---

## 4. 项目结构与模块划分

不建 `Source/Modules/*`。业务全部放主项目，可复用的服务 / 实体 / 控件放 Framework。

```
Source/SimpleCollectionHub/
├── Views/
│   ├── MainWindow.xaml                 # 三栏壳：侧栏 + 库 + 详情
│   ├── CollectionSidebarView.xaml      # 筛选 + 树 + 页脚
│   ├── CollectionLibraryView.xaml      # 工具栏 + 卡片/列表
│   ├── CollectionDetailView.xaml       # 右侧详情（可折叠）
│   └── Dialogs/
│       ├── AboutDialog.xaml            # 已有，改成产品说明（对应原型「原型说明」）
│       ├── AddItemDialog.xaml
│       ├── AddGroupDialog.xaml
│       └── GroupIconDialog.xaml
├── ViewModels/                         # 上述页面对应 XxxViewModel（不要写成 XxxViewViewModel）
└── （现有 App / Splash 保留）

Source/SimpleCollectionHub.Framework/
├── Database/                           # 扩展：FreeSql 启动、实体
├── Services/                           # 藏品库、路径探测、媒体副本
├── Controls/                           # 若需要 Toast、Fluent 图标选择网格
└── Configs/AppFolderConfig.cs          # 增加媒体目录
```

XAML 命名空间（主项目 View，执行时从 `Source/XAML_Preset.txt` 复制 xmlns）：

```xml
xmlns:vms="clr-namespace:SimpleCollectionHub.ViewModels"
DataContext="{base:DISource Type=vms:CollectionSidebarViewModel}"
```

不要用错误的 `local:XxxViewModel` 或乱写 `d:DataContext` 类型。

### 4.1 主要 View / ViewModel 清单（对照原型 4）

| 原型区域 | View | ViewModel |
|---|---|---|
| 窗口整体（原生标题栏，不要仿 HTML 假标题栏） | `MainWindow` | `MainWindowViewModel` |
| 左侧「筛选」+ 树 + 页脚 | `CollectionSidebarView` | `CollectionSidebarViewModel` |
| 中间面包屑、标题、完整度、排序、视图切换、卡片墙 | `CollectionLibraryView` | `CollectionLibraryViewModel` |
| 右侧详情 | `CollectionDetailView` | `CollectionDetailViewModel` |
| 添加条目 | `AddItemDialog` | `AddItemDialogViewModel` |
| 新建分组 | `AddGroupDialog` | `AddGroupDialogViewModel` |
| 设置图标 | `GroupIconDialog` | `GroupIconDialogViewModel` |
| 说明 | `AboutDialog` | 现有对话框 VM，改文案 |

---

## 5. 基础设施层

### 5.1 数据库

新建 `DatabaseService`（`ISingletonDependency` + `IAppInitializeAsync`）：

- 连接串：`Data Source={AppFolderConfig.AppDatabaseFilePath}`
- `UseAutoSyncStructure(true)`（Code First）
- 注册 `IFreeSql` 单例
- 启动说明：`IAppInitializeAsync.Description = "初始化收藏库"`
- **禁止**把数据库文件放到条目的原路径

### 5.2 媒体目录

在 `AppFolderConfig` 增加例如 `MediaFolder` = `Path.Combine(DataFolder, "Media")`，初始化时创建。条目图片**复制**到此目录，库里只记相对库根的路径。原图所在磁盘位置只作导入来源，导入后不再依赖。

> 推荐方案。若用户希望「只记原图路径、不拷贝」，见第 15 节。

### 5.3 日志 / 缓存 / DI

- 日志：继续 `Debug.WriteLine(ex)` + `DebugLog.LogHandledException`，不引入 NLog / Serilog。
- 缓存：不引入。
- DI：容器服务用 `ISingletonDependency` 构造函数注入；`TypedUIObjectService` 等必须 XAML `Interaction.Behaviors` + `GetService`。`IFreeSql` 若无法靠标记接口发现，在 `Startup.Initialize` 或 `DatabaseService` 里 `builder.RegisterInstance`。

---

## 6. 框架层

| 服务 | 类型 | 职责 |
|---|---|---|
| `ILibraryService` | 容器单例 | 分组 / 条目 / 字段 / 标签的读写 |
| `IPathProbeService` | 容器单例 | **只读**探测路径是否存在（文件或目录）。禁止任何写入 |
| `IMediaStore` | 容器单例 | 把用户选择的图片复制进 `MediaFolder`，返回库内相对路径；删除字段时回收未引用文件（可第二轮再做） |
| `ICollectionWorkspace` | 容器单例 | 当前选中分组/条目、搜索词、标签筛选、完整度、排序、卡片/列表。侧栏/库/详情都绑它 |
| `IFluentIconCatalog` | 容器单例 | 原型 4 那份精选 Segoe Fluent Icons 码位 + 中文名，供图标对话框搜索 |

沿用已有：`MessageBoxServiceEx`、`DialogService`、`LoadingStatus`、`AppFileHelper.PickFolder`、`OpenFileDialogService`（选图片，禁止 `Microsoft.Win32.OpenFileDialog`）。

轻量 Toast：主窗口内一个绑定 `ICollectionWorkspace` 或事件的提示条，对应原型 toast；错误仍用 MessageBox。不要新 NuGet。

---

## 7. 硬件抽象层

不涉及硬件。本章节不适用。

---

## 8. UI 层（基于原型 4）

配色沿用 `CustomColors.xaml` / 原型 `--accent: #326cf3`。界面图标一律 `wpf:FontIcon`。

**窗口：用 WPF 原生 `Window`**（可拖、双击最大化、系统菜单、边框缩放）。HTML 原型的假标题栏是网页演示，不要做成无边框自绘窗，除非第 15 节改口。

### 8.1 主窗口布局

```
[ 原生标题栏 ]
[ 筛选（搜索+标签） ]
[ 分组树          ] [ 面包屑 / 标题 / 工具 ] [ 详情（可隐藏） ]
[ 页脚说明        ] [ 卡片或列表           ] [                ]
```

侧栏宽度约 276px；详情约 408px，无选中时 `Visibility=Collapsed`。

### 8.2 左侧筛选（必须在一起）

- 标题「筛选」，有搜索词或选中标签时显示「清除」
- 搜索：名称、路径、字段内容、标签
- 「标签 · 多选为或」芯片，数据来自全库用过的标签
- 不要把搜索和标签再拆到树的上下两端

### 8.3 分组树

- 虚拟根「全部收藏」：不是数据库分组，图标/颜色存在 `AppSettingsModel`
- 其余节点：`[chevron] [Fluent 图标] [名称] [计数]`
- 计数 = 自身 + 所有后代条目数，**不受筛选影响**
- 点名称选中；点箭头展开；**点图标**打开 `GroupIconDialog`（不要误选分组）
- 右键：查看 / 新建子分组 / 添加条目 / 设置图标 / 删除（全部收藏没有删除）
- 有子分组时删除：先要求移走或删掉子节点；有条目的非顶级分组可把条目上移一层（与原型一致）

### 8.4 中间库

- 面包屑：全部收藏 / 祖先链
- 标题 + `N 项` 或筛选时 `N / M 项`
- 完整度：全部 / 不完整 / 路径丢失 / 缺少图片 / 缺少文字 / 缺少链接
- 排序：名称 A–Z、Z–A、最近加入、最早加入
- 卡片 / 列表
- 「新建分组」「添加条目」
- **全部收藏**：按顶级分组分块，块内再按子分组（与原型 `renderLibrary` 一致）
- **选中有子分组的节点**：先「本组」直属条目，再每个子分组一块；无筛选时空子分组也显示
- 卡片比例跟分组 `Kind` 走：`games` 竖图、`music` 方封、`video` 宽图（存在分组上，子分组继承）
- 无封面时用占位（色块/标题即可，**不要扫原目录找 cover**）

### 8.5 详情

- 名称、路径只读、存在芯片、「打开原目录」（`FileActionHelper`，只开资源管理器，不写盘）
- 分组（编辑时可改所属）
- 标签（编辑时增删；只影响筛选）
- 自定义字段：图片 / 文字 / 链接，可改字段名、删字段、底部再加
- 图片：导入走文件对话框 → 复制进媒体库；点击看大图（lightbox）
- 「从库中移除」确认后只删库

### 8.6 对话框（对照原型）

| 对话框 | 要点 |
|---|---|
| 添加条目 | 已有路径（`AppFileHelper.PickFolder`）+ 显示名 + 放到分组。只记录路径 |
| 新建分组 | 名称 + 父级。默认图标文件夹 `E8B7`，颜色继承父级 |
| 设置图标 | 精选 Fluent 网格 + 搜索 + 色板 + 自定义色。含「全部收藏」 |
| 说明 | 替换 HELLO WORLD About，写清 LocalAppData 规则 |

---

## 9. 数据模型与实体

全部 `Entity`（Guid 主键），放 Framework。JSON 列用 FreeSql JsonMap。

### 9.1 `CollectionGroup`

| 字段 | 说明 |
|---|---|
| Id | Guid |
| ParentId | Guid?，null 为顶级 |
| Name | 名称 |
| Kind | `games` / `music` / `video`，子节点可空并继承祖先 |
| Icon | 码位字符串，如 `E7FC` |
| IconColor | `#RRGGBB` |
| SortOrder | 同级顺序 |

禁止 `Path`。分组不是磁盘文件夹。

### 9.2 `CollectionItem`

| 字段 | 说明 |
|---|---|
| Id | Guid |
| GroupId | 所属分组 |
| Name | 显示名 |
| Path | 用户绑定的已有路径（只记字符串） |
| AddedAt | 加入库的时间 |
| Tags | `List<string>` JsonMap，或独立表。实现选一种，全项目一致 |

`Exists` **不要持久化当真相**：每次展示用 `IPathProbeService` 探测。

### 9.3 `CollectionField`

| 字段 | 说明 |
|---|---|
| Id | Guid |
| ItemId | 所属条目 |
| Type | `images` / `text` / `links` |
| Label | 用户可改的字段名 |
| SortOrder | 顺序 |
| Payload | JSON：text=`string`；images=`[{id, relativePath, caption}]`；links=`[{title,url}]` |

图片 `relativePath` 相对 `MediaFolder`，不要存原盘绝对路径作为唯一来源。

### 9.4 设置

`AppSettingsModel` 增加「全部收藏」的 `Icon` / `IconColor`（默认 `E8F1` / `#326cf3`）。

### 9.5 关系

- Group 1—N Group（树）
- Group 1—N Item
- Item 1—N Field
- 标签在条目上，**没有**「标签分组」实体

首启：**空库**（见第 15 节若要演示数据）。

---

## 10. 业务流程实现路径

### 10.1 添加条目

`CollectionLibraryViewModel` / 树右键 → `AddItemDialog` → 选已有文件夹 → `ILibraryService.AddItem` 只插库 → 工作区选中该条并进入编辑。Toast：「已写入本机库，原目录没有改动」。

### 10.2 自定义字段

详情编辑态改 label / 文本 / 链接；加图走 `IMediaStore.Import`。保存写 `CollectionField`。

### 10.3 筛选

`ICollectionWorkspace`：Query、TagFilters（OR）、IncompleteMode、SortBy。`ILibraryService.QueryItems(scopeGroupId)` 先按树范围取，再套这四层。树绑定未筛选的全量分组。

### 10.4 设置树图标

点图标或右键 → `GroupIconDialog`。目标为 `all` 时写设置，否则写 `CollectionGroup`。

### 10.5 打开原目录

`FileActionHelper` 打开资源管理器。路径不存在则提示（库记录仍在）。**不要** `Process.Start` 去跑 exe / 播视频。

### 10.6 从库中移除

确认 → 删条目和字段（及无引用媒体，可稍后）→ 不碰 `Path` 指向的磁盘。

---

## 11. rules 与 skills 更新

在**开始写业务代码的那一轮**再改 rules（本 plan 确认前不要动）。

| 动作 | 文件 |
|---|---|
| 新增 | `.cursor/rules/collection-domain.mdc`：LocalAppData only、禁止写原路径、标签只筛选、树计数不受筛选、Fluent 图标、图片进 MediaFolder |
| 更新 | `architecture.mdc`：补产品一句话 + 主界面三栏，注明无 Modules、无图表 |
| 不改 | `wpf-core` / `csharp-syntax` / `icon-usage` / 现有 skills |

不需要新 skill；对话框走 `add-dialog`，子视图走 `add-view`。

---

## 12. 文档

- `README.md`：个人藏品库、打开方式、数据在 `%LocalAppData%\RainbowWolfer\SimpleCollectionHub\`、不碰原目录。
- 不要把 HTML 原型写成「产品文档」；点明 `Files/ui-prototype-4` 只是设计稿。

---

## 13. 任务执行顺序（分模块、多轮执行）

**一轮对话只做一轮。** 每轮结束：`dotnet build` 能过，能启动看到本轮界面。

### Round 1：基础设施 + 空壳主窗口

- [P0] 注册 `IFreeSql` + Code First 同步空表 → 启动后 LocalAppData 出现 `AppData.db`
- [P0] `AppFolderConfig.MediaFolder` 并创建目录
- [P0] 实体类 + Repository（可先空表）
- [P0] 拆掉 HELLO WORLD，`MainWindow` 三栏空壳（侧栏/库/详情占位），原生窗口
- [P1] 页脚那句「元数据只存在本机 LocalAppData…」
- **验证**：启动不再是模板按钮墙；库文件出现在 Data 目录；原盘无新文件

### Round 2：分组树 + 图标

- [P0] `ILibraryService` 分组 CRUD、树绑定、展开、计数
- [P0] 「全部收藏」虚拟节点 + 设置里的图标颜色
- [P0] `GroupIconDialog`（Fluent 精选 + 颜色）
- [P0] 新建分组对话框、删除规则、右键菜单
- **验证**：能建嵌套分组、改图标颜色、刷新后还在；树计数在还没有条目时为 0

### Round 3：条目绑定 + 卡片/列表

- [P0] 添加条目（只记路径）、列表/卡片、面包屑、选中
- [P0] `IPathProbeService` 路径可用 / 丢失芯片
- [P0] 从库中移除、打开原目录
- [P1] `Kind` 控制卡片比例
- **验证**：绑定已有文件夹后原目录零变化；删库记录文件还在

### Round 4：详情 + 自定义字段 + 图片入库

- [P0] 详情编辑：文字 / 链接 / 图片字段增删改名
- [P0] `IMediaStore` 复制到 MediaFolder
- [P1] 图片灯箱
- **验证**：导入图后只在 MediaFolder 多文件；原目录与源图片文件夹不被改写

### Round 5：筛选（搜索 + 标签 + 完整度 + 排序）

- [P0] 侧栏搜索与标签芯片放在同一「筛选」块，带清除
- [P0] 标签 OR；完整度；组内排序；`N / M 项`
- [P0] 筛选不改树结构和计数
- **验证**：与原型 4 同一套交互；选 Steam 类标签时树数字不变

### Round 6：说明、Toast、空态、对照原型修

- [P0] About/说明文案、空库提示、LoadingStatus 读库
- [P1] Toast、侧栏/详情滚动、高 DPI 粗看
- [P1] README
- [P1] 按第 11 节补 `collection-domain.mdc`
- **验证**：对照 `Files/ui-prototype-4` 走一遍添加 / 筛选 / 改图标 / 移除；`dotnet build` 干净

**不要**在 Round 3 之前做筛选，也不要在 Round 1 塞满业务。

---

## 14. 验收标准

- `dotnet build` 无错误（若 MSB3021 文件锁：停编、告知，不死循环）
- 启动：闪屏 → 三栏主窗口，不是 HELLO WORLD
- 最小闭环：新建分组 → 绑定已有目录为条目 → 加一张图和一条链接 → 标签筛到它 → 从库移除后原目录完好
- 用 Process Monitor 或人工看：操作前后目标路径文件集合不变
- 数据仅出现在 `AppFolderConfig` 的 Data / Media / Debug

---

## 15. 待补充 / 待确认事项

执行前建议拍板（括号内为计划默认，与原型和已确认规则对齐）：

| 项 | 默认（将按此做，除非你改口） |
|---|---|
| 导入的图片 | 复制进 LocalAppData `Media`，库内只记相对路径 |
| 首次启动 | 空库，不写入原型里的 Hades 等演示数据 |
| 「打开原目录」 | 做，只开资源管理器 |
| 窗口 | 原生 WPF 标题栏，不仿 HTML 假标题栏 |
| 条目路径 | 详情只读；不做「重新指定」 |
| 分组 `Kind` | 新建时继承父级，顶级默认 `games`；不做单独「类型」编辑 UI，除非以后要 |
| 未引用媒体文件 | Round 4 可只导入不回收，Round 6 再扫垃圾 |
| 多语言 | 先中文 `AppStrings`，不在首版做语言切换 |
| 部署 | 本机 exe，不做 ClickOnce / 安装包 |
| HTML 原型 | 保留作对照，不删除 |

原型有、但产品已拒绝的能力（自动封面、拖放、启动文件、sidecar 等）**不要**再问要不要做，除非你主动改需求。

---

## 16. 执行阶段代码质量准则

1. **csproj / slnx**：本计划不新建 csproj。若以后新建，必须同时改 `.slnx`。
2. **XAML**：`base:DISource` + 正确 xmlns；View 用 `UserControlStyle` + `ViewModelBaseEx`；对话框用 `DialogViewModelOkCancel<T>`。
3. **窗口**：原生 `Window` 行为完整。
4. **无图表**：不要引入 ScottPlot 等。
5. **属性 / 命令**：`GetProperty`/`SetProperty`；命令 `field ??= new DelegateCommand`；禁止 `var`；禁止写原路径。
6. **TypedUIObjectService**：只在 XAML 注册，`.As<T>()`。
7. **选文件夹**：`AppFileHelper.PickFolder`。
8. **注释**：接口成员 `///`；实现类注释率 ≥ 30%。
9. **对照**：每轮 UI 对照 `Files/ui-prototype-4`，不要对照 v1–v3。
10. **DRY**：筛选条件、树计数、字段 JSON 只应有一处实现。

---

## 附录 A — 原型 4 交互核对表

执行 Round 6 时逐项勾：

- [ ] 筛选在树上方且搜索+标签在一起
- [ ] 标签多选为或
- [ ] 树计数不随筛选变
- [ ] 点图标改 Fluent 字形和颜色（含全部收藏）
- [ ] 嵌套树、面包屑、父级分块
- [ ] 自定义字段三种类型
- [ ] 完整度五种 + 排序四种
- [ ] 卡片 / 列表
- [ ] 详情可关
- [ ] 页脚 LocalAppData 说明
- [ ] 移除条目不删盘
)
</parameter>
</invoke>
</parameter>
</invoke>