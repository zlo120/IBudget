using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Enums;
using IBudget.Core.Interfaces;
using IBudget.GUI.Services;

namespace IBudget.GUI.ViewModels
{
    public partial class SettingsPageViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IMessageService _messageService;

        [ObservableProperty]
        private DatabaseType _selectedDatabaseType;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isResetInProgress = false;

        public ObservableCollection<DatabaseType> DatabaseTypes { get; }

        public bool IsResetCollectionsSectionVisible => SelectedDatabaseType != DatabaseType.StacksBackend;

        public bool IsEditConfigurationVisible => SelectedDatabaseType == DatabaseType.CustomMongoDbInstance;

        public SettingsPageViewModel(
            ISettingsService settingsService,
            IMessageService messageService)
        {
            _settingsService = settingsService;
            _messageService = messageService;

            DatabaseTypes = new ObservableCollection<DatabaseType>
            {
                DatabaseType.CustomMongoDbInstance,
                DatabaseType.Offline,
                DatabaseType.StacksBackend
            };

            LoadCurrentDatabaseType();
        }

        private void LoadCurrentDatabaseType()
        {
            SelectedDatabaseType = _settingsService.GetDatabaseType();
        }

        partial void OnSelectedDatabaseTypeChanged(DatabaseType value)
        {
            _settingsService.SetDatabaseType(value);
            RestartApplicationAsync();
        }

        [RelayCommand]
        private async Task EditConfiguration()
        {
            try
            {
                _settingsService.ResetDbConnectionString();
                RestartApplicationAsync();
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error opening configuration: {ex.Message}");
            }
        }

        private void RestartApplicationAsync()
        {
            // restart application by opening up the InitialisationWindow and closing the MainWindow
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Close the current application instance
                desktop.Shutdown();

                // Start a new process for the application
                string executablePath = Process.GetCurrentProcess()?.MainModule?.FileName!;
                Process.Start(executablePath!);
            }
        }

        [RelayCommand]
        private async Task ResetExpenseTagCollection()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private async Task ResetExpenseRuleTagCollection()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private async Task ResetExpenseCollection()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private async Task ResetIncomeCollection()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private async Task ResetTagsCollection()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private async Task ResetFinancialGoalsCollection()
        {
            throw new NotImplementedException();
        }
    }
}
