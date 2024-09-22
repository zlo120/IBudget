using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace IBudget.GUI.ViewModels.UploadCsv
{
    public partial class StepContainerViewModel : ViewModelBase
    {
        private readonly List<ViewModelBase> _views;
        private int _currentViewIndex;
        private readonly StepViewModel _stepViewModel;
        public ViewModelBase GetCurrentView
        {
            get { return _views[_currentViewIndex]; }
        }

        public StepContainerViewModel(
            UploadStepPageViewModel uploadStepPageViewModel,
            TagDataStepPageViewModel tagDataStepPageViewModel,
            CompleteStepPageViewModel completeStepPageViewModel,
            StepViewModel stepViewModel
        )
        {
            _views = new List<ViewModelBase>() {uploadStepPageViewModel, tagDataStepPageViewModel, completeStepPageViewModel};
            foreach(StepBase step in _views)
            {
                step.RequestStepOver += OnStepOverRequest!;
                step.RequestStepBack += OnStepBackRequest!;
            }
            _currentViewIndex = 0;
            _stepViewModel = stepViewModel;
            CurrentView = GetCurrentView;
        }

        [ObservableProperty]
        private ViewModelBase? _currentView = null;

        private void StepThroughView()
        {
            _currentViewIndex = (_currentViewIndex + 1) % 3;
            CurrentView = GetCurrentView;
        }
        private void StepBackAView()
        {
            _currentViewIndex = (_currentViewIndex - 1) % 3;
            CurrentView = GetCurrentView;
        }

        private void OnStepOverRequest(object sender, EventArgs e)
        {
            StepThroughView();
        }
        private void OnStepBackRequest(object sender, EventArgs e)
        {
            StepBackAView();
        }
    }
}
