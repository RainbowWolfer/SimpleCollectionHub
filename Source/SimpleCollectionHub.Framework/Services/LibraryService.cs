using FreeSql;
using RW.Base.WPF.DependencyInjections;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCollectionHub.Framework.Services;

/// <summary>收藏库读写。所有写入只针对 LocalAppData 里的 SQLite / Media。</summary>
public interface ILibraryService
{
	/// <summary>全部组，按 SortOrder 再按名称。</summary>
	Task<List<CollectionGroup>> GetGroupsAsync();

	/// <summary>全部条目。</summary>
	Task<List<CollectionItem>> GetItemsAsync();

	/// <summary>某条的自定义字段，按 SortOrder。</summary>
	Task<List<CollectionField>> GetFieldsAsync(Guid itemId);

	/// <summary>全部字段，用于筛选和封面。</summary>
	Task<List<CollectionField>> GetAllFieldsAsync();

	/// <summary>新建分组。</summary>
	Task<CollectionGroup> AddGroupAsync(string name, Guid? parentId, string kind, string icon, string iconColor);

	/// <summary>更新分组名称 / 图标 / 父级等。</summary>
	Task UpdateGroupAsync(CollectionGroup group);

	/// <summary>删除分组。有子分组时失败。条目可上移到父级。</summary>
	Task DeleteGroupAsync(Guid id, bool moveItemsToParent);

	/// <summary>绑定已有路径为新条目，并带上默认字段。</summary>
	Task<CollectionItem> AddItemAsync(string name, string path, Guid groupId);

	/// <summary>更新条目（名称、分组、标签）。不改磁盘。</summary>
	Task UpdateItemAsync(CollectionItem item);

	/// <summary>从库移除条目和字段，可选删除已导入图片。</summary>
	Task DeleteItemAsync(Guid id, bool deleteImportedImages);

	/// <summary>整表替换某条的字段。</summary>
	Task SaveFieldsAsync(Guid itemId, IReadOnlyList<CollectionField> fields);
}

