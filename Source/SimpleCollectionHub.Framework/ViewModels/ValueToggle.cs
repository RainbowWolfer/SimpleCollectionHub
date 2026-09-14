using DevExpress.Mvvm;

namespace SimpleCollectionHub.Framework.ViewModels;

public class ValueToggle<T> : BindableBase
{
	public bool IsEnabled
	{
		get => GetProperty(() => IsEnabled);
		set => SetProperty(() => IsEnabled, value);
	}

	public T Value
	{
		get => GetProperty(() => Value);
		set => SetProperty(() => Value, value);
	}

}

public class ValueToggle<T1, T2> : BindableBase
{
	public bool IsEnabled
	{
		get => GetProperty(() => IsEnabled);
		set => SetProperty(() => IsEnabled, value);
	}

	public T1 Value1
	{
		get => GetProperty(() => Value1);
		set => SetProperty(() => Value1, value);
	}

	public T2 Value2
	{
		get => GetProperty(() => Value2);
		set => SetProperty(() => Value2, value);
	}

}
