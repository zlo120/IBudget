using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.Core.Interfaces;
using IBudget.GUI.Services;
using MongoDB.Bson;

namespace IBudget.GUI.ViewModels
{
    public partial class TagsPageViewModel : ViewModelBase
    {
        private readonly ITagService _tagService;
        private readonly IMessageService _messageService;
        public ObservableCollection<AllTagsListItemTemplate> Tags { get; } = new();

        [ObservableProperty]
        private string _tagName = string.Empty;
        [ObservableProperty]
        private string _message = string.Empty;
        [ObservableProperty]
        private bool _isTracked = false;

        public TagsPageViewModel(ITagService tagService, IMessageService messageService)
        {
            _tagService = tagService;
            _messageService = messageService;
            _ = InitializeTagsAsync();
        }

        private async Task InitializeTagsAsync()
        {
            try
            {
                var tags = await _tagService.GetAll();
                foreach (var tag in tags)
                {
                    Tags.Add(new AllTagsListItemTemplate(tag.Name, tag.IsTracked, _tagService, _messageService, (ObjectId)tag.Id!, RemoveTagFromCollection));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tags: {ex.Message}");
            }
        }

        [RelayCommand]
        private async void CreateTag()
        {
            if (TagName == string.Empty) return;
            var tagName = TagName.ToLower();
            
            // Check if tag already exists by name (before creating the template)
            var existingTags = await _tagService.GetAll();
            if (existingTags.Any(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
            {
                Message = $"\"{TagName}\" already exists";
                TagName = string.Empty;
                IsTracked = false;
                return;
            }
            
            var tag = new Core.Model.Tag() { Name = tagName, IsTracked = IsTracked, CreatedAt = DateTime.Now };
            
            try
            {
                // Save the tag to the database first
                await _tagService.CreateTag(tag);
                
                // Get the saved tag with the actual ID
                var savedTag = await _tagService.GetTagByName(tagName);
                if (savedTag?.Id != null)
                {
                    // Now create the template with the real ID
                    var tagTemplate = new AllTagsListItemTemplate(
                        savedTag.Name, 
                        savedTag.IsTracked, 
                        _tagService, 
                        _messageService, 
                        (ObjectId)savedTag.Id, 
                        RemoveTagFromCollection);
                    
                    // Add to the UI collection
                    Tags.Add(tagTemplate);
                }
                
                Message = $"\"{TagName}\" created successfully!";
                TagName = string.Empty;
                IsTracked = false;
            }
            catch (Exception ex)
            {
                Message = $"Error creating tag: {ex.Message}";
                Debug.WriteLine($"Error creating tag: {ex.Message}");
            }
        }

        public async Task RefreshViewAsync()
        {
            try
            {
                var tags = await _tagService.GetAll();
                foreach (var tag in tags)
                {
                    var existing = Tags.FirstOrDefault(t => t.TagName == tag.Name);
                    if (existing != null)
                    {
                        // Update properties if changed
                        if (existing.IsTracked != tag.IsTracked)
                        {
                            existing.IsTracked = tag.IsTracked;
                            existing.Label = tag.IsTracked
                                ? "⭐ " + tag.Name
                                : tag.Name;
                        }
                    }
                    else
                    {
                        Tags.Add(new AllTagsListItemTemplate(tag.Name, tag.IsTracked, _tagService, _messageService, (ObjectId)tag.Id!, RemoveTagFromCollection));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing tags: {ex.Message}");
            }
        }

        private void RemoveTagFromCollection(AllTagsListItemTemplate item)
        {
            Tags.Remove(item);
        }

        // Keep for backward compatibility
        public void RefreshView()
        {
            _ = RefreshViewAsync();
        }
    }

    public partial class AllTagsListItemTemplate : ViewModelBase
    {
        private readonly ITagService _tagService;
        private readonly IMessageService _messageService;
        private readonly Action<AllTagsListItemTemplate> _removeFromCollection;
        private const string IS_TRACKED_PREFIX = "⭐ ";
        private readonly ObjectId _id;
        
        public AllTagsListItemTemplate(string label, bool isTracked, ITagService tagService, IMessageService messageService, ObjectId id, Action<AllTagsListItemTemplate> removeFromCollection)
        {
            TagName = label;
            IsTracked = isTracked;
            _tagService = tagService;
            _messageService = messageService;
            _removeFromCollection = removeFromCollection;
            if (IsTracked) Label = IS_TRACKED_PREFIX + label;
            else Label = label;
            _id = id;
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
            if (TagName == "ignored")  // special case: you can't track ignored tag
            {
                IsTracked = false;
                return;
            }
            var updatedTag = await _tagService.GetTagByName(TagName);
            updatedTag.IsTracked = IsTracked;
            await _tagService.UpdateTag(updatedTag);

            if (IsTracked) Label = IS_TRACKED_PREFIX + Label;
            else Label = TagName;
        }

        [RelayCommand]
        private async Task DeleteClick()
        {
            var confirmedDelete = await _messageService.ShowConfirmationAsync(
                "Delete Tag", 
                $"Are you sure you want to delete tag '{TagName}'?");
            
            if (!confirmedDelete)
            {
                return;
            }

            try
            {
                var tagToDelete = await _tagService.GetTagByName(TagName);
                if (tagToDelete == null)
                {
                    await _messageService.ShowErrorAsync($"Tag '{TagName}' not found.");
                    return;
                }
                
                await _tagService.DeleteTagById(_id);
                
                _removeFromCollection(this);
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync($"Error deleting tag '{TagName}': {ex.Message}");
            }
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