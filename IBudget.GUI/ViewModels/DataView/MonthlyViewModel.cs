using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace IBudget.GUI.ViewModels.DataView
{
    public partial class MonthlyViewModel : ViewModelBase
    {
        private readonly IAkavacheService _akavacheService;

        [ObservableProperty]
        private string _thisMonth = string.Empty;

        [ObservableProperty]
        private ObservableCollection<SummaryItem> _summaryItems = new ObservableCollection<SummaryItem>();

        public MonthlyViewModel(IAkavacheService akavacheService)
        {
            _akavacheService = akavacheService;
            ThisMonth = DateTime.Now.ToString("MMMM");

            // PREDICTION: when adding new data to the db, this view will not update
            // POTENTIAL SOLUTION: event handler? update this view when adding data to akavache db
            Task.Run(() => UpdateMonthDataView()).Wait();

            for (int i = 0; i < 50; i++)
            {
                SummaryItems.Add(new SummaryItem($"Summary value {i + 1}", 22.22));
            }
        }

        public async Task<List<FormattedFinancialCSV>> UpdateMonthDataView()
        {
            var tags = await _akavacheService.GetAllTags();
            return new List<FormattedFinancialCSV>();
        }
    }

    public class SummaryItem
    {
        public SummaryItem(string title, double value)
        {
            SummaryTitle = title;
            SummaryValue = $"${value}";
        }
        public string SummaryTitle { get; set; }
        public string SummaryValue { get; set; }
    }
}