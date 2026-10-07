using DevExpress.Mvvm;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModels;
using SimpleCollectionHub.Framework.ViewModelServices;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleCollectionHub.ViewModels;

/// <summary>主窗口壳：加载收藏库、说明对话框、退出确认。</summary>
internal class MainWindowViewModel(
	ICollectionWorkspace workspace,
	IMediaStore mediaStore
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

	/// <summary>窗口隧道阶段拦截 Ctrl+V，这样输入框还没把截图当成文字处理。</summary>
	public IDelegateCommand PreviewKeyDownCommand => field ??= new DelegateCommand<KeyEventArgs>(OnPreviewKeyDown);
	private void OnPreviewKeyDown(KeyEventArgs e)
	{
		if (e is null || e.Handled)
		{
			return;
		}

		if (e.Key != Key.V || Keyboard.Modifiers != ModifierKeys.Control)
		{
			return;
		}

		if (CollectionImagePaste.ShouldLeaveTextPaste())
		{
			return;
		}

		if (!CollectionImagePaste.TryPaste(workspace, mediaStore, null, out string message))
		{
			return;
		}

		e.Handled = true;
		if (message.IsNotBlank())
		{
			workspace.ShowToast(message);
		}
	}
}
