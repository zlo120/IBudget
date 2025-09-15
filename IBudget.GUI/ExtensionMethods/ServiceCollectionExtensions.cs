using IBudget.Core.Interfaces;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Core.Services;
using IBudget.GUI.Services.Impl;
using IBudget.GUI.ViewModels;
using IBudget.GUI.ViewModels.DataView;
using IBudget.GUI.ViewModels.UploadCsv;
using IBudget.Infrastructure;
using IBudget.Infrastructure.Repositories;
using IBudget.Spreadsheet.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IBudget.GUI.ExtensionMethods
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddSingleton<UploadCsvPageViewModel>();
            collection.AddSingleton<StepViewModel>();
            collection.AddSingleton<StepContainerViewModel>();
            collection.AddSingleton<CsvService>();

            collection.AddSingleton<CompleteStepPageViewModel>();
            collection.AddSingleton<TagDataStepPageViewModel>();
            collection.AddSingleton<UploadStepPageViewModel>();

            collection.AddTransient<MainWindowViewModel>();
            collection.AddTransient<HomePageViewModel>();
            collection.AddTransient<TagsPageViewModel>();
            collection.AddTransient<DataPageViewModel>();
            collection.AddTransient<DictionariesPageViewModel>();
            collection.AddTransient<MonthlyViewModel>();
            collection.AddTransient<WeeklyViewModel>();

            collection.AddTransient<Context>();

            collection.AddTransient<IUserDictionaryService, UserDictionaryService>();
            collection.AddTransient<IUserDictionaryRepository, UserDictionaryAkavacheRepository>();
            collection.AddTransient<ICSVParserService, CSVParserService>();
            collection.AddScoped<IIncomeService, IncomeService>();
            collection.AddScoped<IIncomeRepository, IncomeRepository>();
            collection.AddScoped<IExpenseService, ExpenseService>();
            collection.AddScoped<IExpenseRepository, ExpenseRepository>();
            collection.AddScoped<ISummaryService, SummaryService>();
            collection.AddScoped<ISummaryRepository, SummaryRepository>();
            collection.AddScoped<ITagService, TagService>();
            collection.AddScoped<ITagRepository, TagRepository>();
            collection.AddScoped<IBatchHashService, BatchHashService>();
            collection.AddScoped<IBatchHashRepository, BatchHashRepository>();
            collection.AddScoped<ICalendarService, CalendarService>();
            collection.AddScoped<ISpreadSheetGeneratorService, SpreadSheetGeneratorService>();
            collection.AddScoped<IPopulator, SpreadSheetPopulatorService>();
            collection.AddScoped<ICSVParserService, CSVParserService>();
        }
    }
}
