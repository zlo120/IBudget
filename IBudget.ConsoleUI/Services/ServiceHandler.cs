﻿using IBudget.ConsoleUI.UserInterface;
using IBudget.ConsoleUI.UserInterface.MenuOptions;
using IBudget.ConsoleUI.Utils;
using IBudget.Core.Interfaces;
using IBudget.Core.Services;
using IBudget.Infrastructure.Repositories;
using IBudget.Spreadsheet;
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
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<ISummaryService, SummaryService>();
            services.AddScoped<ISummaryRepository, SummaryRepository>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<IRecordUtility, RecordUtility>();
            services.AddScoped<IGenerator, Generator>();
            services.AddScoped<IPopulator, Populator>();
            services.AddScoped<ICSVParserService, CSVParserService>();

            services.AddTransient<IUserDictionaryService, UserDictionaryService>();
            services.AddTransient<IUserDictionaryRepository, UserDictionaryRepository>();

            services.AddScoped<IMenuOption, AddExpenseOption>();
            services.AddScoped<IMenuOption, AddIncomeOption>();
            services.AddScoped<IMenuOption, DeleteRecordOption>();
            services.AddScoped<IMenuOption, ReadMonthOption>();
            services.AddScoped<IMenuOption, ReadWeekOption>();
            services.AddScoped<IMenuOption, UpdateRecordOption>();
            services.AddScoped<IMenuOption, ParseCSVOption>();
            services.AddScoped<IMenuOption, AddRuleDictionaryOption>();

            services.AddScoped<IMainMenu, MainMenu>();
        }
    }
}
