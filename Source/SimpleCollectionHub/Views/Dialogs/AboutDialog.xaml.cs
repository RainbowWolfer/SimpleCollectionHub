using SimpleCollectionHub.Framework.ViewModels;
using System.Windows.Controls;

namespace SimpleCollectionHub.Views.Dialogs;

public partial class AboutDialog : UserControl
{
	public AboutDialog()
	{
		InitializeComponent();
	}
}

/// <summary>产品说明：元数据只在 LocalAppData，不改原目录。</summary>
internal class AboutDialogViewModel : DialogViewModelOk
{
	protected override void OnInitialized()
	{
		DialogTitle = "说明";
		ConfirmDialogCommand.Content = "关闭";
	}
}
