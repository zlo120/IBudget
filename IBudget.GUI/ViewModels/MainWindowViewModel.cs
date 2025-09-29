using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.GUI.Utils;
using IBudget.GUI.ViewModels.DataView;
using System;
using System.Collections.ObjectModel;

namespace IBudget.GUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly HomePageViewModel _homePageViewModel;
        private readonly UploadCsvPageViewModel _uploadCsvPageViewModel;
        private readonly DataPageViewModel _dataPageViewModel;
        private readonly DictionariesPageViewModel _dictionariesPageViewModel;
        private readonly TagsPageViewModel _tagsPageViewModel;
        private readonly FinancialGoalsPageViewModel _financialGoalsPageViewModel;
        private readonly DataTableViewModel _dataTableViewModel;
        private readonly SettingsPageViewModel _settingsPageViewModel;

        public MainWindowViewModel(
            HomePageViewModel homePageViewModel,
            UploadCsvPageViewModel uploadCsvPageViewModel,
            DataPageViewModel dataPageViewModel,
            DictionariesPageViewModel dictionariesPageViewModel,
            TagsPageViewModel tagsPageViewModel,
            FinancialGoalsPageViewModel financialGoalsPageViewModel,
            DataTableViewModel dataTableViewModel,
            SettingsPageViewModel settingsPageViewModel
        )
        {
            _homePageViewModel = homePageViewModel;
            _uploadCsvPageViewModel = uploadCsvPageViewModel;
            _dataPageViewModel = dataPageViewModel;
            _dictionariesPageViewModel = dictionariesPageViewModel;
            _tagsPageViewModel = tagsPageViewModel;
            _financialGoalsPageViewModel = financialGoalsPageViewModel;
            _dataTableViewModel = dataTableViewModel;
            _settingsPageViewModel = settingsPageViewModel;

            CurrentPage = _homePageViewModel;
        }

        [ObservableProperty]
        private bool _isPaneOpen = true;

        [ObservableProperty]
        private ViewModelBase _currentPage = null;

        [ObservableProperty]
        private ListItemTemplate? _selectedListItem;
        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;

            ViewModelBase? instance = null;
            if (value.ModelType == typeof(HomePageViewModel))
                instance = _homePageViewModel;
            if (value.ModelType == typeof(UploadCsvPageViewModel))
                instance = _uploadCsvPageViewModel;
            if (value.ModelType == typeof(DataPageViewModel))
                instance = _dataPageViewModel;
            if (value.ModelType == typeof(DictionariesPageViewModel))
                instance = _dictionariesPageViewModel;
            if (value.ModelType == typeof(TagsPageViewModel))
                instance = _tagsPageViewModel;
            if (value.ModelType == typeof(FinancialGoalsPageViewModel))
                instance = _financialGoalsPageViewModel;
            if (value.ModelType == typeof(DataTableViewModel))
                instance = _dataTableViewModel;
            if (value.ModelType == typeof(SettingsPageViewModel))
                instance = _settingsPageViewModel;

            if (instance is null) return;
            CurrentPage = instance;
        }

        public ObservableCollection<ListItemTemplate> Items { get; } = new()
        {
            new ListItemTemplate(typeof(HomePageViewModel), "HomeRegular"),
            new ListItemTemplate(typeof(UploadCsvPageViewModel), "DocumentRegular"),
            new ListItemTemplate(typeof(DataPageViewModel), "DataRegular"),
            new ListItemTemplate(typeof(DataTableViewModel), "FolderRegular"),
            new ListItemTemplate(typeof(DictionariesPageViewModel), "BookDbRegular"),
            new ListItemTemplate(typeof(TagsPageViewModel), "TagRegular"),
            new ListItemTemplate(typeof(FinancialGoalsPageViewModel), "MoneyRegular"),
            new ListItemTemplate(typeof(SettingsPageViewModel), "SettingsRegular"),
        };

        [RelayCommand]
        private void TogglePane()
        {
            IsPaneOpen = !IsPaneOpen;
        }
    }
    public class ListItemTemplate
    {
        public ListItemTemplate(Type type, string iconKey)
        {
            var label = type.Name.Replace("PageViewModel", "");
            ModelType = type;
            if (type == typeof(DataPageViewModel))
            {
                Label = "Financial Overview";
            }
            else if (type == typeof(DictionariesPageViewModel))
            {
                Label = "Tag Dictionary";
            }
            else if (type == typeof(DataTableViewModel))
            {
                Label = "Financial Records";
            }
            else if (type == typeof(SettingsPageViewModel))
            {
                Label = "Settings";
            }
            else
            {
                Label = LabelUtils.AddSpacesBeforeCapitals(label);
            }
                Application.Current!.TryFindResource(iconKey, out var res);
            ListItemIcon = (StreamGeometry)res!;
        }
        public string Label { get; }
        public Type ModelType { get; }
        public StreamGeometry ListItemIcon { get; }
    }
}
