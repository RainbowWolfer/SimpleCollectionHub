using DevExpress.Mvvm;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModels;
using SimpleCollectionHub.Framework.ViewModelServices;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SimpleCollectionHub.ViewModels;

/// <summary>主窗口壳：加载收藏库、说明对话框、退出确认。</summary>
internal class MainWindowViewModel(
	ICollectionWorkspace workspace
) : ViewModelBaseEx
{
	public ICollectionWorkspace Workspace => workspace;

	public IDialogServiceEx AboutDialog => GetService<IDialogServiceEx>(nameof(AboutDialog));

	protected override void LoadedOnce()
	{
		base.LoadedOnce();
		WindowService.Object.Closing += Window_Closing;
		_ = workspace.ReloadAsync();
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		if (!MessageBoxService.ShowOkCancelQuestion("是否确认退出 SimpleCollectionHub？"))
		{
			e.Cancel = true;
		}
	}

	public IDelegateCommand OpenAboutCommand => field ??= new DelegateCommand(OpenAbout);
	private void OpenAbout()
	{
		AboutDialog.ShowDialog(this);
	}

	public IDelegateCommand CloseLightboxCommand => field ??= new DelegateCommand(workspace.CloseLightbox);

	public IDelegateCommand RefreshCommand => field ??= new AsyncCommand(Refresh);
	private async Task Refresh()
	{
		await workspace.ReloadAsync();
	}
}
