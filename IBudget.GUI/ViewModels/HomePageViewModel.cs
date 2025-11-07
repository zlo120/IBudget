using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.GUI.Services;
using IBudget.Spreadsheet.Interfaces;

namespace IBudget.GUI.ViewModels
{
    public partial class HomePageViewModel(ISpreadSheetGeneratorService spreadSheetGenerator, IMessageService messageService) : ViewModelBase
    {
        private readonly ISpreadSheetGeneratorService _spreadSheetGenerator = spreadSheetGenerator;
        private readonly IMessageService _messageService = messageService;

        [ObservableProperty]
        private string? _message;

        [RelayCommand]
        private async Task GenerateExcel()
        {
            Message = "Generating spreadsheet...";
            await Task.Yield();
            try
            {
                var path = await Task.Run(() => _spreadSheetGenerator.GenerateSpreadsheet().Result);
                Message = $"Spreadsheet generated at {path}";
                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", path);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", path);
                    }
                }
                catch (Exception ex)
                {
                    Message = $"Spreadsheet generated at {path}, but could not open it automatically.\n{ex.Message}";
                }
            }
            catch (Exception ex)
            {
                Message = "An error occurred while generating your spreadsheet.";
                await _messageService.ShowErrorAsync($"There was an issue generating your spreadsheet.\n{ex.StackTrace}");
            }
        }
    }
}
