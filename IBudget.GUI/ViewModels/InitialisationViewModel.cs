using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using IBudget.GUI.Views;
using Avalonia.Controls;
using IBudget.Infrastructure;

namespace IBudget.GUI.ViewModels
{
    public partial class InitialisationViewModel(IServiceProvider serviceProvider) : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private Window? _parentWindow;

        [ObservableProperty]
        private string _statusMessage = "Initializing connection...";

        [ObservableProperty]
        private bool _isConnecting = true;

        [ObservableProperty]
        private bool _isConnected = false;

        public event EventHandler<bool>? ConnectionCompleted;

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
            var settingsService = _serviceProvider.GetService(typeof(Core.Interfaces.ISettingsService)) as Core.Interfaces.ISettingsService
                ?? throw new InvalidOperationException("Settings service not available.");
            var connectionString = settingsService.GetDbConnectionString();
            await MongoDbContext.TestConnection(connectionString);
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
