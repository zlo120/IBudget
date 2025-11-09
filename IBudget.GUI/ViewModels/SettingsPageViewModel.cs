using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        private readonly IUpdateService _updateService;
        private readonly IExpenseRuleTagService _expenseRuleTagService;
        private readonly IExpenseService _expenseService;
        private readonly IExpenseTagService _expenseTagService;
        private readonly IFinancialGoalService _financialGoalService;
        private readonly IIncomeService _incomeService;
        private readonly ITagService _tagService;
        private readonly IImportExportService _importExportService;

        [ObservableProperty]
        private DatabaseType? _selectedDatabaseType = null;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isResetInProgress = false;

        [ObservableProperty]
        private string _updateStatus = "Check for updates";

        [ObservableProperty]
        private string _currentVersion = "Unknown";

        [ObservableProperty]
        private string _exportStatusMessage = string.Empty;

        [ObservableProperty]
        private string _importStatusMessage = string.Empty;

        [ObservableProperty]
        private bool _isCheckingForUpdates = false;

        public ObservableCollection<DatabaseType> DatabaseTypes { get; }

        public bool IsResetCollectionsSectionVisible => SelectedDatabaseType != DatabaseType.StacksBackend;

        public bool IsEditConfigurationVisible => SelectedDatabaseType == DatabaseType.CustomMongoDbInstance;

        public SettingsPageViewModel(
            ISettingsService settingsService,
            IMessageService messageService,
            IUpdateService updateService,
            IExpenseRuleTagService expenseRuleTagService,
            IExpenseService expenseService,
            IExpenseTagService expenseTagService,
            IFinancialGoalService financialGoalService,
            IIncomeService incomeService,
            ITagService tagService,
            IImportExportService importExportService)
        {
            _settingsService = settingsService;
            _messageService = messageService;
            _updateService = updateService;
            _expenseRuleTagService = expenseRuleTagService;
            _expenseService = expenseService;
            _expenseTagService = expenseTagService;
            _financialGoalService = financialGoalService;
            _incomeService = incomeService;
            _tagService = tagService;

            DatabaseTypes = new ObservableCollection<DatabaseType>
            {
                DatabaseType.CustomMongoDbInstance,
                DatabaseType.Offline,
                //DatabaseType.StacksBackend
            };

            LoadCurrentDatabaseType();
            CurrentVersion = _updateService.GetCurrentVersion();
            _importExportService = importExportService;
        }

        private void LoadCurrentDatabaseType()
        {
            var dbType = _settingsService.GetDatabaseType();
            if (dbType is not null)
            {
                SelectedDatabaseType = dbType;
            }
        }

        partial void OnSelectedDatabaseTypeChanged(DatabaseType? oldValue, DatabaseType? newValue)
        {
            _settingsService.SetDatabaseType(newValue);
            OnPropertyChanged(nameof(IsResetCollectionsSectionVisible));
            OnPropertyChanged(nameof(IsEditConfigurationVisible));
            if (oldValue is not null)
            {
                RestartApplicationAsync();
            }
        }

        [RelayCommand]
        private async Task ResetConfiguration()
        {
            var decision = await _messageService.ShowConfirmationAsync("Reset Stacks Configuration", "This will reset all your settings and you will need to input your MongoDB connection string again.");
            if (!decision)
            {
                return;
            }
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
            var confirmation = await _messageService.ShowConfirmationAsync("Reset collection", "This will delete all expense tags. Are you sure you want to continue?");
            if (confirmation)
            {
                await _expenseTagService.ClearCollection();
            }
        }

        [RelayCommand]
        private async Task ResetExpenseRuleTagCollection()
        {
            var confirmation = await _messageService.ShowConfirmationAsync("Reset collection", "This will delete all expense rule tags. Are you sure you want to continue?");
            if (confirmation)
            {
                await _expenseRuleTagService.ClearCollection();
            }
        }

        [RelayCommand]
        private async Task ResetExpenseCollection()
        {
            var confirmation = await _messageService.ShowConfirmationAsync("Reset collection", "This will delete all expense records. Are you sure you want to continue?");
            if (confirmation)
            {
                await _expenseService.ClearCollection();
            }
        }

        [RelayCommand]
        private async Task ResetIncomeCollection()
        {
            var confirmation = await _messageService.ShowConfirmationAsync("Reset collection", "This will delete all income records. Are you sure you want to continue?");
            if (confirmation)
            {
                await _incomeService.ClearCollection();
            }
        }

        [RelayCommand]
        private async Task ResetTagsCollection()
        {
            var confirmation = await _messageService.ShowConfirmationAsync("Reset collection", "This will delete all the tags. Are you sure you want to continue?");
            if (confirmation)
            {
                await _tagService.ClearCollection();
            }
        }

        [RelayCommand]
        private async Task ResetFinancialGoalsCollection()
        {
            var confirmation = await _messageService.ShowConfirmationAsync("Reset collection", "This will delete all your financial goals. Are you sure you want to continue?");
            if (confirmation)
            {
                await _financialGoalService.ClearCollection();
            }
        }

        [RelayCommand]
        private async Task CheckForUpdates()
        {
            try
            {
                IsCheckingForUpdates = true;
                UpdateStatus = "Checking for updates...";

                var updateInfo = await _updateService.CheckForUpdatesAsync();

                if (updateInfo == null)
                {
                    UpdateStatus = "You're up to date!";
                    await _messageService.ShowMessageAsync("No updates available", $"You're running the latest version (v{CurrentVersion})");
                    return;
                }

                UpdateStatus = $"Downloading v{updateInfo.Version}...";
                await _updateService.DownloadUpdateAsync(updateInfo);

                UpdateStatus = "Update ready! Restart to apply.";

                var restart = await _messageService.ShowConfirmationAsync(
                    "Update Downloaded",
                    $"Version {updateInfo.Version} is ready to install. Restart now?");

                if (restart)
                {
                    _updateService.ApplyUpdateAndRestart(updateInfo);
                }
                else
                {
                    UpdateStatus = "Update ready - restart when convenient";
                }
            }
            catch (Exception ex)
            {
                UpdateStatus = "Update check failed";
                await _messageService.ShowErrorAsync($"Failed to check for updates: {ex.Message}");
            }
            finally
            {
                IsCheckingForUpdates = false;
            }
        }

        [RelayCommand]
        private async Task Export()
        {
            try
            {
                ExportStatusMessage = "Preparing export... This may take a moment.";
                var exportDirectory = await _importExportService.ExportData();
                ExportStatusMessage = $"✅ Export successful! Your data has been saved to:\n{exportDirectory}";
                
                var decision = await _messageService.ShowConfirmationAsync(
                    "Export Completed", 
                    $"Your financial data has been successfully exported.\n\nLocation: {exportDirectory}\n\nWould you like to open the export folder?");
                
                if (decision)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = exportDirectory,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
            }
            catch (Exception ex)
            {
                ExportStatusMessage = $"❌ Export failed: {ex.Message}";
                await _messageService.ShowErrorAsync($"Failed to export data.\n\nError: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ImportFile(string filePath)
        {
            try
            {
                ImportStatusMessage = $"Importing data from:\n{Path.GetFileName(filePath)}";
                
                var confirmation = await _messageService.ShowConfirmationAsync(
                    "Confirm Import", 
                    "This will replace your existing data with the imported data. Are you sure you want to continue?");
                
                if (!confirmation)
                {
                    ImportStatusMessage = "Import cancelled by user.";
                    return;
                }
                
                await _importExportService.ImportData(filePath);
                ImportStatusMessage = "✅ Import completed successfully! Your data has been restored.";
            }
            catch (Exception ex)
            {
                ImportStatusMessage = $"❌ Import failed: {ex.Message}";
                await _messageService.ShowErrorAsync($"Failed to import data.\n\nError: {ex.Message}");
            }
        }
    }
}
