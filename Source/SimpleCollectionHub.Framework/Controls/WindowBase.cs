using SimpleCollectionHub.Framework.Services;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SimpleCollectionHub.Framework.Controls;

public class WindowBase : Window {

	public nint WindowHandle { get; private set; }
	public HwndSource? HwndSource { get; private set; }

	static WindowBase() {
		// Reuse the built-in Window theme; a custom template breaks client-area rendering.
		DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase), new FrameworkPropertyMetadata(typeof(Window)));
	}

	public Window? CenterToOwner {
		get => (Window?)GetValue(CenterToOwnerProperty);
		set => SetValue(CenterToOwnerProperty, value);
	}

	public static readonly DependencyProperty CenterToOwnerProperty = DependencyProperty.Register(
		nameof(CenterToOwner),
		typeof(Window),
		typeof(WindowBase),
		new PropertyMetadata(null)
	);

	public ResizeMode? MyResizeMode { get; set; }

	public WindowBase() {

	}

	protected override void OnMouseDown(MouseButtonEventArgs e) {
		base.OnMouseDown(e);

		if (Content is FrameworkElement frameworkElement) {
			frameworkElement.Focus();
		}
	}

	public override void OnApplyTemplate() {
		base.OnApplyTemplate();
	}

	protected override void OnSourceInitialized(EventArgs e) {
		base.OnSourceInitialized(e);

		WindowHandle = new WindowInteropHelper(this).Handle;
		HwndSource = HwndSource.FromHwnd(WindowHandle);

		if (CenterToOwner != null) {
			double ownerLeft = CenterToOwner.Left;
			double ownerTop = CenterToOwner.Top;
			double ownerWidth = CenterToOwner.ActualWidth;
			double ownerHeight = CenterToOwner.ActualHeight;

			Left = ownerLeft + ((ownerWidth - ActualWidth) / 2);
			Top = ownerTop + ((ownerHeight - ActualHeight) / 2);
		}

		if (MyResizeMode != null) {
			ResizeMode = MyResizeMode.Value;
		}

		AppTheme.ApplyTitleBar(this);
	}

}
