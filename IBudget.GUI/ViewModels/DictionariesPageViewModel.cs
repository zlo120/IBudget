using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.GUI.ViewModels
{
    public partial class DictionariesPageViewModel : ViewModelBase
    {
        public ObservableCollection<InfoContainer> ExpenseTagsInfo { get; } = new();
        public ObservableCollection<InfoContainer> ExpenseRuleTagsInfo { get; } = new();

        public List<ExpenseTag> ExpenseTags { get; set; }
        public List<ExpenseRuleTag> ExpenseRuleTags { get; set; }

        [ObservableProperty]
        private bool _isLoadingED = true;

        [ObservableProperty]
        private bool _isLoadingRD = true;
        private readonly IExpenseTagService _expenseTagService;
        private readonly IExpenseRuleTagService _expenseRuleTagService;

        public DictionariesPageViewModel(IExpenseTagService expenseTagService, IExpenseRuleTagService expenseRuleTagService)
        {
            _expenseTagService = expenseTagService;
            _expenseRuleTagService = expenseRuleTagService;
            _ = InitializeDbSearchAsync(); // Change to async Task
        }

        private async Task InitializeDbSearchAsync() // Changed from async void
        {
            ExpenseTagsInfo.Clear();
            ExpenseRuleTagsInfo.Clear();
            try
            {
                var expenseTagsTask = GetExpenseTagsAsync();
                var expenseRuleTagsTask = GetExpenseRuleTagsAsync();

                await Task.WhenAll(expenseTagsTask, expenseRuleTagsTask);

                ExpenseTags = await expenseTagsTask;
                ExpenseRuleTags = await expenseRuleTagsTask;

                FinishEdDbSearch();
                IsLoadingED = false;

                FinishRdDbSearch();
                IsLoadingRD = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing dictionary data: {ex.Message}");
                IsLoadingED = false;
                IsLoadingRD = false;
            }
        }

        public void RefreshView()
        {
            _ = InitializeDbSearchAsync(); // Use the async version
        }

        private async Task<List<ExpenseTag>> GetExpenseTagsAsync()
        {
            return await _expenseTagService.GetAllExpenseTags();
        }

        private async Task<List<ExpenseRuleTag>> GetExpenseRuleTagsAsync()
        {
            return await _expenseRuleTagService.GetAllExpenseRuleTags();
        }

        private void FinishEdDbSearch()
        {
            if (ExpenseTags is not null)
            {
                foreach (var eD in ExpenseTags)
                {
                    ExpenseTagsInfo.Add(new InfoContainer() { Key = eD.Title, Value = eD.Tags.First() });
                }
            }
        }

        private void FinishRdDbSearch()
        {
            if (ExpenseRuleTags is not null)
            {
                foreach (var rD in ExpenseRuleTags)
                {
                    ExpenseRuleTagsInfo.Add(new InfoContainer() { Key = rD.Rule, Value = rD.Tags.First() });
                }
            }
        }
    }

    public class InfoContainer
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
