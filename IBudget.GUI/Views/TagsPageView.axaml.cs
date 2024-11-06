using Avalonia.Controls;
using Avalonia.Interactivity;
using IBudget.GUI.ViewModels;

namespace IBudget.GUI.Views;

public partial class TagsPageView : UserControl
{
    public TagsPageView()
    {
        InitializeComponent();
        Loaded += TagsPage_Loaded;
    }

    private void TagsPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is TagsPageViewModel viewModel)
        {
            viewModel.RefreshView();
        }
    }
}