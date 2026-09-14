using RW.Base.WPF.DependencyInjections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCollectionHub.Framework.Services;

/// <summary>一条精选 Segoe Fluent Icons，供分组图标对话框搜索。</summary>
public sealed class FluentIconItem
{
	/// <summary>码位，如 E7FC。</summary>
	public string Code { get; init; } = "E8B7";

	/// <summary>中文名。</summary>
	public string Name { get; init; } = string.Empty;

	/// <summary>搜索关键字。</summary>
	public string Keys { get; init; } = string.Empty;

	/// <summary>FontIcon 用的字形。</summary>
	public string Glyph => Helpers.FluentIconHelper.ToGlyph(Code);
}

/// <summary>系统内置（项目自带字体）图标目录。</summary>
public interface IFluentIconCatalog
{
	/// <summary>全部精选图标。</summary>
	IReadOnlyList<FluentIconItem> All { get; }

	/// <summary>按名称 / 码位 / 关键字过滤。</summary>
	IReadOnlyList<FluentIconItem> Search(string query);

	/// <summary>预设颜色，#RRGGBB。</summary>
	IReadOnlyList<string> Colors { get; }
}

/// <summary>与原型 4 对齐的精选码位。不引用 Segoe MDL2 作为字体来源。</summary>
internal class FluentIconCatalog : IFluentIconCatalog, ISingletonDependency
{
	public IReadOnlyList<FluentIconItem> All { get; } =
	[
		Item("E8F1", "库", "library 全部 收藏"),
		Item("E71D", "全部", "all apps"),
		Item("E80F", "主页", "home"),
		Item("E8B7", "文件夹", "folder"),
		Item("E838", "打开的文件夹", "open folder"),
		Item("E7C3", "页面", "page document 文档"),
		Item("E8E5", "打开文件", "open file"),
		Item("E7FC", "游戏", "games 手柄"),
		Item("EA86", "拼图", "puzzle 独立"),
		Item("E8D6", "音乐", "music audio 专辑"),
		Item("E7F6", "耳机", "headphones ost"),
		Item("E7F5", "扬声器", "speaker"),
		Item("E768", "播放", "play 在玩"),
		Item("E769", "暂停", "pause"),
		Item("E714", "视频", "video film 电影 动画"),
		Item("E7F4", "显示器", "tv monitor 电视"),
		Item("E722", "相机", "camera"),
		Item("E8B9", "图片", "photo picture"),
		Item("E7C5", "相册", "browse photos"),
		Item("E734", "收藏", "star 想玩 星"),
		Item("E735", "实心收藏", "star fill"),
		Item("EB51", "心形", "heart"),
		Item("EB52", "实心心形", "heart fill"),
		Item("E8A4", "书签", "bookmarks"),
		Item("E8EC", "标签", "tag"),
		Item("E718", "图钉", "pin"),
		Item("E7C1", "旗帜", "flag"),
		Item("E73E", "完成", "check 已通关"),
		Item("E716", "人物", "people"),
		Item("E774", "地球", "globe 开放世界"),
		Item("E707", "地点", "map pin"),
		Item("E787", "日历", "calendar"),
		Item("E8FD", "列表", "list"),
		Item("E890", "视图", "view"),
		Item("E753", "云", "cloud"),
		Item("E896", "下载", "download"),
		Item("E719", "商店", "shop steam"),
		Item("E71B", "链接", "link"),
		Item("E713", "设置", "settings"),
		Item("E721", "搜索", "search"),
		Item("E71C", "筛选", "filter"),
		Item("E990", "Xbox", "xbox console"),
		Item("E7CC", "阅读列表", "reading book"),
		Item("E82D", "词典", "dictionary 书"),
		Item("E715", "邮件", "mail"),
		Item("E8EA", "手机", "phone"),
		Item("E70F", "编辑", "edit"),
		Item("E74E", "保存", "save"),
		Item("E72C", "刷新", "refresh"),
	];

	public IReadOnlyList<string> Colors { get; } =
	[
		"#326cf3", "#1b1b1b", "#5c5c5c", "#c23b22", "#db3340",
		"#b45309", "#ca8a04", "#15803d", "#0f766e", "#0369a1",
		"#1f4fd0", "#7c3aed", "#be185d", "#ea580c",
	];

	public IReadOnlyList<FluentIconItem> Search(string query)
	{
		string q = (query ?? string.Empty).Trim().ToLowerInvariant();
		if (q.Length == 0)
		{
			return All;
		}

		return [.. All.Where(item =>
			item.Name.Contains(q, StringComparison.CurrentCultureIgnoreCase)
			|| item.Code.Contains(q, StringComparison.OrdinalIgnoreCase)
			|| item.Keys.Contains(q, StringComparison.CurrentCultureIgnoreCase))];
	}

	private static FluentIconItem Item(string code, string name, string keys)
	{
		return new FluentIconItem
		{
			Code = code,
			Name = name,
			Keys = keys,
		};
	}
}
