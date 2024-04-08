using ClosedXML.Excel;
using Core;

namespace Spreadsheet
{
    public class Generator
    {
        public static void GenerateSpreadsheet()
        {
            var calendar = Calendar.InitiateCalendar();
            var workbook = new XLWorkbook();

            foreach(var month in calendar.Months)
            {
                var monthSummaryWorksheet = workbook.Worksheets.Add(month.MonthName);
                foreach(var week in month.Weeks)
                {
                    var weekWorksheet = workbook.Worksheets.Add(week.Label);

                    weekWorksheet.Cell(1, 1).Value = "Date";
                    weekWorksheet.Cell(1, 2).Value = "Income";
                    weekWorksheet.Cell(1, 3).Value = "Expense";
                    weekWorksheet.Cell(1, 4).Value = "Tags";

                    weekWorksheet.Cell(2, 1).FormulaA1 = "_xlfn.SUM(E:E)";
                }
            }

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IBudgetterSpreadsheet.xlsx";

            workbook.SaveAs(path);
        }
    }
}