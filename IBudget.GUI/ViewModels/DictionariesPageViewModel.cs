using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.DatabaseModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.GUI.Services;
using MongoDB.Bson;

namespace IBudget.GUI.ViewModels
{
    public enum EditType
    {
        ExpenseTag,
        ExpenseRuleTag
    }

    public class EditTypeToFontFamilyConverter : IValueConverter
    {
        public static readonly EditTypeToFontFamilyConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is EditType editType)
            {
                return editType == EditType.ExpenseRuleTag ? "Consolas, 'Courier New', monospace" : "Default";
            }
            return "Default";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class DictionariesPageViewModel : ViewModelBase
    {
        public ObservableCollection<InfoContainer> ExpenseTagsInfo { get; } = new();
        public ObservableCollection<InfoContainer> ExpenseRuleTagsInfo { get; } = new();

        public List<ExpenseTag> ExpenseTags { get; set; }
        public List<ExpenseRuleTag> ExpenseRuleTags { get; set; }
        public List<Tag> AllTags { get; set; } = new();

        [ObservableProperty]
        private bool _isLoadingED = true;

        [ObservableProperty]
        private bool _isLoadingRD = true;

        [ObservableProperty]
        private bool _isEditDialogOpen = false;

        [ObservableProperty]
        private string _editItemName = string.Empty;

        [ObservableProperty]
        private string _selectedTag = string.Empty;

        [ObservableProperty]
        private InfoContainer? _currentEditingItem;

        [ObservableProperty]
        private EditType _currentEditType;

        [ObservableProperty]
        private string _dialogTitle = string.Empty;

        [ObservableProperty]
        private string _dialogSubtitle = string.Empty;

        [ObservableProperty]
        private string _itemFieldLabel = string.Empty;

        [ObservableProperty]
        private bool _isIgnored = false;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _isSearchActive = false;

        [ObservableProperty]
        private bool _hasMoreExpenseTags = false;

        [ObservableProperty]
        private bool _hasMoreExpenseRuleTags = false;

        [ObservableProperty]
        private int _expenseTagsCurrentPage = 1;

        [ObservableProperty]
        private int _expenseRuleTagsCurrentPage = 1;

        public ObservableCollection<string> AvailableTags { get; } = new();

        private readonly IExpenseTagService _expenseTagService;
        private readonly IExpenseRuleTagService _expenseRuleTagService;
        private readonly IMessageService _messageService;
        private readonly ITagService _tagService;
        private readonly System.Timers.Timer _searchDebounceTimer;

        public DictionariesPageViewModel(IExpenseTagService expenseTagService, IExpenseRuleTagService expenseRuleTagService, IMessageService messageService, ITagService tagService)
        {
            _expenseTagService = expenseTagService;
            _expenseRuleTagService = expenseRuleTagService;
            _messageService = messageService;
            _tagService = tagService;
            
            // Initialize debounce timer (500ms delay)
            _searchDebounceTimer = new System.Timers.Timer(500);
            _searchDebounceTimer.Elapsed += OnSearchDebounceTimerElapsed;
            _searchDebounceTimer.AutoReset = false;
            
            _ = InitializeDbSearchAsync(); // Change to async Task
        }

        partial void OnSearchTextChanged(string value)
        {
            // Reset and restart the debounce timer
            _searchDebounceTimer.Stop();
            
            if (!string.IsNullOrWhiteSpace(value))
            {
                _searchDebounceTimer.Start();
            }
            else
            {
                // If search text is empty, reset immediately
                _ = ResetSearch();
            }
        }

        private async void OnSearchDebounceTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            await PerformSearch();
        }

        [RelayCommand]
        private async Task PerformSearch()
        {
            var currentSearchText = SearchText;
            if (string.IsNullOrWhiteSpace(currentSearchText))
            {
                await ResetSearch();
                return;
            }

            // Reset pagination when performing new search
            ExpenseTagsCurrentPage = 1;
            ExpenseRuleTagsCurrentPage = 1;

            IsLoadingED = true;
            IsLoadingRD = true;
            IsSearchActive = true;

            try
            {
                var expenseTagsTask = _expenseTagService.Search(currentSearchText, ExpenseTagsCurrentPage);
                var expenseRuleTagsTask = _expenseRuleTagService.Search(currentSearchText, ExpenseRuleTagsCurrentPage);

                await Task.WhenAll(expenseTagsTask, expenseRuleTagsTask);

                var expenseTagsResult = await expenseTagsTask;
                var expenseRuleTagsResult = await expenseRuleTagsTask;

                ExpenseTags = expenseTagsResult.Data.ToList();
                ExpenseRuleTags = expenseRuleTagsResult.Data.ToList();
                HasMoreExpenseTags = expenseTagsResult.HasMoreData;
                HasMoreExpenseRuleTags = expenseRuleTagsResult.HasMoreData;

                UpdateExpenseTagsCollection();
                UpdateExpenseRuleTagsCollection();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error performing search: {ex.Message}");
                await _messageService.ShowErrorAsync($"Error performing search: {ex.Message}");
            }
            finally
            {
                IsLoadingED = false;
                IsLoadingRD = false;
            }
        }

