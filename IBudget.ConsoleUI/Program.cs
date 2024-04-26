// Create the SQLite db if it doesn't exist
using IBudget.ConsoleUI.Services;
using IBudget.ConsoleUI.UserInterface;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static Task Main(string[] args)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var rootDir = Path.Combine(Environment.GetFolderPath(folder), "IBudget");
        var dbDir = Path.Join(rootDir, "IBudgetDB");
        if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir);
        if (!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);
        if (!File.Exists(Path.Join(dbDir, "IBudget.db")))
        {
            var dbFile = File.Create(Path.Join(dbDir, "IBudget.db"));
            dbFile.Close();
        }
        var services = ServiceHandler.RegisterServices();
        var serviceProvider = services.BuildServiceProvider();
        var mainMenu = serviceProvider.GetService(typeof(IMainMenu)) as IMainMenu;
        mainMenu.MainMenuLoop();
        return Task.CompletedTask;
    }
}