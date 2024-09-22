using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class StepViewModel : ViewModelBase
    {
        private static readonly IBrush _activeColour = Brushes.LimeGreen;
        private static readonly IBrush _disabledColour = Brushes.LightSkyBlue;

        [ObservableProperty]
        private IBrush _firstStepColour = _activeColour;

        [ObservableProperty]
        private IBrush _secondStepColour = _disabledColour;

        [ObservableProperty]
        private IBrush _thirdStepColour = _disabledColour;

        [ObservableProperty]
        private int _step = 0;

        [ObservableProperty]
        private ViewModelBase? _currentPage = null;
        public int StepOver()
        {
            Step = (Step + 1) % 3;
            return Step;
        }
        public int StepBack()
        {
            Step = (Step - 1) % 3;
            return Step;
        }
        partial void OnStepChanged(int value)
        {
            UpdateState();
        }
        private void UpdateState()
        {
            switch (Step)
            {
                case 0:
                    FirstStepColour = _activeColour;
                    SecondStepColour = _disabledColour;
                    ThirdStepColour = _disabledColour;
                    break;

                case 1:
                    FirstStepColour = _activeColour;
                    SecondStepColour = _activeColour;
                    ThirdStepColour = _disabledColour;
                    break;

                case 2:
                    FirstStepColour = _activeColour;
                    SecondStepColour = _activeColour;
                    ThirdStepColour = _activeColour;
                    break;
            }
        }
    }
}
