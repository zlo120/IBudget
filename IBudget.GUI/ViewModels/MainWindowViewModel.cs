using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.GUI.Utils;
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

        public MainWindowViewModel(
            HomePageViewModel homePageViewModel,
            UploadCsvPageViewModel uploadCsvPageViewModel,
            DataPageViewModel dataPageViewModel,
            DictionariesPageViewModel dictionariesPageViewModel,
            TagsPageViewModel tagsPageViewModel
        )
        {
            _homePageViewModel = homePageViewModel;
            _uploadCsvPageViewModel = uploadCsvPageViewModel;
            _dataPageViewModel = dataPageViewModel;
            _dictionariesPageViewModel = dictionariesPageViewModel;
            _tagsPageViewModel = tagsPageViewModel;

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

            if (instance is null) return;
            CurrentPage = instance;
        }

        public ObservableCollection<ListItemTemplate> Items { get; } = new()
        {
            new ListItemTemplate(typeof(HomePageViewModel), "HomeRegular"),
            new ListItemTemplate(typeof(UploadCsvPageViewModel), "DocumentRegular"),
            new ListItemTemplate(typeof(DataPageViewModel), "DataRegular"),
            new ListItemTemplate(typeof(DictionariesPageViewModel), "BookDbRegular"),
            new ListItemTemplate(typeof(TagsPageViewModel), "TagRegular"),
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
