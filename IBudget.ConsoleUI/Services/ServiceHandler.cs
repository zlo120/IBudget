using IBudget.ConsoleUI.UserInterface;
using IBudget.ConsoleUI.Utils;
using IBudget.Core.Interfaces;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Core.Services;
using IBudget.Infrastructure.Repositories;
using IBudget.Spreadsheet.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IBudget.ConsoleUI.Services
{
    public static class ServiceHandler
    {
        public static void RegisterServices(ref IServiceCollection services)
        {
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IIncomeRepository, IncomeRepository>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IExpenseRepository, ExpensesRepository>();
            services.AddScoped<ISummaryService, SummaryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITagsRepository, TagsRepository>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<IRecordUtility, RecordUtility>();
            services.AddScoped<ISpreadSheetGeneratorService, SpreadSheetGeneratorService>();
            services.AddScoped<IPopulator, SpreadSheetPopulatorService>();
            services.AddScoped<ICSVParserService, CSVParserService>();

            services.AddScoped<IMainMenu, MainMenu>();
        }
    }
}
