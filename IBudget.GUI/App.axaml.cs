using Akavache;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using IBudget.Core.Interfaces;
using IBudget.Core.RepositoryInterfaces;
using IBudget.GUI.ExtensionMethods;
using IBudget.GUI.ViewModels;
using IBudget.GUI.Views;
using IBudget.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace IBudget.GUI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            BlobCache.ApplicationName = "Stacks";

            // Register all the services needed for the application to run
            var collection = new ServiceCollection();
            collection.AddCommonServices();
            //collection.AddDbContext<Context>();
            collection.AddEntityFrameworkDesignTimeServices();
            var services = collection.BuildServiceProvider(); 
            using (var context = services.GetService<Context>())
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string stacksPath = Path.Combine(appDataPath, "Stacks");
                if (!File.Exists(stacksPath + "\\Stacks.db"))
                    context!.Database.Migrate();
            }
            var akavacheRepository = services.GetService<IUserDictionaryRepository>();
            akavacheRepository!.InitialiseDB();
            var mainViewModel = services.GetRequiredService<MainWindowViewModel>();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}