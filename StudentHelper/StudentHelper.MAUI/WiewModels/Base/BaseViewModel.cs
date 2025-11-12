// StudentHelper.MAUI/ViewModels/Base/BaseViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Base;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public virtual Task InitializeAsync(object? parameter = null) => Task.CompletedTask;

    protected ICommand CreateCommand(Action execute, Func<bool> canExecute = null)
        => new RelayCommand(execute, canExecute);

    protected ICommand CreateCommand<T>(Action<T> execute, Func<T, bool> canExecute = null)
        => new RelayCommand<T>(execute, canExecute);
}