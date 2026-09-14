using DevExpress.Mvvm;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleCollectionHub.Framework.ViewModels;

public class ButtonViewModel : BindableBase
{

	public string Content
	{
		get => GetProperty(() => Content);
		set => SetProperty(() => Content, value);
	}

	public ImageSource Icon
	{
		get => GetProperty(() => Icon);
		set => SetProperty(() => Icon, value);
	}

	public ICommand Command
	{
		get => GetProperty(() => Command);
		set => SetProperty(() => Command, value);
	}

	public ButtonViewModel()
	{

	}

}
