using System.Windows;
using System.Windows.Input;

namespace SimpleCollectionHub.Views;

public partial class AppSplashScreen : Window
{
	public AppSplashScreen()
	{
		InitializeComponent();
	}

	private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		// 确保是鼠标左键按下，防止引发异常
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			// 调用 WPF 窗体自带的拖拽移动方法
			DragMove();
		}
	}
}
