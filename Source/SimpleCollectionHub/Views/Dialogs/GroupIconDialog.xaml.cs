using DevExpress.Mvvm;
using RW.Common.Helpers;
using SimpleCollectionHub.Framework.Helpers;
using SimpleCollectionHub.Framework.Services;
using SimpleCollectionHub.Framework.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleCollectionHub.Views.Dialogs;

public partial class GroupIconDialog : UserControl
{
	public GroupIconDialog()
	{
		InitializeComponent();
	}
}

internal record GroupIconDialogParameter(Guid GroupId, string Name, string Icon, string IconColor)
{
	public string ResultIcon { get; set; } = "E8B7";

	public string ResultIconColor { get; set; } = "#5c5c5c";
}

internal class IconPickItem : BindableBase
{
	public FluentIconItem Source { get; init; }

	public string Code => Source.Code;

	public string Name => Source.Name;

	public string Glyph => Source.Glyph;

	public bool IsSelected
	{
		get => GetProperty(() => IsSelected);
		set => SetProperty(() => IsSelected, value);
	}
}

internal class ColorPickItem : BindableBase
{
	public string Hex { get; init; } = "#5c5c5c";

	public Brush Brush => FluentIconHelper.ToBrush(Hex);

	public bool IsSelected
	{
		get => GetProperty(() => IsSelected);
		set => SetProperty(() => IsSelected, value);
	}
}

/// <summary>精选 Segoe Fluent Icons + 色板。全部收藏写设置，其余写分组表。</summary>
internal class GroupIconDialogViewModel(IFluentIconCatalog catalog) : DialogViewModelOkCancel<GroupIconDialogParameter>
{
	public ObservableCollection<IconPickItem> Icons { get; } = [];

	public ObservableCollection<ColorPickItem> Colors { get; } = [];

	public string TargetName
	{
		get => GetProperty(() => TargetName);
		private set => SetProperty(() => TargetName, value);
	}

	public string SearchText
	{
		get => GetProperty(() => SearchText);
		set
		{
			if (SetProperty(() => SearchText, value))
			{
				RebuildIcons();
			}
		}
	}

	public string SelectedCode
	{
		get => GetProperty(() => SelectedCode);
		private set
		{
			SetProperty(() => SelectedCode, value);
			RaisePropertyChanged(() => PreviewGlyph);
		}
	}

	public string SelectedColor
	{
		get => GetProperty(() => SelectedColor);
		set
		{
			SetProperty(() => SelectedColor, value);
			RaisePropertyChanged(() => PreviewBrush);
			MarkColors();
		}
	}

	public string CustomColor
	{
		get => GetProperty(() => CustomColor);
		set => SetProperty(() => CustomColor, value);
	}

	public string PreviewGlyph => FluentIconHelper.ToGlyph(SelectedCode);

	public Brush PreviewBrush => FluentIconHelper.ToBrush(SelectedColor);

	protected override void OnInitialized()
	{
		DialogTitle = "设置图标";
		ConfirmDialogCommand.Content = "确定";
		CancelDialogCommand.Content = "取消";
		TargetName = Parameter.Name;
		SelectedCode = Parameter.Icon.NotBlankCheck() ?? "E8B7";
		SelectedColor = Parameter.IconColor.NotBlankCheck() ?? "#326cf3";
		CustomColor = SelectedColor;
		foreach (string hex in catalog.Colors)
		{
			Colors.Add(new ColorPickItem { Hex = hex });
		}

		RebuildIcons();
		MarkColors();
	}

	public IDelegateCommand SelectIconCommand => field ??= new DelegateCommand<IconPickItem>(SelectIcon);
	private void SelectIcon(IconPickItem item)
	{
		if (item is null)
		{
			return;
		}

		SelectedCode = item.Code;
		foreach (IconPickItem icon in Icons)
		{
			icon.IsSelected = icon.Code == SelectedCode;
		}
	}

	public IDelegateCommand SelectColorCommand => field ??= new DelegateCommand<ColorPickItem>(SelectColor);
	private void SelectColor(ColorPickItem item)
	{
		if (item is null)
		{
			return;
		}

		SelectedColor = item.Hex;
		CustomColor = item.Hex;
	}

	public IDelegateCommand ApplyCustomColorCommand => field ??= new DelegateCommand(ApplyCustomColor);
	private void ApplyCustomColor()
	{
		string hex = CustomColor?.Trim();
		if (hex.IsBlank())
		{
			return;
		}

		if (!hex.StartsWith('#'))
		{
			hex = $"#{hex}";
		}

		SelectedColor = hex;
	}

	protected override bool OnConfirmed()
	{
		Parameter.ResultIcon = SelectedCode;
		Parameter.ResultIconColor = SelectedColor;
		return true;
	}

	protected override void ShowErrorDialog(string message)
	{
		MessageBoxService.ShowError(message);
	}

	private void RebuildIcons()
	{
		Icons.Clear();
		foreach (FluentIconItem item in catalog.Search(SearchText))
		{
			Icons.Add(new IconPickItem
			{
				Source = item,
				IsSelected = item.Code == SelectedCode,
			});
		}
	}

	private void MarkColors()
	{
		foreach (ColorPickItem item in Colors)
		{
			item.IsSelected = string.Equals(item.Hex, SelectedColor, StringComparison.OrdinalIgnoreCase);
		}
	}
}
