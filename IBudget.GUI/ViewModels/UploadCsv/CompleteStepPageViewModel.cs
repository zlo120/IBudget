using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using IBudget.Core.Utils;
using IBudget.GUI.ExtensionMethods;
using IBudget.GUI.Services.Impl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class CompleteStepPageViewModel : StepBase
    {
        private readonly CsvService _csvService;
        private readonly ICSVParserService _csvParserService;
        private readonly StepViewModel _stepViewModel;
        private readonly IExpenseService _expenseService;
        private readonly IIncomeService _incomeService;
        private readonly ITagService _tagService;
        [ObservableProperty]
        private string _headerMessage = string.Empty;
        [ObservableProperty]
        private string _bodyMessage = string.Empty;
        [ObservableProperty]
        private bool _isEnabled = false; // is button enabled
        [ObservableProperty]
        private bool _isLoading = false;

        public CompleteStepPageViewModel(
            StepViewModel stepViewModel,
            CsvService csvService,
            ICSVParserService csvParserService,
            IExpenseService expenseService,
            IIncomeService incomeService,
            ITagService tagService
        )
        {
            _csvService = csvService;
            _csvParserService = csvParserService;
            _stepViewModel = stepViewModel;
            _expenseService = expenseService;
            _incomeService = incomeService;
            _tagService = tagService;
            ProcessCsv();
        }

        partial void OnIsLoadingChanged(bool value)
        {
            switch (value)
            {
                case true: // is loading
                    IsEnabled = false;
                    HeaderMessage = "Saving your csv data...";
                    BodyMessage = "Please wait while your data is being saved.";
                    break;

                case false: // finished loading
                    IsEnabled = true;
                    HeaderMessage = "All done!";
                    BodyMessage = "Your data has been saved, you can view your information on the view data page.";
                    break;
            }
        }
        [RelayCommand]
        private void StartAgain()
        {
            Reset();
            OnSteppingOver();
            _stepViewModel.StepOver();
        }
        public async void ProcessCsv()
        {
            IsLoading = true;
            if (_csvService.FileUri is null) return; 
            var csvFile = _csvService.FileUri!.LocalPath;
            var formattedFinancialDataList = await _csvParserService.ParseCSV(csvFile.Replace("%20", " "));
            // verify hash doesn't exist yet
            // TO  DO

            // receive all the data without tags,
            // then tag all the data
            foreach (var formattedFinancialCSV in formattedFinancialDataList)
            {
                if (formattedFinancialCSV.Description is null) continue;
                var tags = await _tagService.FindTagByDescription(formattedFinancialCSV.Description!);
                var formattedTags = new List<Tag>();
                foreach (var tag in tags)
                    formattedTags.Add(new Tag { Name = tag });
                var formattedDescription = CsvFormatter.FormatDescription(formattedFinancialCSV.Description!);
                if (formattedFinancialCSV.Amount > 0)
                {
                    var income = new Income()
                    {
                        Amount = formattedFinancialCSV.Amount,
                        Source = formattedDescription,
                        Date = formattedFinancialCSV.Date.ToDateTime(new TimeOnly(0, 0)),
                        Tags = formattedTags
                    };
                    await _incomeService.AddIncome(income);
                }
                else
                {
                    var expense = new Expense()
                    {
                        Amount = formattedFinancialCSV.Amount,
                        Notes = formattedDescription,
                        Date = formattedFinancialCSV.Date.ToDateTime(new TimeOnly(0, 0)),
                        Tags = formattedTags
                    };
                    await _expenseService.AddExpense(expense);
                }
            }
            IsLoading = false;
        }
        private void Reset()
        {
            IsLoading = false;
            IsEnabled = false;
            HeaderMessage = string.Empty;
            BodyMessage = string.Empty;
        }
    }
}
