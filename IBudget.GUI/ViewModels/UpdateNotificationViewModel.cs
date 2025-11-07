using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.GUI.Services;
using System.Threading.Tasks;

namespace IBudget.GUI.ViewModels
{
    public partial class UpdateNotificationViewModel : ViewModelBase
    {
        private readonly IUpdateService _updateService;
        private UpdateInfo? _pendingUpdate;

        [ObservableProperty]
        private string _newVersion = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isDownloading = false;

        [ObservableProperty]
        private bool _isUpdateReady = false;

        public UpdateNotificationViewModel(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        public void SetPendingUpdate(UpdateInfo updateInfo)
        {
            _pendingUpdate = updateInfo;
            NewVersion = updateInfo.Version;
            StatusMessage = $"A new version (v{updateInfo.Version}) is available!";
        }

        [RelayCommand]
        private async Task DownloadAndInstall()
        {
            if (_pendingUpdate == null)
                return;

            IsDownloading = true;
            StatusMessage = "Downloading update...";

            await _updateService.DownloadUpdateAsync(_pendingUpdate);

            IsDownloading = false;
            IsUpdateReady = true;
            StatusMessage = "Update ready! Click 'Restart Now' to apply.";
        }

        [RelayCommand]
        private void RestartNow()
        {
            if (_pendingUpdate == null)
                return;

            _updateService.ApplyUpdateAndRestart(_pendingUpdate);
        }

        [RelayCommand]
        private void RemindLater()
        {
            // Close the notification window - handled by the window itself
        }
    }
}
