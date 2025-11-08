using Avalonia.Controls;
using IBudget.GUI.ViewModels;

namespace IBudget.GUI.Views;

public partial class ConfigurationWindow : Window
{
    public ConfigurationWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is ConfigurationViewModel viewModel)
        {
            viewModel.CloseRequested += (s, args) => Close();
        }
    }
}