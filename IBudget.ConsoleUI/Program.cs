using IBudget.ConsoleUI.Services;
using IBudget.ConsoleUI.UserInterface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        hostBuilder.ConfigureServices(conf =>
        {
            ServiceHandler.RegisterServices(ref conf);
        });
        hostBuilder.UseConsoleLifetime();

        var host = hostBuilder.Build();
        var mainMenu = host.Services.GetService<IMainMenu>() as MainMenu;
        await mainMenu.Execute();
        var hostTask = host.StartAsync();
        await hostTask;
    }
}