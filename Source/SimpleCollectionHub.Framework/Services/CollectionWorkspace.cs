using DevExpress.Mvvm;
using RW.Base.WPF.DependencyInjections;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Database;
using SimpleCollectionHub.Framework.Helpers;
using SimpleCollectionHub.Framework.Models;
using SimpleCollectionHub.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SimpleCollectionHub.Framework.Services;

/// <summary>主界面共享状态：树、筛选、当前条目。筛选只改中间列表。</summary>
public interface ICollectionWorkspace
{
	/// <summary>读库覆盖层。</summary>
	LoadingStatus LoadingStatus { get; }

	/// <summary>树根：全部收藏 + 顶级分组。</summary>
	ObservableCollection<GroupNode> TreeRoots { get; }

	/// <summary>侧栏标签芯片。</summary>
	ObservableCollection<TagChipModel> TagChips { get; }

	/// <summary>卡片模式下的分块。</summary>
	ObservableCollection<LibrarySectionModel> Sections { get; }

	/// <summary>列表模式或叶子分组的扁平列表。</summary>
	ObservableCollection<ItemCardModel> FlatItems { get; }

	/// <summary>面包屑。</summary>
	ObservableCollection<CrumbModel> Crumbs { get; }

	/// <summary>当前详情字段。</summary>
	ObservableCollection<FieldEditorModel> DetailFields { get; }

	/// <summary>详情标签。</summary>
	ObservableCollection<string> DetailTags { get; }

	/// <summary>详情里可选的全部分组。</summary>
	ObservableCollection<GroupPickOption> GroupPicks { get; }

	Guid SelectedGroupId { get; set; }

	ItemCardModel SelectedItem { get; set; }

	string Query { get; set; }

	IncompleteFilter Incomplete { get; set; }

	ItemSortMode SortBy { get; set; }

	LibraryViewMode ViewMode { get; set; }

	bool IsEditing { get; set; }

	string PageTitle { get; }

	string PageCountText { get; }

	bool HasSidebarFilter { get; }

	bool ShowSections { get; }

	bool ShowFlat { get; }

	bool ShowEmpty { get; }

	/// <summary>是否选中了条目（右侧详情可见）。</summary>
	bool HasDetail { get; }

	/// <summary>空列表主标题。有条目时为空。</summary>
	string EmptyTitle { get; }

	/// <summary>空列表说明。有条目时为空。</summary>
	string EmptyHint { get; }

	/// <summary>当前是卡片墙。</summary>
	bool IsCardsView { get; }

	/// <summary>当前是列表。</summary>
	bool IsListView { get; }

	string ToastMessage { get; }

	bool IsToastVisible { get; }

	string DetailName { get; set; }

	string DetailPath { get; }

	bool DetailExists { get; }

	Guid DetailGroupId { get; set; }

	string NewTagText { get; set; }

	string NewLinkTitle { get; set; }

	string NewLinkUrl { get; set; }

	ImageSource LightboxImage { get; }

	string LightboxCaption { get; }

	bool IsLightboxVisible { get; }

	/// <summary>打开图片灯箱。只读库内副本。</summary>
	void OpenLightbox(ImageSource image, string caption);

	/// <summary>关闭灯箱。</summary>
	void CloseLightbox();

	/// <summary>从 SQLite 重载分组和条目，保留展开与选中。</summary>
	Task ReloadAsync();

	/// <summary>只按当前筛选重算中间列表，树计数不变。</summary>
	void RefreshVisible();

	void ShowToast(string message);

	void ClearSidebarFilters();

	void ToggleTag(string tag);

	GroupNode FindNode(Guid id);

	CollectionGroup FindGroup(Guid id);

	/// <summary>内存快照里的条目。找不到时返回 null。</summary>
	CollectionItem FindItem(Guid id);

	string ResolveKind(CollectionGroup group);

	IReadOnlyList<CollectionField> FieldsOf(Guid itemId);
}

