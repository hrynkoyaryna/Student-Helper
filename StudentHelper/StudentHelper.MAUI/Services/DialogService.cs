// StudentHelper.MAUI/Services/DialogService.cs 
using Microsoft.Maui.Controls;

namespace StudentHelper.MAUI.Services
{
    public class DialogService : IDialogService
    {
        public async Task ShowAlertAsync(string message, string title = "Info", string buttonLabel = "OK")
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, buttonLabel);
            }
        }

        public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation")
        {
            if (Application.Current?.MainPage != null)
            {
                return await Application.Current.MainPage.DisplayAlert(title, message, "Так", "Ні");
            }
            return false;
        }

        public async Task<string?> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string? placeholder = null)
        {
            if (Application.Current?.MainPage != null)
            {
                return await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder ?? "");
            }
            return null;
        }

        // ДОДАТКОВІ МЕТОДИ ДЛЯ КОНКРЕТНИХ ВИПАДКІВ
        public async Task ShowErrorAsync(string message)
        {
            await ShowAlertAsync(message, "Помилка", "OK");
        }

        public async Task ShowSuccessAsync(string message)
        {
            await ShowAlertAsync(message, "Успіх", "OK");
        }

        public async Task<bool> ConfirmDeleteAsync(string itemName)
        {
            return await ShowConfirmationAsync($"Ви впевнені, що хочете видалити {itemName}?", "Підтвердження видалення");
        }
    }
}