using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public ObservableCollection<string> AvailableTags { get; } = new();

        private readonly IExpenseTagService _expenseTagService;
        private readonly IExpenseRuleTagService _expenseRuleTagService;
        private readonly IMessageService _messageService;
        private readonly ITagService _tagService;

        public DictionariesPageViewModel(IExpenseTagService expenseTagService, IExpenseRuleTagService expenseRuleTagService, IMessageService messageService, ITagService tagService)
        {
            _expenseTagService = expenseTagService;
            _expenseRuleTagService = expenseRuleTagService;
            _messageService = messageService;
            _tagService = tagService;
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
                var allTagsTask = GetAllTagsAsync();

                await Task.WhenAll(expenseTagsTask, expenseRuleTagsTask, allTagsTask);

                ExpenseTags = await expenseTagsTask;
                ExpenseRuleTags = await expenseRuleTagsTask;
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

        private async Task<List<ExpenseTag>> GetExpenseTagsAsync()
        {
            return await _expenseTagService.GetAllExpenseTags();
        }

        private async Task<List<ExpenseRuleTag>> GetExpenseRuleTagsAsync()
        {
            return await _expenseRuleTagService.GetAllExpenseRuleTags();
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
                CreatedAt = DateTime.UtcNow
            }; 
            await _expenseRuleTagService.UpdateExpenseRuleTag(newExpenseRuleTag);
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
