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
using IBudget.Core.Services;
using IBudget.GUI.Services;

namespace IBudget.GUI.ViewModels
{
    public partial class SettingsPageViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IMessageService _messageService;
        private readonly IExpenseRuleTagService _expenseRuleTagService;
        private readonly IExpenseService _expenseService;
        private readonly IExpenseTagService _expenseTagService;
        private readonly IFinancialGoalService _financialGoalService;
        private readonly IIncomeService _incomeService;
        private readonly ITagService _tagService;
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
            IMessageService messageService,
            IExpenseRuleTagService expenseRuleTagService,
            IExpenseService expenseService,
            IExpenseTagService expenseTagService,
            IFinancialGoalService financialGoalService,
            IIncomeService incomeService,
            ITagService tagService)
        {
            _settingsService = settingsService;
            _messageService = messageService;
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
    }
}
