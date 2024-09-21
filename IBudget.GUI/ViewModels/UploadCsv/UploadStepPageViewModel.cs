using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class UploadStepPageViewModel : StepBase
    {
        private readonly StepViewModel _stepViewModel;
        public UploadStepPageViewModel(StepViewModel stepViewModel)
        {
            _stepViewModel = stepViewModel;
        }
        [ObservableProperty]
        private bool _canProceed = false;

        [ObservableProperty]
        private string _uploadMessage = "No file has been uploaded yet...";

        [RelayCommand]
        private void UploadFile()
        {
            var fileName = "testFile.jpg";
            UploadMessage = $"{fileName} has been uploaded...";
            CanProceed = true;
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
            _stepViewModel.StepOver();
            OnSteppingOver();
        }
    }
}
