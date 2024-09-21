using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.GUI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IBudget.GUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly List<ViewModelBase> _viewModels;
        public MainWindowViewModel(
            HomePageViewModel homePageViewModel,
            UploadCsvPageViewModel uploadCsvPageViewModel,
            ThisMonthPageViewModel thisMonthPageViewModel,
            TagsPageViewModel tagsPageViewModel,
            DictionariesPageViewModel dictionariesPageViewModel
        )
        {
            _viewModels = new List<ViewModelBase>() 
            { 
                homePageViewModel, 
                uploadCsvPageViewModel, 
                thisMonthPageViewModel, 
                tagsPageViewModel,
                dictionariesPageViewModel
            };
        }

        [ObservableProperty]
        private bool _isPaneOpen = true;

        [ObservableProperty]
        private ViewModelBase _currentPage = new HomePageViewModel();

        [ObservableProperty]
        private ListItemTemplate? _selectedListItem;

        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;
            var instance = _viewModels.ResolveViewModel(value.ModelType);
            CurrentPage = instance;
        }

        public ObservableCollection<ListItemTemplate> Items { get; } = new()
        {
            new ListItemTemplate(typeof(HomePageViewModel), "HomeRegular"),
            new ListItemTemplate(typeof(UploadCsvPageViewModel), "DocumentRegular"),
            new ListItemTemplate(typeof(ThisMonthPageViewModel), "CalendarStar"),
            new ListItemTemplate(typeof(TagsPageViewModel), "TagRegular"),
            new ListItemTemplate(typeof(DictionariesPageViewModel), "BookDbRegular"),
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
            Label = LabelUtils.AddSpacesBeforeCapitals(label);
            Application.Current!.TryFindResource(iconKey, out var res);
            ListItemIcon = (StreamGeometry)res!;
        }
        public string Label { get; }
        public Type ModelType { get; }
        public StreamGeometry ListItemIcon { get; }
    }
}
