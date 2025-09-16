using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Spreadsheet.Interfaces;

namespace IBudget.GUI.ViewModels
{
    public partial class HomePageViewModel(ISpreadSheetGeneratorService spreadSheetGenerator) : ViewModelBase
    {
        private readonly ISpreadSheetGeneratorService _spreadSheetGenerator = spreadSheetGenerator;
        [RelayCommand]
        private void GenerateExcel()
        {
            try
            {
                _spreadSheetGenerator.GenerateSpreadsheet();
                Message = "Spreadsheet generated successfully!";
            }
            catch (Exception ex)
            {
                Message = $"There was an issue generating your spreadsheet.\n{ex.StackTrace}";
            }
        }
        [ObservableProperty]
        private string _message = string.Empty;
    }
}
