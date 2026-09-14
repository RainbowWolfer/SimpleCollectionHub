---
name: update-rules-skills
description: >-
  更新项目 rules 和 skills 的专用入口。当用户要求修改 rules、更新 skills、添加新约束、
  强调已有规则、根据错误反例强化规范、或调整 rules/skills 的加载策略时使用。
  触发词：/update-rules-skills、修改 rules、更新 skill、强化规则、规则没生效、AI 老写错。
disable-model-invocation: false
---

# 更新项目 Rules & Skills

本 skill 是修改项目 rules（`SimpleCollectionHub` AI 行为约束）和 skills（`SimpleCollectionHub` 专用技能）的唯一入口。

## 核心职责

1. **添加新内容**：按分层策略决定新约束/新参考放到哪个 rule 或 skill 中
2. **修改已有内容**：外科手术式修改现有 rule/skill 文件，不破坏结构
3. **强化已有规则**：当用户反馈"AI 还是写错 XX"时，分析原因并针对性强化
4. **调整加载策略**：根据内容性质修改 `alwaysApply`/`globs`/`disable-model-invocation`

---

## Step 0 — 了解当前架构

在修改前，必须读取并理解以下文件，确保不破坏现有结构：

### Rules 清单

| 文件 | 加载策略 | 内容范围 |
|---|---|---|
| `.cursor/rules/wpf-core.mdc` | alwaysApply: true | 项目结构、命名约定、ViewModel、MVVM属性、Command、服务、DI、对话框、LoadingStatus、文件对话框/文件夹选择、Mapster |
| `.cursor/rules/csharp-syntax.mdc` | alwaysApply: true | C# 语法硬约束（禁止 var、null检查、Debug.WriteLine、DRY等） |
| `.cursor/rules/architecture.mdc` | alwaysApply: true | 技术栈、项目架构、编译校验 |
| `.cursor/rules/editor-conventions.mdc` | alwaysApply: true | 换行符、编码规范 |
| `.cursor/rules/rw-libraries-reference.mdc` | alwaysApply: true | RW.Common / RW.Common.WPF / RW.Base.WPF 源码路径与结构速查 |
| `.cursor/rules/wpf-xaml.mdc` | globs: `**/*.xaml` | XAML 头部、BindingProxy、DRY、颜色、枚举绑定、MarkupExtension |
| `.cursor/rules/icon-usage.mdc` | globs: `**/*.xaml`, `**/*.cs` | 图标/图片 T4 资源管线完整规范 |
| `.cursor/rules/controls-reference.mdc` | globs: `**/*.xaml` | Framework 自有控件 + RW.Common.WPF 通用控件 |

### Skills 清单

| Skill | disable-model-invocation | 类型 |
|---|---|---|
| `add-view` | false | 操作：新建 View+ViewModel |
| `add-dialog` | false | 操作：新建对话框 |
| `add-service` | false | 操作：新建 Service |
| `add-module` | false | 操作：新建模块 |
| `initialize-new-project` | true | 操作：初始化项目（需用户点名触发） |
| `write-commit-message` | true | 工具：生成提交日志 |
| `write-staged-commit-message` | true | 工具：生成暂存区提交日志 |
| `review-rules` | true | 审查：代码合规检查 |
| `reference-typed-ui-object-service` | false | 参考：TypedUIObjectService API |
| `reference-loading-status` | false | 参考：LoadingStatus API |
| `reference-enum-display` | false | 参考：EnumDisplayService 内部实现 |
| `reference-timer` | false | 参考：CompensatingTimer 及三个 TimerController 定时器用法 |
| `update-rules-skills` | false | 元：修改 rules/skills（本 skill 自身） |

---

## Step 1 — 决定内容去向

当用户提供了新内容（代码示例、新约束、新控件等），按以下决策树决定放哪里：

### 决策树

