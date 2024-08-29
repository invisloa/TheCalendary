using System.Windows.Input;

// Generic relay command implementation
public class RelayCommand : ICommand
{
	#region Fields

	private readonly Action _execute;
	private readonly Func<bool> _canExecute;
	private event EventHandler canExecuteChanged;

	#endregion

	#region Constructors

	public RelayCommand(Action execute, Func<bool> canExecute = null)
	{
		_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		_canExecute = canExecute;
	}

	#endregion

	#region ICommand Members

	public bool CanExecute(object parameter)
	{
		return _canExecute == null || _canExecute();
	}

	public void Execute(object parameter)
	{
		_execute();
	}

	public event EventHandler CanExecuteChanged
	{
		add { canExecuteChanged += value; }
		remove { canExecuteChanged -= value; }
	}

	#endregion

	#region Helper Methods

	public void NotifyCanExecuteChanged()
	{
		canExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
	public void RaiseCanExecuteChanged()
	{
		canExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
	#endregion
}

// Generic relay command implementation with a parameter of type T
public class RelayCommand<T> : ICommand
{
	#region Fields

	private readonly Action<T> _execute;
	private readonly Predicate<T> _canExecute;
	private event EventHandler canExecuteChanged;

	#endregion

	#region Constructors

	public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
	{
		_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		_canExecute = canExecute;
	}

	#endregion

	#region ICommand Members

	public bool CanExecute(object parameter)
	{
		return _canExecute == null || _canExecute((T)parameter);
	}

	public void Execute(object parameter)
	{
		_execute((T)parameter);
	}

	public event EventHandler CanExecuteChanged
	{
		add { canExecuteChanged += value; }
		remove { canExecuteChanged -= value; }
	}

	#endregion

	#region Helper Methods

	public void RaiseCanExecuteChanged()
	{
		canExecuteChanged?.Invoke(this, EventArgs.Empty);
	}

	#endregion
}

public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return !_isExecuting && (_canExecute == null || _canExecute());
    }

    public async void Execute(object parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T, Task> _execute;
    private readonly Predicate<T> _canExecute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return !_isExecuting && (_canExecute == null || _canExecute((T)parameter));
    }

    public async void Execute(object parameter)
    {
        if (CanExecute(parameter) && parameter is T)
        {
            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute((T)parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
