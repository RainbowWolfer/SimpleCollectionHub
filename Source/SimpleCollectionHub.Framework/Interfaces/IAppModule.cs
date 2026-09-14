using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleCollectionHub.Framework.Interfaces;

/// <summary>
/// 功能模块元数据。每个模块项目提供一个实现，
/// 由主程序在启动后收集并展示。
/// </summary>
public interface IAppModule
{
	/// <summary>模块唯一键，对应 <c>ConstKeys.SK_*</c>。</summary>
	string Key { get; }

	/// <summary>短显示名，用于导航、Tab 标题等紧凑位置。</summary>
	string DisplayName { get; }

	/// <summary>完整中文名。</summary>
	string FullName { get; }

	/// <summary>完整英文名。</summary>
	string EnglishFullName { get; }

	/// <summary>模块功能说明。</summary>
	string Description { get; }

	/// <summary>模块图标。通常来自 <c>IconResources</c> 的位图。</summary>
	ImageSource Icon { get; }

	/// <summary>模块主视图。</summary>
	ContentControl Content { get; }

	/// <summary>是否对用户可见、可进入。</summary>
	bool IsEnabled { get; }
}
