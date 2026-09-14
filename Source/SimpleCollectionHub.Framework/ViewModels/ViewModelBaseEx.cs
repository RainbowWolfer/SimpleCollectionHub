using DevExpress.Mvvm;
using RW.Base.WPF.ViewModelServices;
using System.Windows;
using System.Windows.Controls;

namespace SimpleCollectionHub.Framework.ViewModels;


public class ViewModelBaseEx : ViewModelBase
{
	public IMessageBoxServiceEx MessageBoxService => GetService<IMessageBoxServiceEx>();
	public IDispatcherServiceEx DispatcherService => GetService<IDispatcherServiceEx>();

	public IUIObjectService<UserControl> UserControlService => GetService<ITypedUIObjectService>(nameof(UserControlService)).As<UserControl>();
	public IUIObjectService<Window> WindowService => GetService<ITypedUIObjectService>(nameof(WindowService)).As<Window>();

	private DelegateCommand? loadedCommand;
	public IDelegateCommand LoadedCommand => loadedCommand ??= new(Loaded);
	protected virtual void Loaded()
	{

	}


	private DelegateCommand? loadedOnceCommand;
	public IDelegateCommand LoadedOnceCommand => loadedOnceCommand ??= new(LoadedOnce);
	protected virtual void LoadedOnce()
	{

	}

}
