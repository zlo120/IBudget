using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using IBudget.GUI.Services.Impl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class CompleteStepPageViewModel : StepBase
    {
        private readonly CsvService _csvService;
        private readonly ICSVParserService _csvParserService;
        private readonly IAkavacheService _akavacheService;
        private readonly StepViewModel _stepViewModel;

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
            IAkavacheService akavacheService
        )
        {
            _csvService = csvService;
            _csvParserService = csvParserService;
            _akavacheService = akavacheService;
            _stepViewModel = stepViewModel;
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
            Parallel.ForEach(formattedFinancialDataList, async item =>
            {
                await _akavacheService.InsertFinance(item);
            });
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
