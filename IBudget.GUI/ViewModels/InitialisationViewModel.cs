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
                DatabaseType.StacksBackend
            };
            SelectedDatabaseType = _settingsService.GetDatabaseType();
        }

        public void SetParentWindow(Window window)
        {
            _parentWindow = window;
        }

        public async Task InitializeConnectionAsync()
        {
            await TestDatabaseConnectionAsync();
        }

        [RelayCommand]
        private async Task TestDatabaseConnectionAsync()
        {
            IsConnecting = true;
            StatusMessage = "Testing database connection...";

            try
            {
                await AttemptDatabaseConnection();
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

        private async Task AttemptDatabaseConnection()
        {
            var databaseType = _settingsService.GetDatabaseType();
            switch(databaseType)
            {
                case DatabaseType.Offline:
                    throw new NotImplementedException("Offline database mode is not implemented yet.");
                case DatabaseType.StacksBackend:
                    throw new NotImplementedException("StacksBackend database mode is not implemented yet.");
                case DatabaseType.CustomMongoDbInstance:
                    var settingsService = _serviceProvider.GetService(typeof(Core.Interfaces.ISettingsService)) as Core.Interfaces.ISettingsService
                        ?? throw new InvalidOperationException("Settings service not available.");
                    var connectionString = settingsService.GetDbConnectionString();
                    await MongoDbContext.TestConnection(connectionString);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported database type.");
            }            
        }

        [RelayCommand]
        private async Task Retry()
        {
            await TestDatabaseConnectionAsync();
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
                await TestDatabaseConnectionAsync();
            };

            // Use ShowDialog to make it modal - this keeps the parent window visible
            await configurationWindow.ShowDialog(_parentWindow);
        }
    }
}
