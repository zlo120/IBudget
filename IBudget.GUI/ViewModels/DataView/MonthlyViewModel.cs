using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Services;
using IBudget.GUI.Services;

namespace IBudget.GUI.ViewModels.DataView
{
    public partial class MonthlyViewModel : ViewModelBase
    {
        private readonly ISummaryService _summaryService;
        private readonly IMessageService _messageService;
        private readonly IFinancialGoalService _financialGoalService;
        private readonly ITagService _tagService;

        [ObservableProperty]
        private string _thisMonth = string.Empty;

        [ObservableProperty]
        private int _selectedIndex = 0;

        [ObservableProperty]
        private ObservableCollection<SummaryItem> _summaryItems = new();

        [ObservableProperty]
        private ObservableCollection<string> _financialGoals = new();

        [ObservableProperty]
        private Dictionary<string, double> _summarySpending = new(); // not used in UI currently

        [ObservableProperty]
        private double _totalSpending = 0.0;

        [ObservableProperty]
        private double _totalIncome = 0.0;

        public MonthlyViewModel(ITagService tagService, ISummaryService summaryService, IMessageService messageService, IFinancialGoalService financialGoalService)
        {
            ThisMonth = DateTime.Now.ToString("MMMM");
            var thisMonthInt = DateTime.ParseExact(ThisMonth, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None).Month;
            SelectedIndex = thisMonthInt - 1;

            _summaryService = summaryService;
            _messageService = messageService;
            _financialGoalService = financialGoalService;
            _tagService = tagService;

            _ = InitializeDataAsync();
        }

        partial void OnSelectedIndexChanged(int value)
        {
            if (_financialGoalService is null || _messageService is null) return;
            // Update the month when selection changes and reload data
            var monthNames = new[]
            {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            };

            try
            {
                if (value >= 0 && value < monthNames.Length)
                {
                    ThisMonth = monthNames[value];
                    _ = LoadMonthDataAsync(value + 1);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                var thisMonthInt = DateTime.ParseExact(ThisMonth, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None).Month;
                await LoadMonthDataAsync(thisMonthInt);
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error initializing monthly data: {ex.Message}");
            }
        }

        private async Task LoadMonthDataAsync(int monthNumber)
        {
            try
            {
                var ignoredTag = await _tagService.GetOrCreateTagByName("ignored");

                // Clear existing data
                SummaryItems.Clear();
                FinancialGoals.Clear();
                SummarySpending.Clear();
                TotalSpending = 0.0;
                TotalIncome = 0.0;

                var financialGoalsResult = await _financialGoalService.GetAll();
                var financialGoals = financialGoalsResult?.Select(goal => goal.Name) ?? Enumerable.Empty<string>();
                var monthsData = await _summaryService.ReadMonth(monthNumber);
                var allExpenses = monthsData?.AllExpenses.Where(e => !e.Tags.Contains(ignoredTag));
                var allIncome = monthsData?.AllIncome.Where(i => !i.Tags.Contains(ignoredTag));
                if (allExpenses is null && allIncome is null)
                {
                    // No data available for this month
                    return;
                }

                foreach (var goal in financialGoals)
                {
                    if (string.IsNullOrEmpty(goal)) continue;

                    FinancialGoals.Add(goal);
                    double totalMoneySpent = allExpenses
                        .Where(e => e.Tags?.Select(t => t.Name).Contains(goal) == true)
                        .Select(e => e.Amount)
                        .Sum();
                    SummarySpending.Add(goal, totalMoneySpent);
                    SummaryItems.Add(new SummaryItem($"Total spending on {goal}", totalMoneySpent));
                }

                TotalSpending = allExpenses.Select(expense => expense.Amount).Sum();
                TotalIncome = allIncome.Select(income => income.Amount).Sum();
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error loading data for month {monthNumber}: {ex.Message}");
            }
        }

        public void RefreshView()
        {
            _ = InitializeDataAsync();
        }
    }

    public class SummaryItem
    {
        public SummaryItem(string title, double value)
        {
            SummaryTitle = title;
            SummaryValue = $"${value:F2}";
        }
        public string SummaryTitle { get; set; }
        public string SummaryValue { get; set; }
    }
}