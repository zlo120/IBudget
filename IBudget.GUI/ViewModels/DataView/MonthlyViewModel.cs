using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace IBudget.GUI.ViewModels.DataView
{
    public partial class MonthlyViewModel : ViewModelBase
    {
        private readonly ITagService _tagService;
        private readonly ISummaryService _summaryService;

        [ObservableProperty]
        private string _thisMonth = string.Empty;
        [ObservableProperty]
        private int _selectedIndex = 0;

        [ObservableProperty]
        private ObservableCollection<SummaryItem> _summaryItems = new();

        [ObservableProperty]
        private ObservableCollection<string> _trackedTags = new();

        [ObservableProperty]
        private Dictionary<string, double> _summaryChartData = new();

        public MonthlyViewModel(ITagService tagService, ISummaryService summaryService)
        {
            ThisMonth = DateTime.Now.ToString("MMMM");
            var thisMonthInt = DateTime.ParseExact(ThisMonth, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None).Month;
            SelectedIndex = thisMonthInt - 1;
            
            for (int i = 0; i < 50; i++)
            {
                SummaryItems.Add(new SummaryItem($"Summary value {i + 1}", 22.22));
            }

            _tagService = tagService;
            _summaryService = summaryService;
            
            _ = InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                var thisMonthInt = DateTime.ParseExact(ThisMonth, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None).Month;
                
                var trackedTags = (await _tagService.GetAll())
                    .Where(tag => tag.IsTracked)
                    .Select(tag => tag.Name)
                    .OrderBy(s => s)
                    .ToList();

                var monthsData = await _summaryService.ReadMonth(thisMonthInt);
                
                foreach (var tag in trackedTags)
                {
                    TrackedTags.Add(tag);
                    double totalMoneySpent = monthsData.AllExpenses
                        .Where(e => e.Tags!.Select(t => t.Name).Contains(tag))
                        .Select(e => e.Amount)
                        .Sum();
                    SummaryChartData.Add(tag, totalMoneySpent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing monthly data: {ex.Message}");
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