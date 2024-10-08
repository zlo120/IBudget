using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace IBudget.GUI.ViewModels.DataView
{
    public partial class MonthlyViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _thisMonth = string.Empty;

        [ObservableProperty]
        private ObservableCollection<SummaryItem> _summaryItems = new ObservableCollection<SummaryItem>();
        
        public MonthlyViewModel()
        {
            ThisMonth = DateTime.Now.ToString("MMMM");

            for (int i = 0; i < 50; i++)
            {
                SummaryItems.Add(new SummaryItem($"Summary value {i+1}", 22.22));
            }
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