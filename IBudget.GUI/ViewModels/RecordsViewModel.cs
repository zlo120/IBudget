using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace IBudget.GUI.ViewModels
{
    public partial class RecordsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _monthText = string.Empty;
        [ObservableProperty]
        private string _thisMonth = string.Empty;
        [ObservableProperty]
        private int _selectedMonthIndex = 0;
        [ObservableProperty]
        private string _searchText = string.Empty;
        [ObservableProperty]
        private ObservableCollection<DataTableItem> _tableData = [];
        public RecordsViewModel()
        {
            var thisMonthInt = DateTime.ParseExact(DateTime.Now.ToString("MMMM"), "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None).Month;
            SelectedMonthIndex = thisMonthInt - 1;
            var monthText = DateTime.Now.ToString("MMMM");
            MonthText = $"{monthText} 2026 · 47 transactions";
            ThisMonth = $"{monthText[..3]} 2026 ▼";
            LoadData(); // DUMMY DATA; REMOVE
        }

        private void LoadData()
        {
            // Placeholder for data loading logic
            TableData = new ObservableCollection<DataTableItem>
            {
                new DataTableItem { Date = DateTime.Now, Description = "Grocery Shopping", Tag = "Food", TagVariant = "Negative", Amount = -94.30, State = "● auto" },
                new DataTableItem { Date = DateTime.Now.AddDays(-1), Description = "Salary", Tag = "Income", TagVariant = "Positive", Amount = 4280.00, State = "● auto" },
                new DataTableItem { Date = DateTime.Now.AddDays(-2), Description = "Electricity Bill", Tag = "Utilities", TagVariant = "Warning", Amount = 60.50, State = "⚠ pending" }
            };
        }
    }

    public class DataTableItem
    {
        public required DateTime Date { get; set; }
        public required string Description { get; set; }
        public required string Tag { get; set; }
        public required double Amount { get; set; }
        public required string State { get; set; }
        public required string TagVariant { get; set; }
        public string FormattedAmount => Amount >= 0 ? $"+{Amount:C}" : $"{Amount:C}";
        public bool IsPositiveAmount => Amount >= 0;
        public bool IsNegativeAmount => Amount < 0;
        public bool IsAutoState => State.Contains("auto");
        public bool IsPendingState => State.Contains("pending");
        public bool IsPositive => Amount > 0;
        public bool IsNegative => Amount < 0;
    }
}
