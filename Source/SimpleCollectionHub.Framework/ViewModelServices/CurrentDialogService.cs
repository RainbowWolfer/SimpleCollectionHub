using SimpleCollectionHub.Framework.Controls;
using SimpleCollectionHub.Framework.ViewModels;
using DevExpress.Mvvm.UI;
using ViewHelper = RW.Common.WPF.Helpers.ViewHelper;

namespace SimpleCollectionHub.Framework.ViewModelServices;

public interface ICurrentDialogService
{
	void SubmitDialogCommand(DialogCommand command);
}

public class CurrentDialogService : ServiceBase, ICurrentDialogService
{
	public void SubmitDialogCommand(DialogCommand command)
	{
		if (ViewHelper.FindVisualParent<DialogWindowWrapper>(AssociatedObject) is { } target)
		{
			target.ExecuteCommand(command);
		}
	}
}
