using IBudget.Core.Enums;
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
using IBudget.Infrastructure.Repositories.LiteDb;
using IBudget.Spreadsheet.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IBudget.GUI.ExtensionMethods
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            // ViewModels
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

            // Core Services (database-agnostic)
            collection.AddTransient<ICSVParserService, CSVParserService>();
            collection.AddScoped<IBatchHashService, BatchHashService>();
            collection.AddScoped<ICalendarService, CalendarService>();
            collection.AddScoped<ISummaryService, SummaryService>();
            collection.AddScoped<ISpreadSheetGeneratorService, SpreadSheetGeneratorService>();
            collection.AddScoped<IPopulator, SpreadSheetPopulatorService>();
            collection.AddSingleton<IMessageService, MessageService>();
            collection.AddScoped<ISettingsService, SettingsService>();

            // Business Logic Services
            collection.AddScoped<IIncomeService, IncomeService>();
            collection.AddScoped<IExpenseService, ExpenseService>();
            collection.AddScoped<ITagService, TagService>();
            collection.AddScoped<IExpenseTagService, ExpenseTagService>();
            collection.AddScoped<IExpenseRuleTagService, ExpenseRuleTagService>();
            collection.AddScoped<IFinancialGoalService, FinancialGoalService>();

            // Other services
            collection.AddSingleton<IUpdateService, UpdateService>();

#if DEBUG
            collection.AddSingleton<IPatchNotesService, TestPatchNotesService>();
#else
            collection.AddSingleton<IPatchNotesService, PatchNotesService>();
#endif
        }

        /// <summary>
        /// Registers database-specific repositories based on the selected database type
        /// </summary>
        public static void AddDatabaseServices(this IServiceCollection collection, DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.Offline:
                    // Register LiteDB context and repositories
                    collection.AddSingleton<LiteDbContext>();
                    collection.AddScoped<IIncomeRepository, LiteDbIncomeRepository>();
                    collection.AddScoped<IExpenseRepository, LiteDbExpensesRepository>();
                    collection.AddScoped<ITagsRepository, LiteDbTagsRepository>();
                    collection.AddScoped<IExpenseTagsRepository, LiteDbExpenseTagsRepository>();
                    collection.AddScoped<IExpenseRuleTagsRepository, LiteDbExpenseRuleTagsRepository>();
                    collection.AddScoped<IFinancialGoalRepository, LiteDbFinancialGoalRepository>();
                    break;

                case DatabaseType.CustomMongoDbInstance:
                case DatabaseType.StacksBackend:
                default:
                    // Register MongoDB context and repositories
                    collection.AddSingleton<MongoDbContext>();
                    collection.AddScoped<IIncomeRepository, IncomeRepository>();
                    collection.AddScoped<IExpenseRepository, ExpensesRepository>();
                    collection.AddScoped<ITagsRepository, TagsRepository>();
                    collection.AddScoped<IExpenseTagsRepository, ExpenseTagsRepository>();
                    collection.AddScoped<IExpenseRuleTagsRepository, ExpenseRuleTagsRepository>();
                    collection.AddScoped<IFinancialGoalRepository, FinancialGoalRepository>();
                    break;
            }
        }
    }
}
