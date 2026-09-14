using DevExpress.Mvvm;
using RW.Common.Helpers;
using RW.Common.WPF.Helpers;

namespace SimpleCollectionHub.Framework.ViewModels;

public class LoadingStatus : BindableBase
{
	public double? Progress
	{
		get => GetProperty(() => Progress);
		set => SetProperty(() => Progress, value);
	}

	public string Message
	{
		get => GetProperty(() => Message);
		set => SetProperty(() => Message, value);
	}

	public string ErrorMessage
	{
		get => GetProperty(() => ErrorMessage);
		set => SetProperty(() => ErrorMessage, value);
	}

	public bool HasError
	{
		get => GetProperty(() => HasError);
		set
		{
			SetProperty(() => HasError, value);
			RaisePropertyChanged(() => IsLoadedSuccess);
			RaisePropertyChanged(() => IsLoadingActive);
		}
	}

	public bool IsLoading
	{
		get => GetProperty(() => IsLoading);
		set
		{
			SetProperty(() => IsLoading, value);
			RaisePropertyChanged(() => IsLoadedSuccess);
			RaisePropertyChanged(() => IsLoadingActive);
		}
	}

	public bool IsLoadedSuccess => !IsLoading && !HasError;
	public bool IsLoadingActive => IsLoading || HasError;

	public LoadingStatus()
	{

	}

	public void Reset()
	{
		IsLoading = false;
		Progress = null;
		HasError = false;
		ErrorMessage = string.Empty;
		Message = string.Empty;
	}


	public void Initialize() => Initialize("");

	public void Initialize(string message = "")
	{
		IsLoading = true;
		Progress = null;
		HasError = false;
		ErrorMessage = string.Empty;
		Message = message.NotBlankCheck() ?? "加载中";
	}


	public void Done() => Done("");

	public void Done(string message = "")
	{
		IsLoading = false;
		HasError = false;
		ErrorMessage = string.Empty;
		Message = message.NotBlankCheck() ?? "载入完成";
	}

	public void Error(string errorMessage)
	{
		IsLoading = false;
		HasError = true;
		ErrorMessage = errorMessage.NotBlankCheck() ?? "载入发生错误";
	}

	public void SetProgress(double progress)
	{
		IsLoading = true;
		Progress = progress;
	}

	public void CopyErrorMessage() => ErrorMessage.CopyToClipboard();
}
