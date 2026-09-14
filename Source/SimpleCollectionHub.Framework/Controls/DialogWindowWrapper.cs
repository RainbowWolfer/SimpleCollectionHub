using DevExpress.Mvvm;
using SimpleCollectionHub.Framework.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SimpleCollectionHub.Framework.Controls;

public class DialogWindowWrapper : ContentControl
{

	static DialogWindowWrapper()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogWindowWrapper), new FrameworkPropertyMetadata(typeof(DialogWindowWrapper)));
	}


	public Window Window
	{
		get => (Window)GetValue(WindowProperty);
		set => SetValue(WindowProperty, value);
	}

	public IDialogViewModel DialogViewModel
	{
		get => (IDialogViewModel)GetValue(DialogViewModelProperty);
		set => SetValue(DialogViewModelProperty, value);
	}

	public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
		nameof(Window),
		typeof(Window),
		typeof(DialogWindowWrapper),
		new PropertyMetadata(null)
	);

	public static readonly DependencyProperty DialogViewModelProperty = DependencyProperty.Register(
		nameof(DialogViewModel),
		typeof(IDialogViewModel),
		typeof(DialogWindowWrapper),
		new PropertyMetadata(null, OnDialogViewModelChanged)
	);

	private static void OnDialogViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((DialogWindowWrapper)d).OnDialogViewModelChanged();
	}

	public IEnumerable<DialogCommand> DialogCommands
	{
		get => (IEnumerable<DialogCommand>)GetValue(DialogCommandsProperty);
		private set => SetValue(DialogCommandsPropertyKey, value);
	}

	private static readonly DependencyPropertyKey DialogCommandsPropertyKey = DependencyProperty.RegisterReadOnly(
		nameof(DialogCommands),
		typeof(IEnumerable<DialogCommand>),
		typeof(DialogWindowWrapper),
		new PropertyMetadata(null)
	);

	public static readonly DependencyProperty DialogCommandsProperty = DialogCommandsPropertyKey.DependencyProperty;



	public DialogCommand ResultDialogCommand
	{
		get => (DialogCommand)GetValue(ResultDialogCommandProperty);
		private set => SetValue(ResultDialogCommandPropertyKey, value);
	}

	private static readonly DependencyPropertyKey ResultDialogCommandPropertyKey = DependencyProperty.RegisterReadOnly(
		nameof(ResultDialogCommand),
		typeof(DialogCommand),
		typeof(DialogWindowWrapper),
		new PropertyMetadata(null)
	);

	public static readonly DependencyProperty ResultDialogCommandProperty = ResultDialogCommandPropertyKey.DependencyProperty;





	public bool ShowDialogWindow
	{
		get => (bool)GetValue(ShowDialogWindowProperty);
		set => SetValue(ShowDialogWindowProperty, value);
	}

	public static readonly DependencyProperty ShowDialogWindowProperty = DependencyProperty.Register(
		nameof(ShowDialogWindow),
		typeof(bool),
		typeof(DialogWindowWrapper),
		new PropertyMetadata(null)
	);

	public DialogWindowWrapper()
	{
		
	}

	private void OnDialogViewModelChanged()
	{
		DialogCommands = DialogViewModel?.DialogCommands ?? [];
	}



	private DelegateCommand<DialogCommand>? dialogButtonCommand;
	public IDelegateCommand DialogButtonCommand => dialogButtonCommand ??= new(ExecuteCommand, CanDialogButton);

	private bool CanDialogButton(DialogCommand command) => command != null;

	/// <summary>执行指定对话框命令，走完整的 HandleDialogCommand → 关闭窗口流程。</summary>
	public void ExecuteCommand(DialogCommand command)
	{
		if (!CanDialogButton(command))
		{
			return;
		}

		DialogCommandResult? result = DialogViewModel.HandleDialogCommand(command);
		if (result is null)
		{
			return;
		}
		if (result.CloseWindow)
		{
			ResultDialogCommand = command;
			if (ShowDialogWindow)
			{
				Window.DialogResult = result.DialogResultFlag;
			}
			Window.Close();
			DialogViewModel.OnWindowClosed();
		}
	}


}


public class DialogWindowWrapperButtonTemplateSelector : DataTemplateSelector
{
	public DataTemplate? Default { get; set; }
	public DataTemplate? Primary { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is DialogCommand dialogCommand && dialogCommand.IsPrimary)
		{
			return Primary;
		}
		return Default;
	}
}