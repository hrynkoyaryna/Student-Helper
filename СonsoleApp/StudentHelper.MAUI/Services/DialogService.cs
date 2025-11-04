using Microsoft.Maui.Controls;

namespace StudentHelper.MAUI.Services
{
    public class DialogService : IDialogService
    {
        public async Task ShowAlertAsync(string message, string title = "Info", string buttonLabel = "OK")
        {
            if (Application.Current?.Windows.FirstOrDefault()?.Page is Page page)
            {
                await page.DisplayAlert(title, message, buttonLabel);
            }
        }

        public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation")
        {
            if (Application.Current?.Windows.FirstOrDefault()?.Page is Page page)
            {
                return await page.DisplayAlert(title, message, "Так", "Ні");
            }
            return false;
        }

        public async Task<string?> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string? placeholder = null)
        {
            if (Application.Current?.Windows.FirstOrDefault()?.Page is Page page)
            {
                return await page.DisplayPromptAsync(title, message, accept, cancel, placeholder ?? "");
            }
            return null;
        }
    }
}