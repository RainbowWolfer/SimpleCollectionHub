using FolderBrowserEx;

namespace SimpleCollectionHub.Framework.Helpers;

public static class AppFileHelper
{
	public static bool PickFolder(string title, string initialFolder, out string path)
	{
		FolderBrowserDialog dialog = new()
		{
			InitialFolder = initialFolder,
			AllowMultiSelect = false,
			DefaultFolder = null,
			Title = title,
		};
		if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
		{
			path = string.Empty;
			return false;
		}
		path = dialog.SelectedFolder;
		return true;
	}

	public static bool PickFolder(out string path)
	{
		return PickFolder(null, null, out path);
	}
}
