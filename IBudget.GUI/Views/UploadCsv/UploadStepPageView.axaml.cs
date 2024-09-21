using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using IBudget.GUI.ViewModels.UploadCsv;
using System.Linq;

namespace IBudget.GUI.Views.UploadCsv;

public partial class UploadStepPageView : UserControl
{
    public UploadStepPageView()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, DropHandler!);
    }
    private void DropHandler(object sender, DragEventArgs e)
    {
        var filePath = e.Data.GetFiles()?.First().Path;
        if (filePath != null)
        {
            var viewModel = DataContext as UploadStepPageViewModel;
            viewModel?.FileProvidedCommand.Execute(filePath);
        }
    }

    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            FileTypeFilter = new[]
            {
                new FilePickerFileType("CSV Files") { Patterns = new[] { "*.csv"} }
            },
            Title = "Open Csv File",
            AllowMultiple = false
        });
        if (files is null || files.Count == 0) return;
        var file = files.First();
        var filePath = new System.Uri(file.Path.AbsolutePath);
        var viewModel = DataContext as UploadStepPageViewModel;
        viewModel?.FileProvidedCommand.Execute(filePath);
    }
}