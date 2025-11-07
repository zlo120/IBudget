using Avalonia.Controls;

namespace IBudget.GUI.Views
{
    public partial class PatchNotesWindow : Window
    {
        public PatchNotesWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
