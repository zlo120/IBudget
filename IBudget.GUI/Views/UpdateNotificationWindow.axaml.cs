using Avalonia.Controls;
using Avalonia.Interactivity;

namespace IBudget.GUI.Views
{
    public partial class UpdateNotificationWindow : Window
    {
        public UpdateNotificationWindow()
        {
            InitializeComponent();
        }

        private void RemindLater_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
