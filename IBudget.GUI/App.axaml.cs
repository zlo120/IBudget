using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using IBudget.GUI.ExtensionMethods;
using IBudget.GUI.ViewModels;
using IBudget.GUI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IBudget.GUI
{
    public partial class App : Application
    {
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

            var services = collection.BuildServiceProvider();

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
                dbConnectionViewModel.ConnectionCompleted += (sender, isConnected) =>
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
        }
    }
}