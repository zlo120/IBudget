using Avalonia.Controls;
using Avalonia.Interactivity;
using IBudget.GUI.ViewModels.DataView;

namespace IBudget.GUI.Views.DataView;

public partial class MonthlyView : UserControl
{
    public MonthlyView()
    {
        InitializeComponent();
        Loaded += MonthlyView_Loaded;
    }

    private void MonthlyView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MonthlyViewModel viewModel)
        {
            viewModel.RefreshView();
        }
    }
}