using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleCollectionHub.Resources;

/// <summary>
/// 资源访问入口。图片与图标本身由 T4 生成到
/// <c>Icons/IconResources.cs</c> 与 <c>Images/ImageResources.cs</c>，
/// 这里只放需要在代码里计算的资源。
/// </summary>
public static class ResourceKeys
{
	/// <summary> 系统默认字体，用于 ToolTip 等需要显式指定字体的场景 </summary>
	public static readonly FontFamily DefaultFont = (FontFamily)TextBlock.FontFamilyProperty.DefaultMetadata.DefaultValue;

	/// <summary>
	/// 在启动早期预热资源。放在这里是为了把耗时集中到启动页阶段，
	/// 避免首次打开窗口时卡顿。
	/// </summary>
	public static void Initialize()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		try
		{

		}
		finally
		{
			stopwatch.Stop();
			Debug.WriteLine($"ResourceKeys.Initialize() took: {stopwatch.ElapsedMilliseconds}ms.");
		}
	}
}
