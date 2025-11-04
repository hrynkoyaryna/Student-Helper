namespace StudentHelper.MAUI.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title = "Info", string buttonLabel = "OK");
        Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation");
        Task<string?> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string? placeholder = null);
    }
}