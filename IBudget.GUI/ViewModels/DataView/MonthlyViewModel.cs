using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using IBudget.GUI.Services;
using MongoDB.Bson;

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
        private ObservableCollection<TrackedTagSpendingItem> _trackedTagSpending = new();

        [ObservableProperty]
        private ObservableCollection<FinancialGoalSpendingItem> _financialGoalSpending = new();

        [ObservableProperty]
        private double _otherSpending = 0.0;

        [ObservableProperty]
        private double _totalSpending = 0.0;

        [ObservableProperty]
        private double _totalIncome = 0.0;

        [ObservableProperty]
        private double _difference = 0.0;

        [ObservableProperty]
        private bool _isDeficit = false;

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
                // Clear existing data
                TrackedTagSpending.Clear();
                FinancialGoalSpending.Clear();
                TotalSpending = 0.0;
                TotalIncome = 0.0;
                OtherSpending = 0.0;

                // Get all tracked tags and financial goals
                var allTags = await _tagService.GetAll();
                var trackedTags = allTags.Where(tag => tag.IsTracked).ToList();
                var financialGoals = await _financialGoalService.GetAll();
                var financialGoalNames = financialGoals?.Select(g => g.Name.ToLower()).ToHashSet() ?? new HashSet<string>();

                // Get month data
                var monthsData = await _summaryService.ReadMonth(monthNumber);
                var allExpenses = monthsData?.AllExpenses.Where(e => !e.IsIgnored).ToList();
                var allIncome = monthsData?.AllIncome.Where(i => !i.IsIgnored).ToList();
                
                if (allExpenses is null || !allExpenses.Any())
                {
                    // No data available for this month
                    return;
                }

                // Calculate total spending and income
                TotalSpending = allExpenses.Sum(e => e.Amount);
                TotalIncome = allIncome?.Sum(i => i.Amount) ?? 0.0;
                
                // Calculate difference (Income - Expenses)
                Difference = TotalIncome - TotalSpending;
                IsDeficit = Difference < 0;

                // Track which expenses have been categorized
                var categorizedExpenseIds = new HashSet<ObjectId?>();

                // Process financial goals (tracked tags that have financial goals)
                if (financialGoals != null)
                {
                    foreach (var goal in financialGoals)
                    {
                        var goalExpenses = allExpenses
                            .Where(e => e.Tags?.Any(t => t.Name.Equals(goal.Name, StringComparison.OrdinalIgnoreCase)) == true)
                            .ToList();

                        var actualSpending = goalExpenses.Sum(e => e.Amount);
                        var difference = (double)goal.TargetAmount - actualSpending;
                        var isOverBudget = actualSpending > (double)goal.TargetAmount;

                        FinancialGoalSpending.Add(new FinancialGoalSpendingItem
                        {
                            TagName = goal.Name,
                            GoalAmount = (double)goal.TargetAmount,
                            ActualSpending = actualSpending,
                            Difference = Math.Abs(difference),
                            IsOverBudget = isOverBudget
                        });

                        // Mark these expenses as categorized
                        foreach (var expense in goalExpenses)
                        {
                            categorizedExpenseIds.Add(expense.Id);
                        }
                    }
                }

                // Process tracked tags that don't have financial goals
                foreach (var tag in trackedTags)
                {
                    // Skip if this tag is already a financial goal
                    if (financialGoalNames.Contains(tag.Name.ToLower()))
                        continue;

                    var tagExpenses = allExpenses
                        .Where(e => e.Tags?.Any(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)) == true)
                        .ToList();

                    if (tagExpenses.Any())
                    {
                        var spending = tagExpenses.Sum(e => e.Amount);
                        TrackedTagSpending.Add(new TrackedTagSpendingItem
                        {
                            TagName = tag.Name,
                            TotalSpending = spending
                        });

                        // Mark these expenses as categorized
                        foreach (var expense in tagExpenses)
                        {
                            categorizedExpenseIds.Add(expense.Id);
                        }
                    }
                }

                // Calculate "Other" spending (expenses not in any tracked tag)
                var otherExpenses = allExpenses
                    .Where(e => !categorizedExpenseIds.Contains(e.Id))
                    .ToList();
                
                OtherSpending = otherExpenses.Sum(e => e.Amount);
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

    public class TrackedTagSpendingItem
    {
        public string TagName { get; set; } = string.Empty;
        public double TotalSpending { get; set; }
        public string FormattedSpending => $"${TotalSpending:F2}";
    }

    public class FinancialGoalSpendingItem
    {
        public string TagName { get; set; } = string.Empty;
        public double GoalAmount { get; set; }
        public double ActualSpending { get; set; }
        public double Difference { get; set; }
        public bool IsOverBudget { get; set; }
        
        public string FormattedGoalAmount => $"${GoalAmount:F2}";
        public string FormattedActualSpending => $"${ActualSpending:F2}";
        public string FormattedDifference => $"${Difference:F2}";
        public string DifferenceLabel => IsOverBudget ? "Over Budget" : "Under Budget";
    }
}