        [RelayCommand]
        private async Task LoadMoreExpenseTags()
        {
            if (!HasMoreExpenseTags || IsLoadingED) return;

            IsLoadingED = true;
            ExpenseTagsCurrentPage++;

            try
            {
                PaginatedResponse<ExpenseTag> result;
                if (IsSearchActive)
                {
                    result = await _expenseTagService.Search(SearchText, ExpenseTagsCurrentPage);
                }
                else
                {
                    result = await _expenseTagService.GetExpenseTagByPage(ExpenseTagsCurrentPage);
                }

                foreach (var tag in result.Data)
                {
                    ExpenseTags.Add(tag);
                    var container = new InfoContainer() { Key = tag.Title, Value = tag.Tags.First(), IsIgnored = tag.IsIgnored };
                    container.SetExpenseTagData(tag, _expenseTagService, _messageService, RemoveExpenseTagFromCollection);
                    ExpenseTagsInfo.Add(container);
                }

                HasMoreExpenseTags = result.HasMoreData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading more expense tags: {ex.Message}");
                await _messageService.ShowErrorAsync($"Error loading more expense tags: {ex.Message}");
            }
            finally
            {
                IsLoadingED = false;
            }
        }

        [RelayCommand]
        private async Task LoadMoreExpenseRuleTags()
        {
            if (!HasMoreExpenseRuleTags || IsLoadingRD) return;

            IsLoadingRD = true;
            ExpenseRuleTagsCurrentPage++;

            try
            {
                PaginatedResponse<ExpenseRuleTag> result;
                if (IsSearchActive)
                {
                    result = await _expenseRuleTagService.Search(SearchText, ExpenseRuleTagsCurrentPage);
                }
                else
                {
                    result = await _expenseRuleTagService.GetExpenseRuleTagByPage(ExpenseRuleTagsCurrentPage);
                }

                foreach (var tag in result.Data)
                {
                    ExpenseRuleTags.Add(tag);
                    var container = new InfoContainer() { Key = tag.Rule, Value = tag.Tags.First(), IsIgnored = tag.IsIgnored };
                    container.SetExpenseRuleTagData(tag, _expenseRuleTagService, _messageService, RemoveExpenseRuleTagFromCollection);
                    ExpenseRuleTagsInfo.Add(container);
                }

                HasMoreExpenseRuleTags = result.HasMoreData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading more expense rule tags: {ex.Message}");
                await _messageService.ShowErrorAsync($"Error loading more expense rule tags: {ex.Message}");
            }
            finally
            {
                IsLoadingRD = false;
            }
        }

        [RelayCommand]
        private async Task ResetSearch()
        {
            SearchText = string.Empty;
            IsSearchActive = false;
            _searchDebounceTimer.Stop();

            // Reset pagination
            ExpenseTagsCurrentPage = 1;
            ExpenseRuleTagsCurrentPage = 1;

            IsLoadingED = true;
            IsLoadingRD = true;

            try
            {
                var expenseTagsTask = _expenseTagService.GetExpenseTagByPage(ExpenseTagsCurrentPage);
                var expenseRuleTagsTask = _expenseRuleTagService.GetExpenseRuleTagByPage(ExpenseRuleTagsCurrentPage);

                await Task.WhenAll(expenseTagsTask, expenseRuleTagsTask);

                var expenseTagsResult = await expenseTagsTask;
                var expenseRuleTagsResult = await expenseRuleTagsTask;

                ExpenseTags = expenseTagsResult.Data.ToList();
                ExpenseRuleTags = expenseRuleTagsResult.Data.ToList();
                HasMoreExpenseTags = expenseTagsResult.HasMoreData;
                HasMoreExpenseRuleTags = expenseRuleTagsResult.HasMoreData;

                UpdateExpenseTagsCollection();
                UpdateExpenseRuleTagsCollection();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error resetting search: {ex.Message}");
            }
            finally
            {
                IsLoadingED = false;
                IsLoadingRD = false;
            }
        }

        private void UpdateExpenseTagsCollection()
        {
            ExpenseTagsInfo.Clear();
            if (ExpenseTags is not null)
            {
                foreach (var eD in ExpenseTags)
                {
                    var container = new InfoContainer() { Key = eD.Title, Value = eD.Tags.First(), IsIgnored = eD.IsIgnored };
                    container.SetExpenseTagData(eD, _expenseTagService, _messageService, RemoveExpenseTagFromCollection);
                    ExpenseTagsInfo.Add(container);
                }
            }
        }

