using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.GUI.ViewModels.UploadCsv;

namespace IBudget.GUI.ViewModels
{
    public partial class UploadCsvPageViewModel : ViewModelBase
    {
        public UploadCsvPageViewModel(
            StepContainerViewModel stepContainerViewModel,
            StepViewModel stepViewModel
        )
        {
            CurrentPage = stepContainerViewModel;
            StepView = stepViewModel;
        }

        [ObservableProperty]
        private ViewModelBase? _currentPage = null;

        [ObservableProperty]
        private ViewModelBase? _stepView = null;
    }
}
