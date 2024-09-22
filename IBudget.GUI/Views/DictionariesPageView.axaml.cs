using Avalonia.Controls;
using Avalonia.Interactivity;
using IBudget.GUI.ViewModels;
using System;

namespace IBudget.GUI.Views;

public partial class DictionariesPageView : UserControl
{
    public DictionariesPageView()
    {
        InitializeComponent();
        Loaded += DictionariesPage_Loaded;
    }

    private void DictionariesPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DictionariesPageViewModel viewModel)
        {
            viewModel.RefreshView();
        }
    }
}