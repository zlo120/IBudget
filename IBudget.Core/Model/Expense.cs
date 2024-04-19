using System.Globalization;
using System.Text;

namespace Core.Model
{
    public class Expense : DataEntry
    {
        public string? Notes { get; set; }

        public override string ToString()
        {
            return $"ID: {ID,-5} Date: {Date.ToString("dd/MM/yyyy"),-15} Notes: {Notes,-20} Amount: {Amount.ToString("C", CultureInfo.GetCultureInfo("en-US")),-10}\n\tTags: {GetTags()}\n";
        }
    }
}