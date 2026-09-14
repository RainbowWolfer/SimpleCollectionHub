using DevExpress.Mvvm;
using SimpleCollectionHub.Framework.Database;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace SimpleCollectionHub.Framework.Models;

/// <summary>详情里一块可编辑字段。</summary>
public class FieldEditorModel : BindableBase
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string Type { get; init; } = FieldTypes.Text;

	public string Label
	{
		get => GetProperty(() => Label);
		set => SetProperty(() => Label, value);
	}

	public string TextValue
	{
		get => GetProperty(() => TextValue);
		set => SetProperty(() => TextValue, value);
	}

	public bool IsImages => Type == FieldTypes.Images;

	public bool IsText => Type == FieldTypes.Text;

	public bool IsLinks => Type == FieldTypes.Links;

	public string TypeLabel => Type switch
	{
		FieldTypes.Images => "图片",
		FieldTypes.Links => "链接",
		_ => "文字",
	};

	public ObservableCollection<ImageEditorModel> Images { get; } = [];

	public ObservableCollection<LinkEditorModel> Links { get; } = [];
}

/// <summary>详情里一张已入库的图。</summary>
public class ImageEditorModel
{
	public Guid Id { get; init; }

	public string RelativePath { get; init; } = string.Empty;

	public string Caption { get; init; } = string.Empty;

	public ImageSource Preview { get; init; }
}

/// <summary>详情里一条链接。</summary>
public class LinkEditorModel : BindableBase
{
	public string Title
	{
		get => GetProperty(() => Title);
		set => SetProperty(() => Title, value);
	}

	public string Url
	{
		get => GetProperty(() => Url);
		set => SetProperty(() => Url, value);
	}
}

/// <summary>面包屑一段。</summary>
public class CrumbModel
{
	public Guid? GroupId { get; init; }

	public string Name { get; init; } = string.Empty;

	public bool IsCurrent { get; init; }
}
