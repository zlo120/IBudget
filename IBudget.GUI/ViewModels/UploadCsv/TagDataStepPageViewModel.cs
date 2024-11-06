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
        private readonly CompleteStepPageViewModel _completeStepPageViewModel;
        private readonly ITagService _tagsService;
        private static int USER_ID = -1;
        public Uri FileUriFromService
        {
            get { return _csvService.FileUri; }
        }

        public TagDataStepPageViewModel(
            StepViewModel stepViewModel,
            CsvService csvService,
            ICSVParserService csvParserService,
            IUserDictionaryService userDictionaryService,
            CompleteStepPageViewModel completeStepPageViewModel,
            ITagService tagsService
        )
        {
            _csvService = csvService;
            _csvParserService = csvParserService;
            _userDictionaryService = userDictionaryService;
            _stepViewModel = stepViewModel;
            _completeStepPageViewModel = completeStepPageViewModel;
            _tagsService = tagsService;

            GetAllTags();
        }

        private void GetAllTags()
        {
            var allTags = _tagsService.GetAll().Result;
            foreach (var tag in allTags)
            {
                if (!ExistingTags.Contains(tag.Name))
                    ExistingTags.Add(tag.Name);
            }
        }

        [ObservableProperty]
        private string? _fileUri = null;

        partial void OnSelectedUntaggedItemChanged(TagListItemTemplate? value)
        {
            if (value is null) return;
            //if (IsCreatingRule) IsCreatingRule = false;
            SelectedUntaggedItemName = value.Label;
        }
        // Untagged side
        [ObservableProperty]
        private TagListItemTemplate? _selectedUntaggedItem;

        public ObservableCollection<TagListItemTemplate> UntaggedItems { get; } = new();

        public ObservableCollection<string> ExistingTags { get; } = new();

        [ObservableProperty]
        private string _selectedUntaggedItemName;

        [ObservableProperty]
        private bool _isLoading = false;
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
            if (Tag == string.Empty) return;
            if (IsCreatingRule) HandleCreateRule(Rule, Tag.ToLower());
            else HandleCreateEntry(SelectedUntaggedItemName, Tag.ToLower());
            _tagsService.CreateTag(new Tag() { Name = Tag.ToLower() });
            if (UntaggedItems.Count == 0)
            {
                ClearAllProperties();
                _stepViewModel.StepOver();
                _completeStepPageViewModel.ProcessCsv();
                OnSteppingOver();
                return;
            }
            Tag = string.Empty;
            SelectedUntaggedItem = UntaggedItems[0];
            SelectedUntaggedItemName = SelectedUntaggedItem.Label;
            RemainingUntaggedRecords = $"{UntaggedItems.Count} entries left";

            GetAllTags();
        }
        [RelayCommand]
        private void Reset()
        {
            ClearAllProperties();
            _stepViewModel.StepBack();
            _csvService.FileUri = null;
            OnSteppingBack();
        }
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
            SelectedUntaggedItem = UntaggedItems[0];
            SelectedUntaggedItemName = SelectedUntaggedItem.Label;
            if (distinctUntaggedRecords.Count == 0)
            {
                ClearAllProperties();
                _stepViewModel.StepOver();
                _completeStepPageViewModel.ProcessCsv();
                OnSteppingOver();
                return;
            }
            IsLoading = false;
            RemainingUntaggedRecords = $"{distinctUntaggedRecords.Count} entries left";
        }
        private void HandleCreateRule(string rule, string tag)
        {
            var removeFromCollection = UntaggedItems.Where(item => item.Label.Contains(rule, StringComparison.InvariantCultureIgnoreCase)).ToList();
            foreach (var removeItem in removeFromCollection)
            {
                UntaggedItems.Remove(removeItem);
            }
            var ruleDictionary = new RuleDictionary()
            {
                rule = rule,
                tags = [tag]
            };
            _userDictionaryService.AddRuleDictionary(USER_ID, ruleDictionary);
            Rule = string.Empty;
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