/// <summary>
/// 内存里的库快照。树始终用全量分组；Query / 标签 / 完整度 / 排序只喂给中间列表。
/// </summary>
internal class CollectionWorkspace(
	ILibraryService libraryService,
	IPathProbeService pathProbe,
	IMediaStore mediaStore,
	IAppSettingsService settingsService
) : BindableBase, ICollectionWorkspace, ISingletonDependency
{
	private readonly HashSet<Guid> _expanded = [];
	private readonly Dictionary<Guid, List<CollectionField>> _fields = [];
	private List<CollectionGroup> _groups = [];
	private List<CollectionItem> _items = [];
	private DispatcherTimer _toastTimer;
	private bool _firstTree = true;

	public LoadingStatus LoadingStatus { get; } = new();

	public ObservableCollection<GroupNode> TreeRoots { get; } = [];

	public ObservableCollection<TagChipModel> TagChips { get; } = [];

	public ObservableCollection<LibrarySectionModel> Sections { get; } = [];

	public ObservableCollection<ItemCardModel> FlatItems { get; } = [];

	public ObservableCollection<CrumbModel> Crumbs { get; } = [];

	public ObservableCollection<FieldEditorModel> DetailFields { get; } = [];

	public ObservableCollection<string> DetailTags { get; } = [];

	public ObservableCollection<GroupPickOption> GroupPicks { get; } = [];

	public Guid SelectedGroupId
	{
		get => GetProperty(() => SelectedGroupId);
		set
		{
			if (SetProperty(() => SelectedGroupId, value))
			{
				SelectedItem = null;
				IsEditing = false;
				RefreshVisible();
			}
		}
	}

	public ItemCardModel SelectedItem
	{
		get => GetProperty(() => SelectedItem);
		set
		{
			if (SelectedItem is not null)
			{
				SelectedItem.IsSelected = false;
			}

			SetProperty(() => SelectedItem, value);
			if (value is not null)
			{
				value.IsSelected = true;
			}

			RaisePropertyChanged(() => HasDetail);
			LoadDetail();
		}
	}

	public string Query
	{
		get => GetProperty(() => Query) ?? string.Empty;
		set
		{
			if (SetProperty(() => Query, value ?? string.Empty))
			{
				RefreshVisible();
				RaisePropertyChanged(() => HasSidebarFilter);
			}
		}
	}

	public IncompleteFilter Incomplete
	{
		get => GetProperty(() => Incomplete);
		set
		{
			if (SetProperty(() => Incomplete, value))
			{
				RefreshVisible();
			}
		}
	}

	public ItemSortMode SortBy
	{
		get => GetProperty(() => SortBy);
		set
		{
			if (SetProperty(() => SortBy, value))
			{
				RefreshVisible();
			}
		}
	}

	public LibraryViewMode ViewMode
	{
		get => GetProperty(() => ViewMode);
		set
		{
			if (SetProperty(() => ViewMode, value))
			{
				RaisePropertyChanged(() => IsCardsView);
				RaisePropertyChanged(() => IsListView);
				RefreshVisible();
			}
		}
	}

	public bool IsEditing
	{
		get => GetProperty(() => IsEditing);
		set => SetProperty(() => IsEditing, value);
	}

	public string PageTitle
	{
		get => GetProperty(() => PageTitle);
		private set => SetProperty(() => PageTitle, value);
	}

	public string PageCountText
	{
		get => GetProperty(() => PageCountText);
		private set => SetProperty(() => PageCountText, value);
	}

	public bool HasSidebarFilter => Query.IsNotBlank() || TagChips.Any(c => c.IsOn);

	public bool ShowSections
	{
		get => GetProperty(() => ShowSections);
		private set => SetProperty(() => ShowSections, value);
	}

	public bool ShowFlat
	{
		get => GetProperty(() => ShowFlat);
		private set => SetProperty(() => ShowFlat, value);
	}

	public bool ShowEmpty
	{
		get => GetProperty(() => ShowEmpty);
		private set => SetProperty(() => ShowEmpty, value);
	}

	public bool HasDetail => SelectedItem is not null;

	public string ToastMessage
	{
		get => GetProperty(() => ToastMessage);
		private set => SetProperty(() => ToastMessage, value);
	}

	public bool IsToastVisible
	{
		get => GetProperty(() => IsToastVisible);
		private set => SetProperty(() => IsToastVisible, value);
	}

	public bool IsCardsView => ViewMode == LibraryViewMode.Cards;

	public bool IsListView => ViewMode == LibraryViewMode.List;

	public string EmptyTitle
	{
		get => GetProperty(() => EmptyTitle);
		private set => SetProperty(() => EmptyTitle, value);
	}

	public string EmptyHint
	{
		get => GetProperty(() => EmptyHint);
		private set => SetProperty(() => EmptyHint, value);
	}

	public string DetailName
	{
		get => GetProperty(() => DetailName);
		set => SetProperty(() => DetailName, value);
	}

	public string DetailPath
	{
		get => GetProperty(() => DetailPath);
		private set => SetProperty(() => DetailPath, value);
	}

	public bool DetailExists
	{
		get => GetProperty(() => DetailExists);
		private set => SetProperty(() => DetailExists, value);
	}

	public Guid DetailGroupId
	{
		get => GetProperty(() => DetailGroupId);
		set => SetProperty(() => DetailGroupId, value);
	}

	public string NewTagText
	{
		get => GetProperty(() => NewTagText);
		set => SetProperty(() => NewTagText, value);
	}

	public string NewLinkTitle
	{
		get => GetProperty(() => NewLinkTitle);
		set => SetProperty(() => NewLinkTitle, value);
	}

	public string NewLinkUrl
	{
		get => GetProperty(() => NewLinkUrl);
		set => SetProperty(() => NewLinkUrl, value);
	}

	public ImageSource LightboxImage
	{
		get => GetProperty(() => LightboxImage);
		private set => SetProperty(() => LightboxImage, value);
	}

	public string LightboxCaption
	{
		get => GetProperty(() => LightboxCaption);
		private set => SetProperty(() => LightboxCaption, value);
	}

	public bool IsLightboxVisible
	{
		get => GetProperty(() => IsLightboxVisible);
		private set => SetProperty(() => IsLightboxVisible, value);
	}

	public void CloseLightbox()
	{
		IsLightboxVisible = false;
		LightboxImage = null;
		LightboxCaption = string.Empty;
	}

	public void OpenLightbox(ImageSource image, string caption)
	{
		LightboxImage = image;
		LightboxCaption = caption ?? string.Empty;
		IsLightboxVisible = image is not null;
	}

	public async Task ReloadAsync()
	{
		try
		{
			LoadingStatus.Initialize("读取收藏库...");
			_groups = await libraryService.GetGroupsAsync();
			_items = await libraryService.GetItemsAsync();
			_fields.Clear();
			foreach (CollectionField field in await libraryService.GetAllFieldsAsync())
			{
				if (!_fields.TryGetValue(field.ItemId, out List<CollectionField> list))
				{
					list = [];
					_fields[field.ItemId] = list;
				}

				list.Add(field);
			}

			RebuildGroupPicks();
			RebuildTree();
			RebuildTags();
			Guid keepItem = SelectedItem?.Id ?? Guid.Empty;
			RefreshVisible();
			SelectedItem = keepItem == Guid.Empty ? null : FindCard(keepItem);
			LoadDetail();
			LoadingStatus.Done();
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			LoadingStatus.Error(ex.Message);
		}
	}

	public void RefreshVisible()
	{
		MarkTreeSelection();
		List<ItemCardModel> cards = BuildFilteredCards();
		int totalInScope = CountInScope();
		GroupNode current = FindNode(SelectedGroupId);
		PageTitle = current?.Name ?? "全部收藏";
		bool filtering = HasSidebarFilter || Incomplete != IncompleteFilter.All;
		PageCountText = filtering ? $"{cards.Count} / {totalInScope} 项" : $"{cards.Count} 项";
		RebuildCrumbs();

		Sections.Clear();
		FlatItems.Clear();

		bool isAll = SelectedGroupId == Guid.Empty;
		List<CollectionGroup> kids = ChildrenOf(isAll ? null : SelectedGroupId);
		bool useSections = ViewMode == LibraryViewMode.Cards && kids.Count > 0;

		if (ViewMode == LibraryViewMode.List || !useSections)
		{
			foreach (ItemCardModel card in cards)
			{
				FlatItems.Add(card);
			}

			ShowSections = false;
			ShowFlat = cards.Count > 0;
			ShowEmpty = cards.Count == 0;
			UpdateEmptyCopy(filtering, cards.Count);
			RaisePropertyChanged(() => HasSidebarFilter);
			return;
		}

		if (isAll)
		{
			List<ItemCardModel> ungrouped = [.. cards.Where(c => c.GroupId == Guid.Empty)];
			if (ungrouped.IsNotEmpty())
			{
				AddSection("未分组", Guid.Empty, ungrouped);
			}

			foreach (CollectionGroup root in ChildrenOf(null))
			{
				List<CollectionGroup> rootKids = ChildrenOf(root.Id);
				if (rootKids.IsEmpty())
				{
					AddSection(root.Name, root.Id, cards);
					continue;
				}

				List<ItemCardModel> direct = [.. cards.Where(c => c.GroupId == root.Id)];
				if (direct.IsNotEmpty())
				{
					AddSection(root.Name, root.Id, direct);
				}

				foreach (CollectionGroup child in rootKids)
				{
					AddSection($"{root.Name} / {child.Name}", child.Id, cards.Where(c => InSubtree(c.GroupId, child.Id)));
				}
			}
		}
		else
		{
			List<ItemCardModel> direct = [.. cards.Where(c => c.GroupId == SelectedGroupId)];
			if (direct.IsNotEmpty())
			{
				AddSection("本组", SelectedGroupId, direct);
			}

			foreach (CollectionGroup child in kids)
			{
				List<ItemCardModel> subset = [.. cards.Where(c => InSubtree(c.GroupId, child.Id))];
				if (subset.IsNotEmpty())
				{
					AddSection(child.Name, child.Id, subset);
				}
				else if (!filtering)
				{
					AddSection(child.Name, child.Id, []);
				}
			}
		}

		ShowSections = Sections.Count > 0;
		ShowFlat = false;
		ShowEmpty = Sections.Count == 0;
		UpdateEmptyCopy(filtering, cards.Count);
		RaisePropertyChanged(() => HasSidebarFilter);
	}

	public void ShowToast(string message)
	{
		ToastMessage = message;
		IsToastVisible = true;
		_toastTimer ??= new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(2400),
		};
		_toastTimer.Tick -= OnToastTick;
		_toastTimer.Tick += OnToastTick;
		_toastTimer.Stop();
		_toastTimer.Start();
	}

	public void ClearSidebarFilters()
	{
		Query = string.Empty;
		foreach (TagChipModel chip in TagChips)
		{
			chip.IsOn = false;
		}

		RaisePropertyChanged(() => HasSidebarFilter);
		RefreshVisible();
	}

	public void ToggleTag(string tag)
	{
		TagChipModel chip = TagChips.FirstOrDefault(c => c.Tag == tag);
		if (chip is null)
		{
			return;
		}

		chip.IsOn = !chip.IsOn;
		RaisePropertyChanged(() => HasSidebarFilter);
		RefreshVisible();
	}

	public GroupNode FindNode(Guid id)
	{
		return FindNode(TreeRoots, id);
	}

	public CollectionGroup FindGroup(Guid id)
	{
		return _groups.FirstOrDefault(g => g.Id == id);
	}

	public CollectionItem FindItem(Guid id)
	{
		return _items.FirstOrDefault(i => i.Id == id);
	}

	public string ResolveKind(CollectionGroup group)
	{
		CollectionGroup current = group;
		while (current is not null)
		{
			if (current.Kind.IsNotBlank())
			{
				return current.Kind;
			}

			current = current.ParentId is Guid pid ? FindGroup(pid) : null;
		}

		return GroupKinds.Games;
	}

	public IReadOnlyList<CollectionField> FieldsOf(Guid itemId)
	{
		return _fields.TryGetValue(itemId, out List<CollectionField> list) ? list : [];
	}

	private void OnToastTick(object sender, EventArgs e)
	{
		_toastTimer?.Stop();
		IsToastVisible = false;
	}

	private ItemCardModel FindCard(Guid id)
	{
		ItemCardModel flat = FlatItems.FirstOrDefault(i => i.Id == id);
		if (flat is not null)
		{
			return flat;
		}

		foreach (LibrarySectionModel section in Sections)
		{
			ItemCardModel hit = section.Items.FirstOrDefault(i => i.Id == id);
			if (hit is not null)
			{
				return hit;
			}
		}

		return null;
	}

	private void RebuildTree()
	{
		if (_firstTree)
		{
			foreach (CollectionGroup group in _groups)
			{
				if (ChildrenOf(group.Id).Count > 0)
				{
					_expanded.Add(group.Id);
				}
			}

			_firstTree = false;
		}

		CollectExpanded(TreeRoots);
		TreeRoots.Clear();
		GroupNode all = new()
		{
			Id = Guid.Empty,
			IsAll = true,
			Name = "全部收藏",
			Icon = settingsService.Model.AllIcon.NotBlankCheck() ?? "E8F1",
			IconColor = settingsService.Model.AllIconColor.NotBlankCheck() ?? "#326cf3",
			Count = _items.Count,
			IsExpanded = true,
		};
		TreeRoots.Add(all);
		foreach (CollectionGroup group in ChildrenOf(null))
		{
			TreeRoots.Add(BuildNode(group));
		}

		MarkTreeSelection();
	}

	private GroupNode BuildNode(CollectionGroup group)
	{
		GroupNode node = new()
		{
			Id = group.Id,
			ParentId = group.ParentId,
			Name = group.Name,
			Icon = group.Icon,
			IconColor = group.IconColor,
			Kind = ResolveKind(group),
			Count = CountSubtree(group.Id),
			IsExpanded = _expanded.Contains(group.Id),
		};
		foreach (CollectionGroup child in ChildrenOf(group.Id))
		{
			node.Children.Add(BuildNode(child));
		}

		return node;
	}

	private void CollectExpanded(IEnumerable<GroupNode> nodes)
	{
		foreach (GroupNode node in nodes)
		{
			if (node.IsExpanded)
			{
				_expanded.Add(node.Id);
			}
			else
			{
				_expanded.Remove(node.Id);
			}

			CollectExpanded(node.Children);
		}
	}

	private void MarkTreeSelection()
	{
		MarkTreeSelection(TreeRoots, SelectedGroupId);
	}

	private static void MarkTreeSelection(IEnumerable<GroupNode> nodes, Guid selectedId)
	{
		foreach (GroupNode node in nodes)
		{
			node.IsSelected = node.Id == selectedId;
			MarkTreeSelection(node.Children, selectedId);
		}
	}

	private static GroupNode FindNode(IEnumerable<GroupNode> nodes, Guid id)
	{
		foreach (GroupNode node in nodes)
		{
			if (node.Id == id)
			{
				return node;
			}

			GroupNode child = FindNode(node.Children, id);
			if (child is not null)
			{
				return child;
			}
		}

		return null;
	}

	private void RebuildTags()
	{
		HashSet<string> on = [.. TagChips.Where(c => c.IsOn).Select(c => c.Tag)];
		TagChips.Clear();
		foreach (string tag in _items.SelectMany(i => i.Tags ?? []).Distinct().OrderBy(t => t, StringComparer.CurrentCulture))
		{
			TagChips.Add(new TagChipModel
			{
				Tag = tag,
				IsOn = on.Contains(tag),
			});
		}
	}

	private void RebuildGroupPicks()
	{
		GroupPicks.Clear();
		GroupPicks.Add(new GroupPickOption
		{
			Id = null,
			Label = "全部收藏",
		});
		WalkPicks(null, "");
	}

	private void WalkPicks(Guid? parentId, string prefix)
	{
		foreach (CollectionGroup group in ChildrenOf(parentId))
		{
			GroupPicks.Add(new GroupPickOption
			{
				Id = group.Id,
				Label = prefix + group.Name,
			});
			WalkPicks(group.Id, prefix + "　");
		}
	}

	private List<CollectionGroup> ChildrenOf(Guid? parentId)
	{
		return [.. _groups
			.Where(g => g.ParentId == parentId)
			.OrderBy(g => g.SortOrder)
			.ThenBy(g => g.Name)];
	}

	private void AddSection(string title, Guid groupId, IEnumerable<ItemCardModel> items)
	{
		LibrarySectionModel section = new()
		{
			Title = title,
			GroupId = groupId,
		};
		foreach (ItemCardModel item in items)
		{
			section.Items.Add(item);
		}

		Sections.Add(section);
	}

	private List<ItemCardModel> BuildFilteredCards()
	{
		HashSet<Guid> scope = VisibleGroupIds();
		HashSet<string> tags = [.. TagChips.Where(c => c.IsOn).Select(c => c.Tag)];
		string q = Query?.Trim().ToLowerInvariant() ?? string.Empty;
		List<ItemCardModel> list = [];
		foreach (CollectionItem item in _items)
		{
			if (scope is not null && !scope.Contains(item.GroupId))
			{
				continue;
			}

			if (!MatchesQuery(item, q) || !MatchesTags(item, tags) || !MatchesIncomplete(item))
			{
				continue;
			}

			list.Add(ToCard(item));
		}

		list.Sort(CompareItems);
		return list;
	}

	private HashSet<Guid> VisibleGroupIds()
	{
		if (SelectedGroupId == Guid.Empty)
		{
			return null;
		}

		return [.. DescendantIds(SelectedGroupId)];
	}

	private IEnumerable<Guid> DescendantIds(Guid id)
	{
		yield return id;
		foreach (CollectionGroup child in ChildrenOf(id))
		{
			foreach (Guid nested in DescendantIds(child.Id))
			{
				yield return nested;
			}
		}
	}

	private bool InSubtree(Guid itemGroupId, Guid rootId)
	{
		return DescendantIds(rootId).Contains(itemGroupId);
	}

	private int CountSubtree(Guid id)
	{
		HashSet<Guid> ids = [.. DescendantIds(id)];
		return _items.Count(i => ids.Contains(i.GroupId));
	}

	private int CountInScope()
	{
		HashSet<Guid> scope = VisibleGroupIds();
		if (scope is null)
		{
			return _items.Count;
		}

		return _items.Count(i => scope.Contains(i.GroupId));
	}

	private bool MatchesQuery(CollectionItem item, string q)
	{
		if (q.IsBlank())
		{
			return true;
		}

		List<string> parts = [item.Name, item.Path, .. item.Tags ?? []];
		foreach (CollectionField field in FieldsOf(item.Id))
		{
			parts.Add(field.Label);
			parts.Add(field.TextValue);
			foreach (StoredLink link in field.Links ?? [])
			{
				parts.Add(link.Title);
				parts.Add(link.Url);
			}

			foreach (StoredImage image in field.Images ?? [])
			{
				parts.Add(image.Caption);
			}
		}

		return string.Join('\n', parts).ToLowerInvariant().Contains(q);
	}

	private static bool MatchesTags(CollectionItem item, HashSet<string> tags)
	{
		if (tags.Count == 0)
		{
			return true;
		}

		foreach (string tag in item.Tags ?? [])
		{
			if (tags.Contains(tag))
			{
				return true;
			}
		}

		return false;
	}

	private bool MatchesIncomplete(CollectionItem item)
	{
		List<CollectionField> fields = [.. FieldsOf(item.Id)];
		bool missingPath = !pathProbe.Exists(item.Path);
		bool missingImages = !fields.Any(f => f.Type == FieldTypes.Images && f.Images.IsNotEmpty());
		bool missingText = !fields.Any(f => f.Type == FieldTypes.Text && f.TextValue.IsNotBlank());
		bool missingLinks = !fields.Any(f => f.Type == FieldTypes.Links && f.Links.IsNotEmpty());
		return Incomplete switch
		{
			IncompleteFilter.Path => missingPath,
			IncompleteFilter.Images => missingImages,
			IncompleteFilter.Text => missingText,
			IncompleteFilter.Links => missingLinks,
			IncompleteFilter.Any => missingPath || missingImages || missingText || missingLinks,
			_ => true,
		};
	}

	private int CompareItems(ItemCardModel a, ItemCardModel b)
	{
		return SortBy switch
		{
			ItemSortMode.NameDesc => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture),
			ItemSortMode.AddedDesc => b.AddedAt.CompareTo(a.AddedAt),
			ItemSortMode.AddedAsc => a.AddedAt.CompareTo(b.AddedAt),
			_ => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture),
		};
	}

	private ItemCardModel ToCard(CollectionItem item)
	{
		CollectionGroup group = FindGroup(item.GroupId);
		List<CollectionField> fields = [.. FieldsOf(item.Id)];
		StoredImage cover = fields
			.Where(f => f.Type == FieldTypes.Images)
			.SelectMany(f => f.Images ?? [])
			.FirstOrDefault();
		ItemCardModel card = new()
		{
			Id = item.Id,
			GroupId = item.GroupId,
			Name = item.Name,
			Path = item.Path,
			Exists = pathProbe.Exists(item.Path),
			AddedAt = item.AddedAt,
			Kind = group is null ? GroupKinds.Games : ResolveKind(group),
			ImageCount = fields.Where(f => f.Type == FieldTypes.Images).Sum(f => f.Images?.Count ?? 0),
			Cover = LoadCover(cover),
			PlaceholderTitle = item.Name,
			GroupPath = group is null ? "全部收藏" : string.Join(" / ", Ancestors(item.GroupId).Select(g => g.Name)),
			IsSelected = SelectedItem?.Id == item.Id,
		};
		foreach (string tag in (item.Tags ?? []).Take(3))
		{
			card.Tags.Add(tag);
		}

		return card;
	}

	private BitmapImage LoadCover(StoredImage cover)
	{
		if (cover is null)
		{
			return null;
		}

		return mediaStore.LoadPreview(cover.RelativePath) as BitmapImage;
	}

	private List<CollectionGroup> Ancestors(Guid groupId)
	{
		List<CollectionGroup> chain = [];
		CollectionGroup current = FindGroup(groupId);
		while (current is not null)
		{
			chain.Insert(0, current);
			current = current.ParentId is Guid pid ? FindGroup(pid) : null;
		}

		return chain;
	}

	private void UpdateEmptyCopy(bool filtering, int visibleCount)
	{
		if (visibleCount > 0)
		{
			EmptyTitle = string.Empty;
			EmptyHint = string.Empty;
			return;
		}

		if (!filtering && _items.Count == 0)
		{
			EmptyTitle = "还没有条目";
			EmptyHint = string.Empty;
			return;
		}

		EmptyTitle = "没有符合条件的条目";
		EmptyHint = string.Empty;
	}

	private void RebuildCrumbs()
	{
		Crumbs.Clear();
		if (SelectedGroupId == Guid.Empty)
		{
			return;
		}

		Crumbs.Add(new CrumbModel
		{
			GroupId = Guid.Empty,
			Name = "全部收藏",
		});
		List<CollectionGroup> chain = Ancestors(SelectedGroupId);
		for (int i = 0; i < chain.Count; i++)
		{
			Crumbs.Add(new CrumbModel
			{
				GroupId = chain[i].Id,
				Name = chain[i].Name,
				IsCurrent = i == chain.Count - 1,
			});
		}
	}

	private void LoadDetail()
	{
		ItemCardModel item = SelectedItem;
		if (item is null)
		{
			DetailName = string.Empty;
			DetailPath = string.Empty;
			DetailExists = false;
			DetailGroupId = Guid.Empty;
			DetailFields.Clear();
			DetailTags.Clear();
			return;
		}

		CollectionItem raw = _items.FirstOrDefault(i => i.Id == item.Id);
		DetailName = item.Name;
		DetailPath = item.Path;
		DetailExists = item.Exists;
		DetailGroupId = item.GroupId;
		DetailTags.Clear();
		IEnumerable<string> tags = raw?.Tags is not null ? raw.Tags : item.Tags;
		foreach (string tag in tags)
		{
			DetailTags.Add(tag);
		}

		DetailFields.Clear();
		foreach (CollectionField field in FieldsOf(item.Id))
		{
			FieldEditorModel editor = new()
			{
				Id = field.Id,
				Type = field.Type,
				Label = field.Label,
				TextValue = field.TextValue ?? string.Empty,
			};
			foreach (StoredImage image in field.Images ?? [])
			{
				editor.Images.Add(new ImageEditorModel
				{
					Id = image.Id,
					RelativePath = image.RelativePath,
					Caption = image.Caption,
					Preview = LoadCover(image),
				});
			}

			foreach (StoredLink link in field.Links ?? [])
			{
				editor.Links.Add(new LinkEditorModel
				{
					Title = link.Title,
					Url = link.Url,
				});
			}

			DetailFields.Add(editor);
		}
	}
}
