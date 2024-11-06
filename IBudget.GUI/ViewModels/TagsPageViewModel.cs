using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IBudget.GUI.ViewModels
{
    public partial class TagsPageViewModel : ViewModelBase
    {
        private readonly ITagService _tagService;
        public ObservableCollection<AllTagsListItemTemplate> Tags { get; } = new();

        public TagsPageViewModel(ITagService tagService)
        {
            _tagService = tagService;
            var tags = _tagService.GetAll().Result;
            foreach (var tag in tags)
            {
                Tags.Add(new AllTagsListItemTemplate(tag.Name, tag.IsTracked, _tagService));
            }
        }

        public void RefreshView()
        {
            var tags = _tagService.GetAll().Result;
            foreach (var tag in tags)
            {
                var tagTemplate = new AllTagsListItemTemplate(tag.Name, tag.IsTracked, _tagService);
                if (!Tags.Contains(tagTemplate)) Tags.Add(tagTemplate);
            }
        }
    }

    public partial class AllTagsListItemTemplate : ViewModelBase
    {
        private readonly ITagService _tagService;
        private const string IS_TRACKED_PREFIX = "⭐ ";
        public AllTagsListItemTemplate(string label, bool isTracked, ITagService tagService)
        {
            TagName = label;
            IsTracked = isTracked;
            _tagService = tagService;
            if (IsTracked) Label = IS_TRACKED_PREFIX + label;
            else Label = label;
        }
        [ObservableProperty]
        private string _label;
        [ObservableProperty]
        private bool _isTracked;
        [ObservableProperty]
        private string _tagName;

        [RelayCommand]
        private async void UpdateIsTracked()
        {
            var updatedTag = await _tagService.GetTag(TagName);
            updatedTag.IsTracked = IsTracked;
            await _tagService.UpdateTag(updatedTag);

            if (IsTracked) Label = IS_TRACKED_PREFIX + Label;
            else Label = TagName;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not AllTagsListItemTemplate) return false;
            var other = (AllTagsListItemTemplate)obj;
            if (other.TagName != TagName) return false;
            return true;
        }
    }
}