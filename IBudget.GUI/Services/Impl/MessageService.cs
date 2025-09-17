using System.Threading.Tasks;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;

namespace IBudget.GUI.Services.Impl
{
    public class MessageService : IMessageService
    {
        public async Task ShowMessageAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "OK"
            };

            var topLevel = TopLevel.GetTopLevel(App.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ?
                desktop.MainWindow : null);

            if (topLevel != null)
            {
                await dialog.ShowAsync(topLevel);
            }
        }

        public async Task ShowErrorAsync(string message)
        {
            await ShowMessageAsync("Error", message);
        }

        public async Task ShowSuccessAsync(string message)
        {
            await ShowMessageAsync("Success", message);
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No"
            };

            var topLevel = TopLevel.GetTopLevel(App.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ?
                desktop.MainWindow : null);

            if (topLevel != null)
            {
                var result = await dialog.ShowAsync(topLevel);
                return result == ContentDialogResult.Primary;
            }

            return false;
        }
    }
}
