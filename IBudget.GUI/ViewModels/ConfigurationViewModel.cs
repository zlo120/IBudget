using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using MongoDB.Driver.Core.Configuration;

namespace IBudget.GUI.ViewModels
{
    public partial class ConfigurationViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private string _connectionString = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isSaving = false;

        public event EventHandler? ConfigurationCompleted;
        public event EventHandler? CloseRequested;

        public ConfigurationViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            LoadExistingConnectionString();
        }

        private void LoadExistingConnectionString()
        {
            try
            {
                ConnectionString = _settingsService.GetDbConnectionString();
            }
            catch
            {
                ConnectionString = string.Empty;
            }
        }

        [RelayCommand]
        private async Task SaveConfiguration()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                StatusMessage = "❌ Please enter a valid connection string.";
                return;
            }

            IsSaving = true;
            StatusMessage = "Saving configuration...";

            try
            {
                _settingsService.SetDbConnectionString(ConnectionString);
                StatusMessage = "✅ Configuration saved successfully!";
                ConfigurationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Failed to save configuration: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}