        private void UpdateExpenseRuleTagsCollection()
        {
            ExpenseRuleTagsInfo.Clear();
            if (ExpenseRuleTags is not null)
            {
                foreach (var rD in ExpenseRuleTags)
                {
                    var container = new InfoContainer() { Key = rD.Rule, Value = rD.Tags.First(), IsIgnored = rD.IsIgnored };
                    container.SetExpenseRuleTagData(rD, _expenseRuleTagService, _messageService, RemoveExpenseRuleTagFromCollection);
                    ExpenseRuleTagsInfo.Add(container);
                }
            }
        }

        private async Task InitializeDbSearchAsync() // Changed from async void
        {
            ExpenseTagsInfo.Clear();
            ExpenseRuleTagsInfo.Clear();

            // Reset pagination
            ExpenseTagsCurrentPage = 1;
            ExpenseRuleTagsCurrentPage = 1;

            try
            {
                var expenseTagsTask = _expenseTagService.GetExpenseTagByPage(ExpenseTagsCurrentPage);
                var expenseRuleTagsTask = _expenseRuleTagService.GetExpenseRuleTagByPage(ExpenseRuleTagsCurrentPage);
                var allTagsTask = GetAllTagsAsync();

                await Task.WhenAll(expenseTagsTask, expenseRuleTagsTask, allTagsTask);

                var expenseTagsResult = await expenseTagsTask;
                var expenseRuleTagsResult = await expenseRuleTagsTask;

                ExpenseTags = expenseTagsResult.Data.ToList();
                ExpenseRuleTags = expenseRuleTagsResult.Data.ToList();
                HasMoreExpenseTags = expenseTagsResult.HasMoreData;
                HasMoreExpenseRuleTags = expenseRuleTagsResult.HasMoreData;
                AllTags = await allTagsTask;

                LoadAvailableTags();
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

        private async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _tagService.GetAll();
        }

        private void LoadAvailableTags()
        {
            AvailableTags.Clear();
            foreach (var tag in AllTags)
            {
                AvailableTags.Add(tag.Name);
            }
        }

        [RelayCommand]
        private void OpenEditExpenseTagDialog(InfoContainer item)
        {
            CurrentEditingItem = item;
            CurrentEditType = EditType.ExpenseTag;
            EditItemName = item.Key;
            SelectedTag = item.Value;
            IsIgnored = item.IsIgnored;
            DialogTitle = "✏️ Edit Expense Tag";
            DialogSubtitle = "Modify the tag assignment for this expense";
            ItemFieldLabel = "Expense Description";
            IsEditDialogOpen = true;
        }

        [RelayCommand]
        private void OpenEditExpenseRuleTagDialog(InfoContainer item)
        {
            CurrentEditingItem = item;
            CurrentEditType = EditType.ExpenseRuleTag;
            EditItemName = item.Key;
            SelectedTag = item.Value;
            IsIgnored = item.IsIgnored;
            DialogTitle = "✏️ Edit Rule Tag";
            DialogSubtitle = "Modify the tag assignment for this rule";
            ItemFieldLabel = "Matching Rule";
            IsEditDialogOpen = true;
        }

        [RelayCommand]
        private void CloseEditDialog()
        {
            IsEditDialogOpen = false;
            CurrentEditingItem = null;
            EditItemName = string.Empty;
            SelectedTag = string.Empty;
            IsIgnored = false;
            CurrentEditType = EditType.ExpenseTag;
            DialogTitle = string.Empty;
            DialogSubtitle = string.Empty;
            ItemFieldLabel = string.Empty;
        }

        [RelayCommand]
        private async Task HandleEditSubmit()
        {
            // This method provides access to:
            // - EditItemName (expense description or rule)
            // - SelectedTag (selected tag from dropdown)
            // - CurrentEditType (ExpenseTag or ExpenseRuleTag)
            // - CurrentEditingItem (original InfoContainer)            
            try
            {
                if (CurrentEditType == EditType.ExpenseTag)
                {
                    await EditExpenseTag();
                }
                else
                {
                    await EditExpenseRuleTag();
                }
            }
            catch(Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error updating item: {ex.Message}");
            }
            finally
            {
                RefreshView();
                CloseEditDialog();
            }            
        }

        private async Task EditExpenseTag()
        {
            var newExpenseTag = new ExpenseTag
            {
                Title = EditItemName,
                Tags = [SelectedTag],
                IsIgnored = IsIgnored,
                CreatedAt = DateTime.UtcNow
            };
            await _expenseTagService.UpdateExpenseTag(newExpenseTag);
        }

        private async Task EditExpenseRuleTag()
        {
            var newExpenseRuleTag = new ExpenseRuleTag
            {
                Rule = EditItemName,
                Tags = [SelectedTag],
                IsIgnored = IsIgnored,
                CreatedAt = DateTime.UtcNow
            }; 
            await _expenseRuleTagService.UpdateExpenseRuleTag(newExpenseRuleTag);
        }   

        private void FinishEdDbSearch()
        {
            UpdateExpenseTagsCollection();
        }

        private void FinishRdDbSearch()
        {
            UpdateExpenseRuleTagsCollection();
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
        public required bool IsIgnored { get; set; }

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
