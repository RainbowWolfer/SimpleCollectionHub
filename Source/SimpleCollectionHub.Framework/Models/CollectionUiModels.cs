using DevExpress.Mvvm;
using SimpleCollectionHub.Framework.Helpers;
using SimpleCollectionHub.Framework.Database;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace SimpleCollectionHub.Framework.Models;

/// <summary>中间列表的完整度筛选。不影响树计数。</summary>
public enum IncompleteFilter
{
	All,
	Any,
	Path,
	Images,
	Text,
	Links,
}

/// <summary>组内排序。</summary>
public enum ItemSortMode
{
	NameAsc,
	NameDesc,
	AddedDesc,
	AddedAsc,
}

/// <summary>卡片或列表。</summary>
public enum LibraryViewMode
{
	Cards,
	List,
}

/// <summary>左侧树的一行，含虚拟「全部收藏」。</summary>
public class GroupNode : BindableBase
{
	public Guid Id { get; init; }

	public Guid? ParentId { get; init; }

	public bool IsAll { get; init; }

	public string Name
	{
		get => GetProperty(() => Name);
		set => SetProperty(() => Name, value);
	}

	public string Icon
	{
		get => GetProperty(() => Icon);
		set
		{
			SetProperty(() => Icon, value);
			RaisePropertyChanged(() => Glyph);
		}
	}

	public string IconColor
	{
		get => GetProperty(() => IconColor);
		set
		{
			SetProperty(() => IconColor, value);
			RaisePropertyChanged(() => IconBrush);
		}
	}

	public string Kind { get; set; } = GroupKinds.Games;

	public int Count
	{
		get => GetProperty(() => Count);
		set => SetProperty(() => Count, value);
	}

	public bool IsExpanded
	{
		get => GetProperty(() => IsExpanded);
		set => SetProperty(() => IsExpanded, value);
	}

	public bool IsSelected
	{
		get => GetProperty(() => IsSelected);
		set => SetProperty(() => IsSelected, value);
	}

	public ObservableCollection<GroupNode> Children { get; } = [];

	public string Glyph => FluentIconHelper.ToGlyph(Icon);

	public Brush IconBrush => FluentIconHelper.ToBrush(IconColor);

	public override string ToString() => Name;
}

/// <summary>卡片 / 列表行。</summary>
public class ItemCardModel : BindableBase
{
	public Guid Id { get; init; }

	public Guid GroupId { get; set; }

	public string Name
	{
		get => GetProperty(() => Name);
		set => SetProperty(() => Name, value);
	}

	public string Path
	{
		get => GetProperty(() => Path);
		set => SetProperty(() => Path, value);
	}

	public bool Exists
	{
		get => GetProperty(() => Exists);
		set => SetProperty(() => Exists, value);
	}

	public DateTime AddedAt { get; set; }

	public string Kind { get; set; } = GroupKinds.Games;

	public int ImageCount
	{
		get => GetProperty(() => ImageCount);
		set => SetProperty(() => ImageCount, value);
	}

	public ImageSource Cover
	{
		get => GetProperty(() => Cover);
		set
		{
			SetProperty(() => Cover, value);
			RaisePropertyChanged(() => HasCover);
		}
	}

	public string PlaceholderTitle
	{
		get => GetProperty(() => PlaceholderTitle);
		set => SetProperty(() => PlaceholderTitle, value);
	}

	public ObservableCollection<string> Tags { get; } = [];

	public bool IsSelected
	{
		get => GetProperty(() => IsSelected);
		set => SetProperty(() => IsSelected, value);
	}

	public string GroupPath { get; set; } = string.Empty;

	/// <summary>卡片封面宽度，随分组 Kind 变化。</summary>
	public double CoverWidth => Kind switch
	{
		GroupKinds.Music => 168,
		GroupKinds.Video => 228,
		_ => 156,
	};

	/// <summary>卡片封面高度：游戏 3:4、音乐 1:1、视频 16:9。</summary>
	public double CoverHeight => Kind switch
	{
		GroupKinds.Music => 168,
		GroupKinds.Video => 128,
		_ => 208,
	};

	public bool HasCover => Cover is not null;
}

/// <summary>中间库的一块（父分组下的子分组）。</summary>
public class LibrarySectionModel
{
	public string Title { get; init; } = string.Empty;

	public Guid GroupId { get; init; }

	public ObservableCollection<ItemCardModel> Items { get; } = [];
}

/// <summary>侧栏标签芯片。</summary>
public class TagChipModel : BindableBase
{
	public string Tag { get; init; } = string.Empty;

	public bool IsOn
	{
		get => GetProperty(() => IsOn);
		set => SetProperty(() => IsOn, value);
	}
}

/// <summary>下拉选项。</summary>
public class NamedOption
{
	public string Label { get; init; } = string.Empty;

	public object Value { get; init; } = string.Empty;
}

/// <summary>新建分组时「放在」下拉。</summary>
public class GroupPickOption
{
	public Guid? Id { get; init; }

	public Guid GroupId => Id ?? Guid.Empty;

	public string Label { get; init; } = string.Empty;

	/// <summary>下拉框无 DisplayMemberPath 时回退显示名称。</summary>
	public override string ToString() => Label;
}