```
用户提供的内容是什么？
├── 行为约束（"必须做 X"、"禁止做 Y"、"用 A 替代 B"）
│   ├── 与 C# 编码通用 → csharp-syntax.mdc
│   ├── 与 WPF MVVM、属性、命令相关 → wpf-core.mdc
│   └── 与 XAML 编写相关 → wpf-xaml.mdc
│
├── API 参考 / 代码模板（方法签名、属性列表、复制即用的代码块 >20 行）
│   ├── 与控件/服务/工具类相关 → 新建或更新 reference-* skill（disable-model-invocation: false）
│   └── 与已有 reference skill 高度相关 → 更新那个 skill
│
├── 图标相关 → icon-usage.mdc（图标系统已在其中完整覆盖）
│
├── 控件文档（作用不是约束而是教"怎么用"） → controls-reference.mdc
│
└── 需要澄清 → 先向用户确认应该放在哪里
```

### alwaysApply 阈值

`alwaysApply: true` 是稀缺资源。以下判断标准：

- **可以放 alwaysApply**：无论任务类型，缺了这条 AI 就会写错的硬约束
- **不能放 alwaysApply**：只是参考手册、API 列表、代码片段、或仅在特定文件类型才需要的约束

当 alwaysApply 的 rules 总行数超过 1500 行时，必须向用户预警："当前 alwaysApply 总量已达 X 行，新增内容建议放到 globs 规则或 reference skill 中"。

---

## Step 2 — 修改策略

### 添加前：去重检查（强制，不可跳过）

**在添加任何新内容之前，必须先确认已有 rules 和 skills 中没有相同或高度相似的内容。** 跳过此检查会导致重复定义、矛盾冲突、或冗余 skill 触发。

#### 检查流程

```
用户要求添加的内容
  ├── 1. grep 所有 rules 和 skills，搜索关键词
  │     └── 关键词 = 用户需求中的核心术语（类名、方法名、约束主题等）
  │
  ├── 2. 判断命中结果
  │     ├── 完全命中（已有文件明确写了同样的内容）
  │     │   └── 告知用户"这条规则已存在于 [文件名] 第 X 行，无需重复添加。是否需要强化/修改？"
  │     │
  │     ├── 部分命中（已有文件覆盖了相关主题，但未覆盖用户提出的具体细节）
  │     │   └── 告知用户"相关内容已在 [文件名] 中，我可以在此基础追加新细节。是否确认？"
  │     │
  │     └── 无命中 → 继续执行添加
  │
  └── 3. 特别检查：新建 skill 前
        └── 确认没有已有 skill 覆盖同样的主题（包括命名相似、功能重叠）
```

#### 常见重复风险

| 用户说 | 实际已存在于 | 处理方式 |
|---|---|---|
| "加一条关于 TypedUIObjectService 的用法" | `wpf-core.mdc` + `reference-typed-ui-object-service` skill | 告知已有，询问是否需要补充细节 |
| "建一个关于图标系统的 skill" | `icon-usage.mdc`（规则已完整覆盖） | 告知已有，图标类不需要拆分 skill |
| "完善 MVVM 属性的规范" | `wpf-core.mdc`「MVVM 属性定义」 | 告知已有，确认是否要强调/补充反例 |
| "增加 LoadingCover 的用法" | `wpf-core.mdc` + `wpf-xaml.mdc` + `reference-loading-status` skill | 告知已有，询问具体要补什么 |

### 添加新内容（追加到已有文件）

1. **（必须先执行去重检查）**
2. 确定目标文件（按 Step 1 决策树）
3. 用 `Read` 读取目标文件，找到最佳插入位置
4. 在相关章节末尾追加新内容（保持与已有内容的间距一致）
5. 如果是全新的主题（文件中没有对应章节），在合适位置插入新 `##` 段落

### 新建 Rule 或 Skill 文件

当 Step 1 决策树判断现有文件都不适合承载新内容时，**可以新建 rule 或 skill**。

#### 新建 Rule（`.cursor/rules/xxx.mdc`）

**触发条件**：新内容是一个独立的行为约束或 XAML 约定，与现有 rule 文件没有概念交集。

**文件模板**：

```markdown
---
description: 简要描述规则用途（一句话）
alwaysApply: true | false
globs: "**/*.cs"  # 仅 alwaysApply: false 时需要
---

# 规则标题

规则内容（代码示例、反例对比等）
```

