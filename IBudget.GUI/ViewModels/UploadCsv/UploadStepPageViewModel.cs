using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.GUI.Services.Impl;
using System;
using System.IO;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class UploadStepPageViewModel : StepBase
    {
        private readonly CsvService _csvService;
        private readonly StepViewModel _stepViewModel;
        private readonly TagDataStepPageViewModel _tagDataStepPageViewModel;

        public UploadStepPageViewModel(
            StepViewModel stepViewModel, 
            CsvService csvService,
            TagDataStepPageViewModel tagDataStepPageViewModel
        )
        {
            _csvService = csvService;
            _stepViewModel = stepViewModel;
            _tagDataStepPageViewModel = tagDataStepPageViewModel;
            
        }
        [ObservableProperty]
        private bool _canProceed = false;

        [ObservableProperty]
        private string _uploadMessage = "No file has been uploaded yet...";

        [ObservableProperty]
        private string _errorMessage = "Only csv files are supported";

        [ObservableProperty]
        private bool _isError = false;

        [ObservableProperty]
        private StreamGeometry _fileIcon = (StreamGeometry) Application.Current!.FindResource("FolderAddRegular")!;

        [ObservableProperty]
        private StreamGeometry _dismissIcon = (StreamGeometry) Application.Current!.FindResource("DismissRegular")!;

        [RelayCommand]
        private void ToggleError()
        {
            IsError = !IsError;
        }

        [RelayCommand]
        private void Clear()
        {
            UploadMessage = "No file has been uploaded yet...";
            CanProceed = false;
        }
        [RelayCommand]
        private void ConfirmFile()
        {
            _csvService.FileUri = DroppedFilePath;
            _tagDataStepPageViewModel.UpdateFileUri();
            _stepViewModel.StepOver();
            OnSteppingOver();
        }
        [ObservableProperty]
        private Uri _droppedFilePath;

        [RelayCommand]
        private void FileProvided(Uri filePath)
        {
            string fileExtension = Path.GetExtension(filePath.LocalPath);

            if (!fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                ToggleError();
                return;
            }

            DroppedFilePath = filePath;
            UploadMessage = $"{Path.GetFileName(DroppedFilePath.LocalPath).Replace("%20", " ")} has been uploaded...";
            CanProceed = true;
        }
    }
}
