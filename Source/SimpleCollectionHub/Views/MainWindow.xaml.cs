using SimpleCollectionHub.Framework.Services;
using System;
using System.Windows;

namespace SimpleCollectionHub.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
		AppTheme.ApplyTitleBar(this);
	}
}