/// <summary>FreeSql 实现。删除条目时绝不删除 Path 指向的原文件。</summary>
internal class LibraryService(
	IFreeSql freeSql,
	IMediaStore mediaStore
) : ILibraryService, ISingletonDependency
{
	public async Task<List<CollectionGroup>> GetGroupsAsync()
	{
		List<CollectionGroup> list = await freeSql.Select<CollectionGroup>()
			.OrderBy(g => g.SortOrder)
			.OrderBy(g => g.Name)
			.ToListAsync();
		return list;
	}

	public async Task<List<CollectionItem>> GetItemsAsync()
	{
		List<CollectionItem> list = await freeSql.Select<CollectionItem>()
			.OrderBy(i => i.AddedAt)
			.ToListAsync();
		foreach (CollectionItem item in list)
		{
			item.Tags ??= [];
		}

		return list;
	}

	public async Task<List<CollectionField>> GetFieldsAsync(Guid itemId)
	{
		List<CollectionField> list = await freeSql.Select<CollectionField>()
			.Where(f => f.ItemId == itemId)
			.OrderBy(f => f.SortOrder)
			.ToListAsync();
		foreach (CollectionField field in list)
		{
			field.Images ??= [];
			field.Links ??= [];
			field.TextValue ??= string.Empty;
		}

		return list;
	}

	public async Task<List<CollectionField>> GetAllFieldsAsync()
	{
		List<CollectionField> list = await freeSql.Select<CollectionField>()
			.OrderBy(f => f.ItemId)
			.OrderBy(f => f.SortOrder)
			.ToListAsync();
		foreach (CollectionField field in list)
		{
			field.Images ??= [];
			field.Links ??= [];
			field.TextValue ??= string.Empty;
		}

		return list;
	}

	public async Task<CollectionGroup> AddGroupAsync(
		string name,
		Guid? parentId,
		string kind,
		string icon,
		string iconColor
	)
	{
		int maxOrder = 0;
		if (await freeSql.Select<CollectionGroup>().Where(g => g.ParentId == parentId).AnyAsync())
		{
			maxOrder = await freeSql.Select<CollectionGroup>()
				.Where(g => g.ParentId == parentId)
				.MaxAsync(g => g.SortOrder);
		}

		CollectionGroup group = new()
		{
			Name = name.Trim(),
			ParentId = parentId,
			Kind = kind.NotBlankCheck() ?? GroupKinds.Games,
			Icon = icon.NotBlankCheck() ?? "E8B7",
			IconColor = iconColor.NotBlankCheck() ?? "#5c5c5c",
			SortOrder = maxOrder + 1,
		};
		await freeSql.Insert(group).ExecuteAffrowsAsync();
		return group;
	}

	public async Task UpdateGroupAsync(CollectionGroup group)
	{
		await freeSql.Update<CollectionGroup>()
			.SetSource(group)
			.ExecuteAffrowsAsync();
	}

	public async Task DeleteGroupAsync(Guid id, bool moveItemsToParent)
	{
		CollectionGroup group = await freeSql.Select<CollectionGroup>().Where(g => g.Id == id).FirstAsync();
		if (group is null)
		{
			return;
		}

		bool hasChildren = await freeSql.Select<CollectionGroup>().Where(g => g.ParentId == id).AnyAsync();
		if (hasChildren)
		{
			throw new InvalidOperationException("先删掉或移走子分组");
		}

		List<CollectionItem> owned = await freeSql.Select<CollectionItem>().Where(i => i.GroupId == id).ToListAsync();
		if (owned.IsNotEmpty())
		{
			if (!moveItemsToParent)
			{
				throw new InvalidOperationException("分组里还有条目");
			}

			if (group.ParentId is null)
			{
				throw new InvalidOperationException("顶级分组里还有条目，先移走再删");
			}

			foreach (CollectionItem item in owned)
			{
				item.GroupId = group.ParentId.Value;
				await freeSql.Update<CollectionItem>().SetSource(item).ExecuteAffrowsAsync();
			}
		}

		await freeSql.Delete<CollectionGroup>().Where(g => g.Id == id).ExecuteAffrowsAsync();
	}

	public async Task<CollectionItem> AddItemAsync(string name, string path, Guid groupId)
	{
		CollectionItem item = new()
		{
			GroupId = groupId,
			Name = name.Trim(),
			Path = path.Trim(),
			AddedAt = DateTime.Now,
			Tags = [],
		};
		await freeSql.Insert(item).ExecuteAffrowsAsync();

		List<CollectionField> fields =
		[
			NewField(item.Id, FieldTypes.Images, "封面", 0),
			NewField(item.Id, FieldTypes.Text, "备注", 1),
			NewField(item.Id, FieldTypes.Links, "相关链接", 2),
		];
		await freeSql.Insert(fields).ExecuteAffrowsAsync();
		return item;
	}

	public async Task UpdateItemAsync(CollectionItem item)
	{
		item.Tags ??= [];
		await freeSql.Update<CollectionItem>().SetSource(item).ExecuteAffrowsAsync();
	}

	public async Task DeleteItemAsync(Guid id, bool deleteImportedImages)
	{
		List<CollectionField> fields = await GetFieldsAsync(id);
		if (deleteImportedImages)
		{
			foreach (CollectionField field in fields)
			{
				foreach (StoredImage image in field.Images ?? [])
				{
					mediaStore.TryDelete(image.RelativePath);
				}
			}
		}

		await freeSql.Delete<CollectionField>().Where(f => f.ItemId == id).ExecuteAffrowsAsync();
		await freeSql.Delete<CollectionItem>().Where(i => i.Id == id).ExecuteAffrowsAsync();
	}

	public async Task SaveFieldsAsync(Guid itemId, IReadOnlyList<CollectionField> fields)
	{
		await freeSql.Delete<CollectionField>().Where(f => f.ItemId == itemId).ExecuteAffrowsAsync();
		int order = 0;
		foreach (CollectionField field in fields)
		{
			field.Id = field.Id == Guid.Empty ? Guid.NewGuid() : field.Id;
			field.ItemId = itemId;
			field.SortOrder = order++;
			field.Images ??= [];
			field.Links ??= [];
			field.TextValue ??= string.Empty;
		}

		if (fields.IsNotEmpty())
		{
			await freeSql.Insert(fields.ToList()).ExecuteAffrowsAsync();
		}

		await Task.CompletedTask;
	}

	private static CollectionField NewField(Guid itemId, string type, string label, int order)
	{
		return new CollectionField
		{
			ItemId = itemId,
			Type = type,
			Label = label,
			SortOrder = order,
			TextValue = string.Empty,
			Images = [],
			Links = [],
		};
	}
}
