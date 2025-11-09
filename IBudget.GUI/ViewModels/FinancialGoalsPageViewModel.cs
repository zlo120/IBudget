using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using IBudget.GUI.Services;
using IBudget.GUI.Services.Impl;

namespace IBudget.GUI.ViewModels
{
    public partial class FinancialGoalsPageViewModel : ViewModelBase
    {
        private readonly IFinancialGoalService _financialGoalService;
        private readonly ITagService _tagService;
        private readonly IMessageService _messageService;
        public required ObservableCollection<string> Tags { get; set; }
        public ObservableCollection<FinancialGoalListItem> FinancialGoals { get; } = [];
        [ObservableProperty]
        private string _goalName = string.Empty;
        [ObservableProperty]
        private string _amount = string.Empty;
        [ObservableProperty]
        private string _message = string.Empty;
        [ObservableProperty]
        private bool _isLoading = true;
        public FinancialGoalsPageViewModel(IFinancialGoalService financialGoalService, ITagService tagService, IMessageService messageService)
        {
            _financialGoalService = financialGoalService;
            _tagService = tagService;
            _messageService = messageService;
            _ = InitializeFinancialGoalsAsync();
        }

        private void RemoveGoalFromCollection()
        {
            _ = InitializeFinancialGoalsAsync();
        }
        
        private async Task InitializeFinancialGoalsAsync()
        {
            try
            {
                FinancialGoals.Clear();
                var financialGoals = await _financialGoalService.GetAll();
                foreach (var financialGoal in financialGoals)
                {
                    FinancialGoals.Add(new FinancialGoalListItem(RemoveGoalFromCollection, _financialGoalService, _messageService)
                    {
                        Key = financialGoal.Name,
                        Value = financialGoal.TargetAmount.ToString("C"),
                    });
                }
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error loading financial goals: {ex.Message}");
            }

            try
            {
                var tags = await _tagService.GetAll();
                Tags = new ObservableCollection<string>(tags.ConvertAll(t => t.Name));
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error loading tags: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task CreateGoal()
        {
            if (GoalName == string.Empty) return;
            if (Amount == string.Empty) return;
            var tagName = GoalName.ToLower();
            if (!decimal.TryParse(Amount, out var amount))
            {
                await _messageService.ShowErrorAsync($"{Amount} is an invalid amount");
                return;
            }

            var tag = await _tagService.GetOrCreateTagByName(tagName);
            if (tag is null)
            {
                await _tagService.CreateTag(new Tag() { Name = tagName, IsTracked = true, CreatedAt = DateTime.Now });
            }
            else if (!tag.IsTracked)
            {
                await _tagService.UpdateTag(new Tag()
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    IsTracked = true,
                    CreatedAt = tag.CreatedAt,
                });
            }

            try
            {
                await _financialGoalService.CreateFinancialGoal(new FinancialGoal()
                {
                    Name = tagName,
                    TargetAmount = amount,
                    CreatedAt = DateTime.Now,
                });
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error creating financial goal: {ex.Message}");
                return;
            }
            GoalName = string.Empty;
            Amount = string.Empty;
            await InitializeFinancialGoalsAsync();
        }

        public void RefreshView()
        {
            _ = InitializeFinancialGoalsAsync();
        }
    }

    public partial class FinancialGoalListItem(Action removeFromCollection, IFinancialGoalService financialGoalService, IMessageService messageService)
    {
        private readonly Action _removeFromCollection = removeFromCollection;
        private readonly IFinancialGoalService _financialGoalService = financialGoalService;
        private readonly IMessageService _messageService = messageService;
        public string Key { get; set; }
        public string Value { get; set; }
        [RelayCommand]
        public async Task DeleteGoal(string goalName)
        {
            var confirmation = await _messageService.ShowConfirmationAsync("Delete Financial Goal", $"Are you sure you want to delete the financial goal '{goalName}'? This action cannot be undone.");
            if (confirmation)
            {
                try
                    {
                
                    await _financialGoalService.DeleteFinancialGoalByName(goalName);
                    _removeFromCollection();
                
                }
                catch (Exception ex)
                {
                    await _messageService.ShowErrorAsync($"Error deleting financial goal: {ex.Message}");
                }
            }
        }
    }
}
