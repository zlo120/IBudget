using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.GUI.Services;
using MongoDB.Bson;

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
        private readonly IMessageService _messageService;

        public DictionariesPageViewModel(IExpenseTagService expenseTagService, IExpenseRuleTagService expenseRuleTagService, IMessageService messageService)
        {
            _expenseTagService = expenseTagService;
            _expenseRuleTagService = expenseRuleTagService;
            _messageService = messageService;
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
                    var container = new InfoContainer() { Key = eD.Title, Value = eD.Tags.First() };
                    container.SetExpenseTagData(eD, _expenseTagService, _messageService, RemoveExpenseTagFromCollection);
                    ExpenseTagsInfo.Add(container);
                }
            }
        }

        private void FinishRdDbSearch()
        {
            if (ExpenseRuleTags is not null)
            {
                foreach (var rD in ExpenseRuleTags)
                {
                    var container = new InfoContainer() { Key = rD.Rule, Value = rD.Tags.First() };
                    container.SetExpenseRuleTagData(rD, _expenseRuleTagService, _messageService, RemoveExpenseRuleTagFromCollection);
                    ExpenseRuleTagsInfo.Add(container);
                }
            }
        }

        private void RemoveExpenseTagFromCollection(InfoContainer item)
        {
            ExpenseTagsInfo.Remove(item);
        }

        private void RemoveExpenseRuleTagFromCollection(InfoContainer item)
        {
            ExpenseRuleTagsInfo.Remove(item);
        }
    }

    public partial class InfoContainer : ObservableObject
    {
        public required string Key { get; set; }
        public required string Value { get; set; }

        // Store references for deletion
        private ExpenseTag? _expenseTag;
        private ObjectId _expenseTagId;
        private ExpenseRuleTag? _expenseRuleTag;
        private ObjectId _expenseRuleTagId;
        private IExpenseTagService? _expenseTagService;
        private IExpenseRuleTagService? _expenseRuleTagService;
        private IMessageService? _messageService;
        private Action<InfoContainer>? _removeFromCollection;

        public void SetExpenseTagData(ExpenseTag expenseTag, IExpenseTagService expenseTagService, IMessageService messageService, Action<InfoContainer> removeFromCollection)
        {
            _expenseTag = expenseTag;
            _expenseTagService = expenseTagService;
            _messageService = messageService;
            _removeFromCollection = removeFromCollection;
            _expenseTagId = (ObjectId)expenseTag.Id!;
        }

        public void SetExpenseRuleTagData(ExpenseRuleTag expenseRuleTag, IExpenseRuleTagService expenseRuleTagService, IMessageService messageService, Action<InfoContainer> removeFromCollection)
        {
            _expenseRuleTag = expenseRuleTag;
            _expenseRuleTagService = expenseRuleTagService;
            _messageService = messageService;
            _removeFromCollection = removeFromCollection;
            _expenseRuleTagId = (ObjectId)expenseRuleTag.Id!;
        }

        [RelayCommand]
        private async Task DeleteExpenseTag()
        {
            if (_expenseTagService == null || _messageService == null) return;

            var confirmation = await _messageService.ShowConfirmationAsync(
                "Delete Expense Dictionary", 
                $"Are you sure you want to delete the expense mapping for '{Key}'? This action cannot be undone.");

            if (confirmation)
            {
                try
                {
                    await _expenseTagService.DeleteExpenseTagById(_expenseTagId);
                    _removeFromCollection?.Invoke(this);
                }
                catch (Exception ex)
                {
                    await _messageService.ShowErrorAsync($"Error deleting expense dictionary: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private async Task DeleteExpenseRuleTag()
        {
            if (_expenseRuleTagService == null || _messageService == null) return;

            var confirmation = await _messageService.ShowConfirmationAsync(
                "Delete Rule Dictionary", 
                $"Are you sure you want to delete the rule '{Key}'? This action cannot be undone.");

            if (confirmation)
            {
                try
                {
                    await _expenseRuleTagService.DeleteExpenseRuleTagById(_expenseRuleTagId);
                    _removeFromCollection?.Invoke(this);
                }
                catch (Exception ex)
                {
                    await _messageService.ShowErrorAsync($"Error deleting rule dictionary: {ex.Message}");
                }
            }
        }
    }
}
