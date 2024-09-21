using IBudget.Core.Interfaces;
using IBudget.Core.Services;
using IBudget.GUI.Services.Impl;
using IBudget.GUI.ViewModels;
using IBudget.GUI.ViewModels.UploadCsv;
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
            collection.AddTransient<ThisMonthPageViewModel>();
            collection.AddTransient<DictionariesPageViewModel>();

            collection.AddTransient<IAkavacheService, AkavacheService>();
            collection.AddTransient<IAkavacheRepository, AkavacheRepository>();
            collection.AddTransient<IUserDictionaryService, UserDictionaryService>();
            collection.AddTransient<IUserDictionaryRepository, UserDictionaryAkavacheRepository>();
            collection.AddTransient<ICSVParserService, CSVParserService>();
        }
    }
}
