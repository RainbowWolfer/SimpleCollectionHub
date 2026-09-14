using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SimpleCollectionHub.Framework.Database;

/// <summary>条目自定义字段的种类。分组树不使用这些类型。</summary>
public static class FieldTypes
{
	/// <summary>图片列表，文件副本在 MediaFolder。</summary>
	public const string Images = "images";

	/// <summary>多行文字。</summary>
	public const string Text = "text";

	/// <summary>标题 + URL 链接列表。</summary>
	public const string Links = "links";
}

/// <summary>顶级分组的卡片比例提示，子分组可留空并继承祖先。</summary>
public static class GroupKinds
{
	/// <summary>游戏：竖图卡片。</summary>
	public const string Games = "games";

	/// <summary>音乐：方封。</summary>
	public const string Music = "music";

	/// <summary>视频：宽图。</summary>
	public const string Video = "video";
}

/// <summary>库里保存的一张图。相对路径相对 MediaFolder，不是原目录。</summary>
public class StoredImage
{
	public Guid Id { get; set; } = Guid.NewGuid();

	/// <summary>相对 MediaFolder 的文件名，例如 {guid}.png。</summary>
	public string RelativePath { get; set; } = string.Empty;

	public string Caption { get; set; } = string.Empty;
}

/// <summary>库里保存的一条链接。</summary>
public class StoredLink
{
	public string Title { get; set; } = string.Empty;

	public string Url { get; set; } = string.Empty;
}

/// <summary>可嵌套的收藏分组。本身不是磁盘文件夹。</summary>
[FreeSql.DataAnnotations.Table(Name = "collection_group")]
public class CollectionGroup : Entity
{
	/// <summary>父分组。null 表示和「游戏」一样的顶级。</summary>
	public Guid? ParentId { get; set; }

	[FreeSql.DataAnnotations.Column(StringLength = 200, IsNullable = false)]
	public string Name { get; set; } = string.Empty;

	/// <summary>games / music / video。空则继承祖先。</summary>
	[FreeSql.DataAnnotations.Column(StringLength = 32)]
	public string Kind { get; set; } = GroupKinds.Games;

	/// <summary>Segoe Fluent Icons 码位，如 E7FC。</summary>
	[FreeSql.DataAnnotations.Column(StringLength = 8)]
	public string Icon { get; set; } = "E8B7";

	[FreeSql.DataAnnotations.Column(StringLength = 16)]
	public string IconColor { get; set; } = "#5c5c5c";

	public int SortOrder { get; set; }
}

/// <summary>绑定已有路径的一条收藏。删除只删本行，不动 Path 指向的磁盘。</summary>
[FreeSql.DataAnnotations.Table(Name = "collection_item")]
public class CollectionItem : Entity
{
	public Guid GroupId { get; set; }

	[FreeSql.DataAnnotations.Column(StringLength = 300, IsNullable = false)]
	public string Name { get; set; } = string.Empty;

	/// <summary>用户指定的已有路径。本软件永不在此路径写入。</summary>
	[FreeSql.DataAnnotations.Column(StringLength = -1, IsNullable = false)]
	public string Path { get; set; } = string.Empty;

	public DateTime AddedAt { get; set; }

	/// <summary>只用于筛选的标签，不参与分组树。</summary>
	[JsonProperty]
	[FreeSql.DataAnnotations.JsonMap]
	public List<string> Tags { get; set; } = [];
}

/// <summary>条目上的一块自定义字段。</summary>
[FreeSql.DataAnnotations.Table(Name = "collection_field")]
public class CollectionField : Entity
{
	public Guid ItemId { get; set; }

	/// <summary>见 <see cref="FieldTypes"/>。</summary>
	[FreeSql.DataAnnotations.Column(StringLength = 16, IsNullable = false)]
	public string Type { get; set; } = FieldTypes.Text;

	[FreeSql.DataAnnotations.Column(StringLength = 100, IsNullable = false)]
	public string Label { get; set; } = string.Empty;

	public int SortOrder { get; set; }

	[FreeSql.DataAnnotations.Column(StringLength = -1)]
	public string TextValue { get; set; } = string.Empty;

	[FreeSql.DataAnnotations.JsonMap]
	public List<StoredImage> Images { get; set; } = [];

	[FreeSql.DataAnnotations.JsonMap]
	public List<StoredLink> Links { get; set; } = [];
}
