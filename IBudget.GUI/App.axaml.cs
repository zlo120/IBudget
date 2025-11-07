using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using IBudget.GUI.ExtensionMethods;
using IBudget.GUI.ViewModels;
using IBudget.GUI.Views;
using IBudget.GUI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using IBudget.Core.Enums;

namespace IBudget.GUI
{
    public partial class App : Application
    {
        private IServiceProvider? _services;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Register all the services needed for the application to run
            var collection = new ServiceCollection();
            collection.AddCommonServices();
            collection.AddDatabaseServices(DatabaseType.CustomMongoDbInstance);

            var services = collection.BuildServiceProvider();
            _services = services;

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                // Show database connection window first
                var dbConnectionViewModel = services.GetRequiredService<InitialisationViewModel>();
                var dbConnectionWindow = new InitialisationWindow
                {
                    DataContext = dbConnectionViewModel
                };

                // Set the parent window reference
                dbConnectionViewModel.SetParentWindow(dbConnectionWindow);

                // Handle connection completion
                dbConnectionViewModel.ConnectionCompleted += async (sender, isConnected) =>
                {
                    if (isConnected)
                    {
                        // Connection successful, show main window
                        var mainViewModel = services.GetRequiredService<MainWindowViewModel>();
                        desktop.MainWindow = new MainWindow
                        {
                            DataContext = mainViewModel,
                        };
                        desktop.MainWindow.Show();
                            
                        // Check and show patch notes if needed (after main window is shown)
                        await CheckAndShowPatchNotesAsync();
                    }
                    else
                    {
                        // Connection failed, exit application
                        desktop.Shutdown();
                    }
                    dbConnectionWindow.Close();
                };

                // Show the database connection window
                dbConnectionWindow.Show();

                // Start the connection test
                await dbConnectionViewModel.InitializeConnectionAsync();
            }

            base.OnFrameworkInitializationCompleted();
            
            // Check for updates in the background
            _ = Task.Run(CheckForUpdatesAsync);
        }

        private async Task CheckAndShowPatchNotesAsync()
        {
            try
            {
                // Wait a moment for the main window to fully load
                await Task.Delay(TimeSpan.FromSeconds(1));

                if (_services == null)
                    return;

                var patchNotesService = _services.GetService<IPatchNotesService>();
                if (patchNotesService == null)
                    return;

                // Check if we should show patch notes
                var shouldShow = await patchNotesService.ShouldShowPatchNotesAsync();
                
                if (shouldShow)
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        await ShowPatchNotesAsync();
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the app
                Console.WriteLine($"Error checking patch notes: {ex.Message}");
            }
        }

        private async Task ShowPatchNotesAsync()
        {
            if (_services == null)
                return;

            try
            {
                var patchNotesViewModel = _services.GetRequiredService<PatchNotesViewModel>();
                
                var patchNotesWindow = new PatchNotesWindow
                {
                    DataContext = patchNotesViewModel
                };

                // Load patch notes content
                await patchNotesViewModel.LoadPatchNotesAsync();

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                    desktop.MainWindow != null)
                {
                    await patchNotesWindow.ShowDialog(desktop.MainWindow);
                }
                else
                {
                    patchNotesWindow.Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing patch notes: {ex.Message}");
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                // Wait a few seconds after startup before checking
                await Task.Delay(TimeSpan.FromSeconds(10));

                if (_services == null)
                    return;

                var updateService = _services.GetService<IUpdateService>();
                if (updateService == null)
                    return;

                // Check if updates are available
                var updateInfo = await updateService.CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    // Show update notification on UI thread
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        ShowUpdateNotification(updateInfo);
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the error - updates failed but app continues working
                Console.WriteLine($"Update check failed: {ex.Message}");
            }
        }

        private void ShowUpdateNotification(UpdateInfo updateInfo)
        {
            if (_services == null)
                return;

            var updateViewModel = _services.GetRequiredService<UpdateNotificationViewModel>();
            updateViewModel.SetPendingUpdate(updateInfo);

            var updateWindow = new UpdateNotificationWindow
            {
                DataContext = updateViewModel
            };

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                desktop.MainWindow != null)
            {
                updateWindow.ShowDialog(desktop.MainWindow);
            }
            else
            {
                updateWindow.Show();
            }
        }
    }
}