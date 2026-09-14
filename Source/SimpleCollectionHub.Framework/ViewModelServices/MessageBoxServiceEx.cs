using SimpleCollectionHub.Framework.Configs;
using RW.Base.WPF.ViewModelServices;

namespace SimpleCollectionHub.Framework.ViewModelServices;

/// <summary>
/// 消息框服务。在框架的 <see cref="MessageBoxService"/> 基础上补上默认标题。
/// </summary>
/// <remarks>
/// 若要换用第三方消息框（例如 HandyControl），重写 <c>Show</c> 即可。
/// </remarks>
public class MessageBoxServiceEx : MessageBoxService, IMessageBoxServiceEx
{
	public MessageBoxServiceEx()
	{
		MessageTitle = AppConfig.AppName;
	}
}
