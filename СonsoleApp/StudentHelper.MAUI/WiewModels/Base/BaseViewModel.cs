using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _title = string.Empty;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string? propertyName = null,
            Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual Task InitializeAsync(object? parameter = null)
        {
            return Task.CompletedTask;
        }

        // Методи для створення команд
        protected ICommand CreateCommand(Action execute, Func<bool>? canExecute = null)
        {
            return new RelayCommand(execute, canExecute);
        }

        protected ICommand CreateCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            return new AsyncRelayCommand(execute, () => !IsBusy && (canExecute?.Invoke() ?? true));
        }

        protected ICommand CreateCommand<T>(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            return new RelayCommand<T>(execute, canExecute);
        }

        protected ICommand CreateCommand<T>(Func<T, Task> execute, Func<T, bool>? canExecute = null)
        {
            return new AsyncRelayCommand<T>(execute, (param) => !IsBusy && (canExecute?.Invoke(param) ?? true));
        }
    }
}