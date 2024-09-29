using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.GUI.ViewModels.DataView;

namespace IBudget.GUI.ViewModels
{
    public partial class DataPageViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _view;
        [ObservableProperty]
        private int _selectedIndex = 0;

        private readonly YearlyViewModel _yearlyViewModel;
        private readonly MonthlyViewModel _monthlyViewModel;
        private readonly WeeklyViewModel _weeklyViewModel;
        public DataPageViewModel(
            YearlyViewModel yearlyViewModel,
            MonthlyViewModel monthlyViewModel,
            WeeklyViewModel weeklyViewModel
        )
        {
            _yearlyViewModel = yearlyViewModel;
            _monthlyViewModel = monthlyViewModel;
            _weeklyViewModel = weeklyViewModel;
            View = yearlyViewModel;
        }
        partial void OnSelectedIndexChanged(int value)
        {
            switch (value)
            {
                case 0:
                    View = _yearlyViewModel;
                    break;

                case 1:
                    View = _monthlyViewModel;
                    break;

                case 2:
                    View = _weeklyViewModel;
                    break;
            }
        }
    }
}
