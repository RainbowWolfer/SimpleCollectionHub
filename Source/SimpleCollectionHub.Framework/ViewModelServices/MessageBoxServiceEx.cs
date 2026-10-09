using DevExpress.Mvvm;
using SimpleCollectionHub.Framework.Configs;
using RW.Base.WPF.ViewModelServices;
using System.Windows;

namespace SimpleCollectionHub.Framework.ViewModelServices;

/// <summary>
/// 消息框服务。按钮和标题沿用框架服务，外观换成 HandyControl。
/// </summary>
public class MessageBoxServiceEx : MessageBoxService, IMessageBoxServiceEx
{
	public MessageBoxServiceEx()
	{
		MessageTitle = AppConfig.AppName;
	}

	public override MessageResult Show(string messageBoxText, string caption, MessageButton button, MessageIcon icon, MessageResult defaultResult)
	{
		// 有所属窗口时作为它的子对话框，没有时（启动早期）就直接弹出
		Window? window = AssociatedObject != null ? Window.GetWindow(AssociatedObject) : null;
		MessageBoxResult result = window is null
			? HandyControl.Controls.MessageBox.Show(messageBoxText, caption, ToButton(button), ToImage(icon), ToResult(defaultResult))
			: HandyControl.Controls.MessageBox.Show(window, messageBoxText, caption, ToButton(button), ToImage(icon), ToResult(defaultResult));

		// HandyControl 返回的是 WPF 的 MessageBoxResult，调用方仍使用 DevExpress 的 MessageResult
		return result switch
		{
			MessageBoxResult.OK => MessageResult.OK,
			MessageBoxResult.Cancel => MessageResult.Cancel,
			MessageBoxResult.Yes => MessageResult.Yes,
			MessageBoxResult.No => MessageResult.No,
			_ => MessageResult.None,
		};
	}

	/// <summary>DevExpress 的按钮组合映射到 WPF。没列出的组合退回只有确定。</summary>
	private static MessageBoxButton ToButton(MessageButton button)
	{
		return button switch
		{
			MessageButton.OKCancel => MessageBoxButton.OKCancel,
			MessageButton.YesNoCancel => MessageBoxButton.YesNoCancel,
			MessageButton.YesNo => MessageBoxButton.YesNo,
			_ => MessageBoxButton.OK,
		};
	}

	/// <summary>图标只保留错误、询问、警告、信息。其余（包括 None）不显示图标。</summary>
	private static MessageBoxImage ToImage(MessageIcon icon)
	{
		return icon switch
		{
			MessageIcon.Error => MessageBoxImage.Error,
			MessageIcon.Question => MessageBoxImage.Question,
			MessageIcon.Warning => MessageBoxImage.Warning,
			MessageIcon.Information => MessageBoxImage.Information,
			_ => MessageBoxImage.None,
		};
	}

	/// <summary>默认按钮。对不上的值交给系统自己决定，避免弹出一个无效的默认焦点。</summary>
	private static MessageBoxResult ToResult(MessageResult result)
	{
		return result switch
		{
			MessageResult.OK => MessageBoxResult.OK,
			MessageResult.Cancel => MessageBoxResult.Cancel,
			MessageResult.Yes => MessageBoxResult.Yes,
			MessageResult.No => MessageBoxResult.No,
			_ => MessageBoxResult.None,
		};
	}
}