**命名**：`{domain}-{focus}.mdc`，如 `wpf-core.mdc`、`api-conventions.mdc`。

#### 新建 Skill（`.cursor/skills/{name}/SKILL.md`）

**触发条件区分**：

| 新内容性质 | 应新建的 skill 类型 | disable-model-invocation |
|---|---|---|
| 操作类（创建文件/更改代码） | `{verb}-{noun}` 操作 skill | `false`（用户说"新建 XX"时自动触发） |
| 参考类（API 手册/代码模板） | `reference-{topic}` 参考 skill | `false`（相关代码出现时自动加载） |
| 工具类（分析/生成/审查） | `{verb}-{noun}` 工具 skill | `true`（需用户点名触发，避免误判） |

**文件模板**：

```markdown
---
name: {skill-name}
description: >-
  简要描述 skill 功能和触发场景。Use when {触发条件}。
disable-model-invocation: false
---

# {Skill 标题}

## 操作步骤

...
```

**命名**：小写字母 + 连字符，最长 64 字符。操作 skill 参考 `add-view`；参考 skill 以 `reference-` 开头；工具 skill 参考 `write-commit-message`。

**新建后**：把新 skill 追加到 Step 0 的 Skills 清单中，确保下次修改时能看到完整视角。

### 修改已有内容

1. 用 `StrReplace` 进行外科手术式精确替换
2. **只改用户要求的部分**，不顺手优化相邻内容
3. 保持原有缩进、格式、代码块语言标签一致

### 强化已有规则（用户反馈"AI 还是写错 XX"）

当用户说 AI 没有遵守某条规则时，按以下优先级处理：

1. **检查规则是否存在**：在 rules 中 grep 相关内容
2. **如果规则已存在但 AI 没遵守**：
   - 检查 loading strategy：是否被放在 globs 规则中而当前场景未触发？
   - 提升优先级：将关键反例从 globs 规则移到 alwaysApply 核心规则
   - 增加强调标记：在关键位置加 `**禁止**`、`❌` 标记
   - 增加正反例对比：如果没有反例代码块，补充 "❌ 错误 vs ✅ 正确" 对比
3. **如果规则不存在**：按 Step 1 决策树添加新约束
4. **如果规则正确但 AI 仍然忽略**：可能是上下文过长被稀释。考虑：
   - 精简 alwaysApply 中的冗余文字
   - 把这条规则提前到规则文件更靠前的位置
   - 在 description 中加入触发关键词

### 强调/标记策略

以下标记按严重程度递减：

| 标记 | 含义 | 示例 |
|---|---|---|
| `**强制遵守**` | 最高优先级，放章节最前 | `**强制遵守：** 图标必须走 T4 资源管线` |
| `**禁止**` | 绝对不能做的 | `**禁止**使用 var` |
| `**必须**` | 必须要做的 | `**必须**在 .xaml 中加名注释` |
| `❌ 错误 / ✅ 正确` | 正反例对比 | 代码块标注 |
| `（详见 xxx skill）` | 内容引用，避免重复 | `TypedUIObjectService 完整 API 详见 reference-xxx` |

---

## Step 3 — 编译验证（仅在修改 rule/skill 本身代码示例时）

如果修改涉及 **rule/skill 中的 C# 代码示例可能影响编译**（如修改了方法的签名、引入了新的类型引用等），需要运行 `dotnet build -v q` 验证。

如果只是调整规则文字、标记、排版等，不需要编译。

---

## Key Constraints

1. **外科手术**：只改用户要求的部分，不顺手优化不相干内容
2. **不破坏分层**：不把 API 参考写进 alwaysApply，不把硬约束降级到 globs
3. **不重复内容**：同一段信息不在两个地方出现（用 "详见 xxx" 引用）
4. **不改已有文件的换行符/编码**（`editor-conventions.mdc` 约束）
5. **操作后汇报**：简要说明改了哪个文件、加/改了什么内容、为什么放在那里
