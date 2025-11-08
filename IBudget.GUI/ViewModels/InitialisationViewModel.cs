using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Enums;
using IBudget.Core.Interfaces;
using IBudget.GUI.Views;
using IBudget.Infrastructure;

namespace IBudget.GUI.ViewModels
{
    public partial class InitialisationViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingsService _settingsService;
        private Window? _parentWindow;

        [ObservableProperty]
        private string _statusMessage = "Initializing connection...";

        [ObservableProperty]
        private bool _isConnecting = true;

        [ObservableProperty]
        private bool _isConnected = false;

        [ObservableProperty]
        private DatabaseType _selectedDatabaseType = DatabaseType.CustomMongoDbInstance;

        [ObservableProperty]
        private bool _isMongoTypeSelected = true;

        public ObservableCollection<DatabaseType> DatabaseTypes { get; }

        public event EventHandler<bool>? ConnectionCompleted;

        public InitialisationViewModel(IServiceProvider serviceProvider, ISettingsService settingsService)
        {
            _serviceProvider = serviceProvider;
            _settingsService = settingsService;
            DatabaseTypes = new ObservableCollection<DatabaseType>
            {
                DatabaseType.CustomMongoDbInstance,
                DatabaseType.Offline,
                //DatabaseType.StacksBackend
            };
            try
            {
                var dbType = _settingsService.GetDatabaseType();
                if (dbType is not null)
                {
                    if (dbType == DatabaseType.CustomMongoDbInstance)
                    {
                        IsMongoTypeSelected = true;
                    }
                    else
                    {
                        IsMongoTypeSelected = false;
                    }
                    SelectedDatabaseType = dbType.Value;
                }
            }
            catch (FileNotFoundException)
            {
                // Default behaviour: Create settings file and use offline mode
                _ = OfflineContinue();
            }
        }

        public void SetParentWindow(Window window)
        {
            _parentWindow = window;
        }

        partial void OnSelectedDatabaseTypeChanged(DatabaseType value)
        {
            SelectedDatabaseType = value;
            IsMongoTypeSelected = SelectedDatabaseType == DatabaseType.CustomMongoDbInstance;
            if (SelectedDatabaseType == DatabaseType.Offline)
            {
                StatusMessage = "You may click continue to progress.";
            }
            else if (SelectedDatabaseType == DatabaseType.CustomMongoDbInstance)
            {
                _ = TestDatabaseConnectionAsync(SelectedDatabaseType);
            }
            else if (SelectedDatabaseType == DatabaseType.StacksBackend)
            {
                StatusMessage = "StacksBackend database mode is not implemented yet.";
            }
        }

        public async Task InitializeConnectionAsync()
        {
            await TestDatabaseConnectionAsync(null);
        }

        [RelayCommand]
        private async Task TestDatabaseConnectionAsync(DatabaseType? dbType)
        {
            IsConnecting = true;
            StatusMessage = "Testing database connection...";

            try
            {
                await AttemptDatabaseConnection(dbType);
                ConnectionCompleted?.Invoke(this, true);
            }
            catch (KeyNotFoundException)
            {
                StatusMessage = "❌ Connection string not found. Please configure your settings.";
            }
            catch (FileNotFoundException)
            {
                StatusMessage = "❌ Connection string not found. Please configure your settings.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Database connection failed: {ex.Message}";
            }
            finally
            {
                IsConnecting = false;
            }
        }

        private async Task AttemptDatabaseConnection(DatabaseType? dbType)
        {
            var databaseType = dbType ?? _settingsService.GetDatabaseType();
            switch (databaseType)
            {
                case DatabaseType.Offline:
                    StatusMessage = "✅ Offline mode selected. No database connection required.";
                    break;
                case DatabaseType.StacksBackend:
                    throw new NotImplementedException("StacksBackend database mode is not implemented yet.");
                case DatabaseType.CustomMongoDbInstance:
                    await TestMongoDbConnection();
                    break;
                default:
                    throw new InvalidOperationException("Unsupported database type.");
            }
        }

        private async Task TestMongoDbConnection()
        {
            var settingsService = _serviceProvider.GetService(typeof(Core.Interfaces.ISettingsService)) as Core.Interfaces.ISettingsService
                        ?? throw new InvalidOperationException("Settings service not available.");
            var connectionString = settingsService.GetDbConnectionString();
            await MongoDbContext.TestConnection(connectionString);
        }

        [RelayCommand]
        private async Task TestConnection()
        {
            await TestDatabaseConnectionAsync(DatabaseType.CustomMongoDbInstance);
        }

        [RelayCommand]
        private async void ConfigureSettings()
        {
            var settingsService = _serviceProvider.GetService(typeof(ISettingsService)) as ISettingsService
                ?? throw new InvalidOperationException("Settings service not available.");
            var configurationViewModel = new ConfigurationViewModel(settingsService);
            var configurationWindow = new ConfigurationWindow
            {
                DataContext = configurationViewModel
            };

            // Handle configuration completion
            configurationViewModel.ConfigurationCompleted += async (sender, e) =>
            {
                configurationWindow.Close();
                await TestDatabaseConnectionAsync(null);
            };

            // Use ShowDialog to make it modal - this keeps the parent window visible
            await configurationWindow.ShowDialog(_parentWindow);
        }

        [RelayCommand]
        private async Task OfflineContinue()
        {
            _settingsService.SetDatabaseType(DatabaseType.Offline);
            await TestDatabaseConnectionAsync(DatabaseType.Offline);
        }
    }
}
