using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ManualEntryPageViewModel : ViewModelBase
    {
        private readonly IExpenseService _expenseService;
        private readonly IIncomeService _incomeService;
        private readonly ITagService _tagService;
        private readonly IBatchHashService _batchHashService;
        private readonly IMessageService _messageService;

        [ObservableProperty]
        private bool _isExpenseSelected = true;

        [ObservableProperty]
        private DateTimeOffset? _selectedDate = DateTimeOffset.Now.Date;

        [ObservableProperty]
        private double _amount;

        [ObservableProperty]
        private string _notes = string.Empty;

        [ObservableProperty]
        private string _source = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _existingTags = new();

        [ObservableProperty]
        private ObservableCollection<Tag> _selectedTags = new();

        [ObservableProperty]
        private string _currentTag = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        public string SubmitButtonText => IsExpenseSelected ? "Add Expense" : "Add Income";

        partial void OnIsExpenseSelectedChanged(bool value)
        {
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        public ManualEntryPageViewModel(
            IExpenseService expenseService,
            IIncomeService incomeService,
            ITagService tagService,
            IBatchHashService batchHashService,
            IMessageService messageService)
        {
            _expenseService = expenseService;
            _incomeService = incomeService;
            _tagService = tagService;
            _batchHashService = batchHashService;
            _messageService = messageService;
        }

        public async void RefreshView()
        {
            await LoadTagsAsync();
        }

        private async Task LoadTagsAsync()
        {
            try
            {
                IsLoading = true;
                var tags = await _tagService.GetAll();
                ExistingTags = new ObservableCollection<string>(tags.OrderBy(t => t.Name).Select(t => t.Name));
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Failed to load tags: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void SwitchToExpense()
        {
            IsExpenseSelected = true;
            ClearForm();
        }

        [RelayCommand]
        private void SwitchToIncome()
        {
            IsExpenseSelected = false;
            ClearForm();
        }

        [RelayCommand]
        private async Task AddTag()
        {
            if (string.IsNullOrWhiteSpace(CurrentTag))
                return;

            // Get or create tag
            var tag = await _tagService.GetOrCreateTagByName(CurrentTag);
            
            if (!SelectedTags.Any(t => t.Name == tag.Name))
            {
                SelectedTags.Add(tag);
            }
            
            CurrentTag = string.Empty;
        }

        [RelayCommand]
        private void RemoveTag(Tag tag)
        {
            SelectedTags.Remove(tag);
        }

        [RelayCommand]
        private async Task SubmitEntry()
        {
            try
            {
                // Validate input
                if (Amount <= 0)
                {
                    await _messageService.ShowErrorAsync("Amount must be greater than 0");
                    return;
                }

                if (!SelectedDate.HasValue)
                {
                    await _messageService.ShowErrorAsync("Date is required");
                    return;
                }

                if (!SelectedTags.Any())
                {
                    await _messageService.ShowErrorAsync("Please select at least one tag");
                    return;
                }

                // Validate expense/income specific fields
                if (IsExpenseSelected && string.IsNullOrWhiteSpace(Notes))
                {
                    await _messageService.ShowErrorAsync("Notes are required for expenses");
                    return;
                }

                if (!IsExpenseSelected && string.IsNullOrWhiteSpace(Source))
                {
                    await _messageService.ShowErrorAsync("Source is required for income");
                    return;
                }

                IsLoading = true;
                StatusMessage = "Saving...";

                // Generate a unique batch hash for this manual entry
                var batchHash = _batchHashService.ComputeBatchHash($"manual_{DateTime.Now.Ticks}");

                if (IsExpenseSelected)
                {
                    await SaveExpenseAsync(batchHash);
                }
                else
                {
                    await SaveIncomeAsync(batchHash);
                }

                StatusMessage = $"{(IsExpenseSelected ? "Expense" : "Income")} added successfully!";
                await Task.Delay(2000); // Show success message for 2 seconds
                ClearForm();
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Failed to save entry: {ex.Message}");
                StatusMessage = string.Empty;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveExpenseAsync(string batchHash)
        {
            var expense = new Expense
            {
                Id = ObjectId.GenerateNewId(),
                Date = SelectedDate!.Value.Date,
                Amount = Amount,
                Tags = SelectedTags.ToList(),
                Notes = Notes,
                BatchHash = batchHash,
                CreatedAt = DateTime.UtcNow,
                IsIgnored = false,
                Frequency = null
            };

            await _expenseService.AddExpense(expense);
        }

        private async Task SaveIncomeAsync(string batchHash)
        {
            var income = new Income
            {
                Id = ObjectId.GenerateNewId(),
                Date = SelectedDate!.Value.Date,
                Amount = Amount,
                Tags = SelectedTags.ToList(),
                Source = Source,
                BatchHash = batchHash,
                CreatedAt = DateTime.UtcNow,
                IsIgnored = false,
                Frequency = null
            };

            await _incomeService.AddIncome(income);
        }

        [RelayCommand]
        private void ClearForm()
        {
            Amount = 0;
            Notes = string.Empty;
            Source = string.Empty;
            SelectedTags.Clear();
            CurrentTag = string.Empty;
            StatusMessage = string.Empty;
            SelectedDate = DateTimeOffset.Now.Date;
        }
    }
}
