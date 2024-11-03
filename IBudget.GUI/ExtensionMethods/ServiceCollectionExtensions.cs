using IBudget.Core.Interfaces;
using IBudget.Core.Services;
using IBudget.GUI.Services.Impl;
using IBudget.GUI.ViewModels;
using IBudget.GUI.ViewModels.DataView;
using IBudget.GUI.ViewModels.UploadCsv;
using IBudget.Infrastructure;
using IBudget.Infrastructure.Repositories;
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

            collection.AddDbContext<Context>();

            // collection.AddTransient<IAkavacheService, AkavacheService>(); // remove this
            // collection.AddTransient<IAkavacheRepository, AkavacheRepository>(); // remove this
            collection.AddTransient<IUserDictionaryService, UserDictionaryService>();
            collection.AddTransient<IUserDictionaryRepository, UserDictionaryAkavacheRepository>(); // edit this class
            collection.AddTransient<ICSVParserService, CSVParserService>();
        }
    }
}
