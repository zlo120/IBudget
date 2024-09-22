using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.GUI.Services.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class TagDataStepPageViewModel : StepBase
    {
        private readonly CsvService _csvService;
        private readonly ICSVParserService _csvParserService;
        private readonly IUserDictionaryService _userDictionaryService;
        private readonly StepViewModel _stepViewModel;

        public Uri FileUriFromService
        {
            get { return _csvService.FileUri; }
        }

        public TagDataStepPageViewModel(
            StepViewModel stepViewModel,
            CsvService csvService, 
            ICSVParserService csvParserService, 
            IUserDictionaryService userDictionaryService
        )
        {
            _csvService = csvService;
            _csvParserService = csvParserService;
            _userDictionaryService = userDictionaryService;
            _stepViewModel = stepViewModel;
        }

        [ObservableProperty]
        private string? _fileUri = null;
        public async void UpdateFileUri()
        {
            IsLoading = true;
            FileUri = FileUriFromService.ToString();
            var formatted = await Task.Run(() => _csvParserService.ParseCSV(FileUriFromService.LocalPath.Replace("%20", " ")));
            var untagged = await Task.Run(() => _csvParserService.FindUntagged(formatted));
            var distinctUntaggedRecords = new HashSet<string>();
            foreach (var untaggedRecord in untagged)
            {
                if (!distinctUntaggedRecords.Contains(untaggedRecord.Description))
                {
                    UntaggedItems.Add(new TagListItemTemplate(untaggedRecord.Description));
                    distinctUntaggedRecords.Add(untaggedRecord.Description);
                }
            }
            if (distinctUntaggedRecords.Count == 0)
            {
                ClearAllProperties();
                _stepViewModel.StepOver();
                OnSteppingOver();
                return;
            }
            IsLoading = false;
            RemainingUntaggedRecords = $"{distinctUntaggedRecords.Count} entries left";
        }

        // Untagged side
        [ObservableProperty]
        private TagListItemTemplate? _selectedUntaggedItem;

        [ObservableProperty]
        private string _selectedUntaggedItemName = "Unselected";

        partial void OnSelectedUntaggedItemChanged(TagListItemTemplate? value)
        {
            if (IsCreatingRule) IsCreatingRule = false;
            if (value is null) return;
            SelectedUntaggedItemName = value.Label;
        }

        [ObservableProperty]
        private bool _isLoading = false;
        public ObservableCollection<TagListItemTemplate> UntaggedItems { get; } = new();

        // Tagging side
        [ObservableProperty]
        private bool _isCreatingRule = false;

        [ObservableProperty]
        private string _remainingUntaggedRecords = string.Empty;

        [ObservableProperty]
        private string _rule = string.Empty;

        [ObservableProperty]
        private string _tag = string.Empty;

        [RelayCommand]
        private void SubmitTagging()
        {
            if (IsCreatingRule) HandleCreateRule(Rule, Tag);
            else HandleCreateEntry(SelectedUntaggedItemName, Tag);
            if (UntaggedItems.Count == 0)
            {
                ClearAllProperties();
                _stepViewModel.StepOver();
                OnSteppingOver();
                return;
            }
            RemainingUntaggedRecords = $"{UntaggedItems.Count} entries left";
        }

        [RelayCommand]
        private void Reset()
        {
            ClearAllProperties();
            _stepViewModel.StepBack();
            _csvService.FileUri = null;
            OnSteppingBack();
        }

        private static int USER_ID = -1;
        private void HandleCreateRule(string rule, string tag)
        {
            var removeFromCollection = UntaggedItems.Where(item => item.Label.Contains(rule, StringComparison.InvariantCultureIgnoreCase)).ToList();
            foreach(var removeItem in removeFromCollection)
            {
                UntaggedItems.Remove(removeItem);
            }
            var ruleDictionary = new RuleDictionary()
            {
                rule = rule,
                tags = [tag]
            };
            _userDictionaryService.AddRuleDictionary(USER_ID, ruleDictionary);
        }

        private void HandleCreateEntry(string entryName, string tag)
        {
            if (SelectedUntaggedItemName == "Unselected") return;
            var entryFromCollection = UntaggedItems.Where(item => item.Label == entryName).First();
            UntaggedItems.Remove(entryFromCollection);
            var entry = new ExpenseDictionary()
            {
                title = entryName,
                tags = [tag]
            };
            _userDictionaryService.AddExpenseDictionary(USER_ID, entry);
        }

        private void ClearAllProperties()
        {
            FileUri = null;
            UntaggedItems.Clear();
            IsCreatingRule = false;
            RemainingUntaggedRecords = string.Empty;
            SelectedUntaggedItemName = "Unselected";
            Tag = string.Empty;
            Rule = string.Empty;
            IsLoading = false;
        }
    }

    public class TagListItemTemplate
    {
        public TagListItemTemplate(string label)
        {
            Label = label;
        }
        public string Label { get; }
    }
}
