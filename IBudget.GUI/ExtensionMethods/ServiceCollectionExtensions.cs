using IBudget.Core.Interfaces;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Core.Services;
using IBudget.GUI.Services;
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
            collection.AddTransient<FinancialGoalsPageViewModel>();
            collection.AddTransient<InitialisationViewModel>();
            collection.AddTransient<DataTableViewModel>();
            collection.AddTransient<SettingsPageViewModel>();
            collection.AddTransient<UpdateNotificationViewModel>();
            collection.AddTransient<PatchNotesViewModel>();

            collection.AddTransient<ICSVParserService, CSVParserService>();
            collection.AddScoped<IBatchHashService, BatchHashService>();
            collection.AddScoped<ICalendarService, CalendarService>();
            collection.AddScoped<ISummaryService, SummaryService>();
            collection.AddScoped<ISpreadSheetGeneratorService, SpreadSheetGeneratorService>();
            collection.AddScoped<IPopulator, SpreadSheetPopulatorService>();
            collection.AddScoped<ICSVParserService, CSVParserService>();
            collection.AddSingleton<IMessageService, MessageService>();
            collection.AddScoped<ISettingsService, SettingsService>();

            collection.AddScoped<IIncomeRepository, IncomeRepository>();
            collection.AddScoped<IExpenseRepository, ExpensesRepository>();
            collection.AddScoped<ITagsRepository, TagsRepository>();
            collection.AddScoped<IExpenseTagsRepository, ExpenseTagsRepository>();
            collection.AddScoped<IExpenseRuleTagsRepository, ExpenseRuleTagsRepository>();
            collection.AddScoped<IFinancialGoalRepository, FinancialGoalRepository>();

            collection.AddScoped<IIncomeService, IncomeService>();
            collection.AddScoped<IExpenseService, ExpenseService>();
            collection.AddScoped<ITagService, TagService>();
            collection.AddScoped<IExpenseTagService, ExpenseTagService>();
            collection.AddScoped<IExpenseRuleTagService, ExpenseRuleTagService>();
            collection.AddScoped<IFinancialGoalService, FinancialGoalService>();

            collection.AddSingleton<MongoDbContext>();
            collection.AddSingleton<IUpdateService, UpdateService>();

#if DEBUG
            collection.AddSingleton<IPatchNotesService, TestPatchNotesService>();
#else
            collection.AddSingleton<IPatchNotesService, PatchNotesService>();
#endif
        }
    }
}
