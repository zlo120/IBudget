using Avalonia.Controls;
using Avalonia.Interactivity;
using IBudget.GUI.ViewModels;

namespace IBudget.GUI.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        Loaded += DashboardView_Loaded;
    }

    private void DashboardView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DashboardViewModel viewModel)
        {
            viewModel.RefreshView();
        }
    }
}