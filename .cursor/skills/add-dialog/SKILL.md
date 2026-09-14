---
name: add-dialog
description: >-
  在 SimpleCollectionHub 客户端中新建一个弹窗对话框，包含 Parameter record、DialogViewModel 和调用方的注册写法。
  当用户需要新建弹窗、编辑对话框、确认框时使用。
disable-model-invocation: false
---

# 新建弹窗对话框

## 文件组织

Dialog 的 UserControl 与 ViewModel 写在**同一个 `.xaml.cs` 文件**中，放在 `Views/Dialogs/` 目录下。

---

## Step 1 — Dialog 文件（`.xaml.cs`）

基类按按钮组合选择：

| 基类 | 底部按钮 |
|---|---|
| `DialogViewModel<T>` | 无（自己在 `GetDialogCommands()` 里提供） |
| `DialogViewModelOk<T>` | 只有确定 |
| `DialogViewModelOkCancel<T>` | 确定 / 取消 |

```csharp
namespace SimpleCollectionHub.<模块名>.Views.Dialogs;

public partial class XxxDialog : UserControl
{
    public XxxDialog() => InitializeComponent();
}

internal record XxxDialogParameter(XxxItem? Item)
{
    public XxxItem? Result { get; set; }
}

internal class XxxDialogViewModel : DialogViewModelOkCancel<XxxDialogParameter>
{
    public SomeModel EditTarget { get; } = new();

    protected override void OnInitialized()
    {
        // Parameter 和 ParentViewModel 均已就绪
        if (Parameter.Item is not null)
        {
            DialogTitle = "编辑";
        }
        else
        {
            DialogTitle = "新增";
        }
    }

    protected override bool Validate(out string message)
    {
        message = string.Empty;
        return true;
    }

    protected override bool OnConfirmed()
    {
        Parameter.Result = EditTarget;
        return true;
    }
}
```

---

## Step 2 — Dialog View（XAML）

先读取 `Source/XAML_Preset.txt` 中 `-------------- Dialog --------------` 区段复制 xmlns，注意：

- `Style="{StaticResource DialogStyle.Base}"`（`StaticResource`，不是 `DynamicResource`）
- `DataContext="{base:DISource Type=local:XxxDialogViewModel}"`

---

## Step 3 — 调用方 View（XAML）注册

控件类型是 `DialogService`，接口是 `IDialogServiceEx`：

```xml
<dxmvvm:Interaction.Behaviors>
    <f:DialogService x:Name="XxxDialog" ContentType="{x:Type dialogs:XxxDialog}"/>
</dxmvvm:Interaction.Behaviors>
```

`dialogs` 命名空间：

```xml
xmlns:dialogs="clr-namespace:SimpleCollectionHub.<模块名>.Views.Dialogs"
```

---

## Step 4 — 调用方 ViewModel 注册与调用

```csharp
public IDialogServiceEx XxxDialog => GetService<IDialogServiceEx>(nameof(XxxDialog));

public IDelegateCommand AddCommand => field ??= new DelegateCommand(Add);
private void Add()
{
    XxxDialogParameter parameter = new(null);
    if (XxxDialog.ShowOKCancel(this, parameter))
    {
        XxxItem result = parameter.Result;
    }
}
```

`IDialogServiceEx` 的两个方法：

| 方法 | 返回值 |
|---|---|
| `ShowOKCancel(parentVM, parameter)` | `bool`（用户点确定为 true） |
| `ShowDialog(parentVM, parameter)` | `DialogResult`（含 `DialogResultFlag` 与按下的命令） |

属性名必须和 XAML 里的 `x:Name` 一致。

---

## 检查清单

- [ ] Parameter 使用 `record` 类型，构造参数为传入值，属性为传出值
- [ ] ViewModel 继承 `DialogViewModelOkCancel<XxxDialogParameter>`（或另外两个基类）
- [ ] 传出结果在 `OnConfirmed()` 中写入 `Parameter.Result`
- [ ] Dialog View 使用 `Style="{StaticResource DialogStyle.Base}"`
- [ ] 调用方 XAML 声明 `f:DialogService`，`x:Name` 与 ViewModel 属性名一致
- [ ] 调用方 ViewModel 用 `GetService<IDialogServiceEx>(nameof(XxxDialog))`
