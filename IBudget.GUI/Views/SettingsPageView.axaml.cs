using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using IBudget.GUI.ViewModels;

namespace IBudget.GUI.Views;

public partial class SettingsPageView : UserControl
{
    public SettingsPageView()
    {
        InitializeComponent();
    }

    private async void SelectFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        // Get the export directory path
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var stacksExportPath = Path.Combine(documentsPath, "Stacks", "Exports");
        
        // Ensure the directory exists
        if (!Directory.Exists(stacksExportPath))
        {
            Directory.CreateDirectory(stacksExportPath);
        }

        // Get the folder as IStorageFolder
        var startFolder = await topLevel.StorageProvider.TryGetFolderFromPathAsync(stacksExportPath);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            FileTypeFilter = new[]
            {
                new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            },
            Title = "Select Import File",
            AllowMultiple = false,
            SuggestedStartLocation = startFolder
        });

        if (files == null || files.Count == 0) return;

        var file = files.First();
        var filePath = file.Path.LocalPath;

        var viewModel = DataContext as SettingsPageViewModel;
        viewModel?.ImportFileCommand.Execute(filePath);
    }
}