using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.GUI.ViewModels.DataView;

namespace IBudget.GUI.ViewModels
{
    public partial class DataPageViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _view;

        private readonly MonthlyViewModel _monthlyViewModel;
        private readonly WeeklyViewModel _weeklyViewModel;
        public DataPageViewModel(
            MonthlyViewModel monthlyViewModel,
            WeeklyViewModel weeklyViewModel
        )
        {
            _monthlyViewModel = monthlyViewModel;
            _weeklyViewModel = weeklyViewModel;
            View = monthlyViewModel;
        }
    }
}
