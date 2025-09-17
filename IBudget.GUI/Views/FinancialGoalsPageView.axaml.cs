using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;

namespace IBudget.GUI.Views
{
    public partial class FinancialGoalsPageView : UserControl
    {
        private static readonly Regex DecimalRegex = new Regex(@"^[0-9]*\.?[0-9]{0,1}$");

        public FinancialGoalsPageView()
        {
            InitializeComponent();
        }

        private void OnAmountTextInput(object? sender, TextInputEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            // Check if input contains only allowed characters
            if (string.IsNullOrEmpty(e.Text)) return;

            // Only allow digits and decimal point
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true;
                    return;
                }
            }

            // Get the text that would result after this input
            var currentText = textBox.Text ?? string.Empty;
            var selectionStart = textBox.SelectionStart;
            var selectionEnd = textBox.SelectionEnd;

            // Remove selected text
            var newText = currentText.Remove(selectionStart, selectionEnd - selectionStart);

            // Insert new text
            newText = newText.Insert(selectionStart, e.Text);

            // Check if the resulting text is valid
            if (!IsValidDecimalInput(newText))
            {
                e.Handled = true; // Prevent the input
            }
        }

        private void OnAmountKeyDown(object? sender, KeyEventArgs e)
        {
            // Allow control keys
            if (e.Key == Key.Back || e.Key == Key.Delete ||
                e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Tab || e.Key == Key.Enter ||
                e.Key == Key.Home || e.Key == Key.End ||
                (e.KeyModifiers.HasFlag(KeyModifiers.Control) && (e.Key == Key.A || e.Key == Key.C || e.Key == Key.V || e.Key == Key.X || e.Key == Key.Z)))
            {
                return;
            }

            // Allow numeric keys (both main keyboard and numpad)
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                return;
            }

            // Allow decimal point (both main keyboard and numpad)
            if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                return;
            }

            // Block all other keys
            e.Handled = true;
        }

        private static bool IsValidDecimalInput(string input)
        {
            if (string.IsNullOrEmpty(input)) return true;

            // Check if it matches decimal pattern (max 1 decimal place)
            if (!DecimalRegex.IsMatch(input)) return false;

            // Check for multiple decimal points
            var decimalCount = 0;
            foreach (var c in input)
            {
                if (c == '.')
                {
                    decimalCount++;
                    if (decimalCount > 1) return false;
                }
            }

            // Check decimal places count
            var decimalIndex = input.IndexOf('.');
            if (decimalIndex >= 0)
            {
                var decimalPlaces = input.Length - decimalIndex - 1;
                if (decimalPlaces > 1) return false;
            }

            // Allow single decimal point temporarily
            if (input == ".") return true;

            // Try to parse as decimal to ensure it's a valid number
            return decimal.TryParse(input, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _);
        }
    }
}