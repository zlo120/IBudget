using Avalonia.Controls;
using Avalonia.Interactivity;
using IBudget.GUI.ViewModels.DataView;

namespace IBudget.GUI.Views.DataView;

public partial class DataTableView : UserControl
{
    public DataTableView()
    {
        InitializeComponent();
        Loaded += DataTableView_Loaded;
    }

    private void DataTableView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DataTableViewModel viewModel)
        {
            viewModel.RefreshView();
        }
    }
}