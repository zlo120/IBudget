using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using IBudget.GUI.Services;

namespace IBudget.GUI.ViewModels.DataView
{
    public partial class DataTableViewModel : ViewModelBase
    {
        private readonly ISummaryService _summaryService;
        private readonly IMessageService _messageService;
        private readonly ITagService _tagService;

        [ObservableProperty]
        private int _selectedMonthIndex = 0;

        [ObservableProperty]
        private int _selectedWeekIndex = 0;

        [ObservableProperty]
        private ObservableCollection<string> _weekOptions = [];

        public List<DateTime> BeginningOfWeeks { get; set; } = [];

        [ObservableProperty]
        private bool _isViewingMonth = true;

        [ObservableProperty]
        private bool _isViewingExpenses = true;

        [ObservableProperty]
        private ObservableCollection<DataTableItem> _tableData = [];

        // Static converters for data binding
        public static readonly IValueConverter ExpenseIncomeConverter = new FuncValueConverter<bool, string>(
            isExpense => isExpense ? "Expense" : "Income");

        public static readonly IValueConverter ExpenseIncomeLowerConverter = new FuncValueConverter<bool, string>(
            isExpense => isExpense ? "expense" : "income");

        public static readonly IValueConverter MonthWeekLowerConverter = new FuncValueConverter<bool, string>(
            isMonth => isMonth ? "monthly" : "weekly");

        public static readonly IValueConverter MonthWeekToggleConverter = new FuncValueConverter<bool, string>(
            isMonth => isMonth ? "Week" : "Month");

        public static readonly IValueConverter ExpenseIncomeToggleConverter = new FuncValueConverter<bool, string>(
            isExpense => isExpense ? "Incomes" : "Expenses");

        public DataTableViewModel(ISummaryService summaryService, IMessageService messageService, ITagService tagService)
        {
            _summaryService = summaryService;
            _messageService = messageService;
            _tagService = tagService;

            var thisMonthInt = DateTime.ParseExact(DateTime.Now.ToString("MMMM"), "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None).Month;
            SelectedMonthIndex = thisMonthInt - 1;
            _ = LoadMonthDataAsync(thisMonthInt + 1);

            var year = Core.Utils.Calendar.InitiateCalendar();
            foreach (var month in year.Months)
            {
                foreach (var week in month.Weeks)
                {
                    WeekOptions.Add(week.Label);
                    BeginningOfWeeks.Add(week.Start);
                }
            }

            var thisWeek = Core.Utils.Calendar.GetBeginningOfWeek(DateTime.Now);
            SelectedWeekIndex = BeginningOfWeeks.FindIndex(d => d == thisWeek);
        }

        [RelayCommand]
        private void ToggleViewPeriod()
        {
            IsViewingMonth = !IsViewingMonth;
        }

        [RelayCommand]
        private void ToggleExpenseIncome()
        {
            IsViewingExpenses = !IsViewingExpenses;
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                if (IsViewingMonth)
                {
                    await LoadMonthDataAsync(SelectedMonthIndex + 1);
                }
                else
                {
                    var selectedDate = BeginningOfWeeks.ElementAtOrDefault(SelectedWeekIndex);
                    if (selectedDate != default)
                    {
                        await LoadWeekData(selectedDate);
                    }
                }
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error initializing data: {ex.Message}");
            }
        }

        partial void OnSelectedMonthIndexChanged(int value)
        {
            if (_summaryService is null || _messageService is null) return;
            try
            {
                _ = LoadMonthDataAsync(value + 1);
            }
            catch (Exception ex)
            {
                Task.Run(async () => await _messageService.ShowErrorAsync($"Error loading data for month {value + 1}: {ex.Message}"));
            }
        }

        partial void OnSelectedWeekIndexChanged(int newValue)
        {
            if (_summaryService is null || _messageService is null) return;
            _ = InitializeDataAsync();
        }

        partial void OnIsViewingMonthChanged(bool oldValue, bool newValue)
        {
            if (_summaryService is null || _messageService is null) return;
            try
            {
                _ = InitializeDataAsync();
            }
            catch (Exception ex)
            {
                Task.Run(async () => await _messageService.ShowErrorAsync($"Error when toggling month view.\n{ex.Message}: {ex.StackTrace}"));
            }
        }

        partial void OnIsViewingExpensesChanged(bool oldValue, bool newValue)
        {
            if (_summaryService is null || _messageService is null) return;
            try
            {
                _ = InitializeDataAsync();
            }
            catch (Exception ex)
            {
                Task.Run(async () => await _messageService.ShowErrorAsync($"Error when toggling expense/income view.\n{ex.Message}: {ex.StackTrace}"));
            }
        }

        private async Task LoadMonthDataAsync(int monthNumber)
        {
            try
            {
                var ignoredTag = await _tagService.GetOrCreateTagByName("ignored");
                var monthsData = await _summaryService.ReadMonth(monthNumber);

                if (monthsData?.AllExpenses == null && monthsData?.AllIncome == null)
                {
                    TableData.Clear();
                    return;
                }

                if (IsViewingExpenses)
                {
                    var expenses = monthsData?.AllExpenses.Where(e => !e.Tags.Contains(ignoredTag)) ?? [];
                    TableData = new ObservableCollection<DataTableItem>(expenses.Select(e => new DataTableItem
                    {
                        Tag = e.Tags?.FirstOrDefault()?.Name ?? "No Tag",
                        Amount = e.Amount,
                        Date = e.Date,
                        Description = e.Notes ?? string.Empty
                    }));
                }
                else
                {
                    var incomes = monthsData?.AllIncome.Where(i => !i.Tags.Contains(ignoredTag)) ?? [];
                    TableData = new ObservableCollection<DataTableItem>(incomes.Select(i => new DataTableItem
                    {
                        Tag = i.Tags?.FirstOrDefault()?.Name ?? "No Tag",
                        Amount = i.Amount,
                        Date = i.Date,
                        Description = i.Source ?? string.Empty
                    }));
                }
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error loading data for month {monthNumber}: {ex.Message}");
            }
        }

        private async Task LoadWeekData(DateTime date)
        {
            try
            {
                var ignoredTag = await _tagService.GetOrCreateTagByName("ignored");
                var weeksData = await _summaryService.ReadWeek(date);

                if (weeksData?.Expenses == null && weeksData?.Income == null)
                {
                    TableData.Clear();
                    return;
                }

                if (IsViewingExpenses)
                {
                    var expenses = weeksData?.Expenses.Where(e => !e.Tags.Contains(ignoredTag)) ?? [];
                    TableData = new ObservableCollection<DataTableItem>(expenses.Select(e => new DataTableItem
                    {
                        Tag = e.Tags?.FirstOrDefault()?.Name ?? "No Tag",
                        Amount = e.Amount,
                        Date = e.Date,
                        Description = e.Notes ?? string.Empty
                    }));
                }
                else
                {
                    var incomes = weeksData?.Income.Where(i => !i.Tags.Contains(ignoredTag)) ?? [];
                    TableData = new ObservableCollection<DataTableItem>(incomes.Select(i => new DataTableItem
                    {
                        Tag = i.Tags?.FirstOrDefault()?.Name ?? "No Tag",
                        Amount = i.Amount,
                        Date = i.Date,
                        Description = i.Source ?? string.Empty
                    }));
                }
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error loading data for the date {date}: {ex.Message}");
            }
        }

        public void RefreshView()
        {
            _ = InitializeDataAsync();
        }
    }

    public class DataTableItem
    {
        public required string Tag { get; set; }
        public required double Amount { get; set; }
        public required DateTime Date { get; set; }
        public required string Description { get; set; }
    }
}
