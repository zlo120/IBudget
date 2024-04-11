using Aspose.Cells.Charts;
using ClosedXML.Excel;
using Core.Utils;

namespace Spreadsheet
{
    public class Generator
    {
        public static void GenerateSpreadsheet()
        {
            var calendar = Calendar.InitiateCalendar();
            var workbook = new XLWorkbook();

            var tableOfContentsWS = workbook.Worksheets.Add("Table of Contents");
            tableOfContentsWS.Cell(1, 1).Value = "Table of Contents";
            tableOfContentsWS.Cell(1, 1).Style.Font.Bold = true;
            tableOfContentsWS.Column(1).Width = 5 * tableOfContentsWS.Column(1).Width;

            int nextFreeCell = 2;

            foreach (var month in calendar.Months)
            {
                // calculating formulas
                var foodFormula = $"_xlfn.SUM(";
                var totalSpendingFormula = $"_xlfn.SUM(";
                var totalIncomeFormula = $"_xlfn.SUM(";
                foreach (var week in month.Weeks)
                {
                    if (week == month.Weeks.Last())
                    {
                        foodFormula += $"'{week.Label}'!K3";
                        totalSpendingFormula += $"'{week.Label}'!K5";
                        totalIncomeFormula += $"'{week.Label}'!L5";
                    }
                    else
                    {
                        foodFormula += $"'{week.Label}'!K3,";
                        totalSpendingFormula += $"'{week.Label}'!K5,";
                        totalIncomeFormula += $"'{week.Label}'!L5,";
                    }
                }
                foodFormula += ")";
                totalSpendingFormula += ")";
                totalIncomeFormula += ")";

                var monthSummaryWorksheet = workbook.Worksheets.Add(month.MonthName);

                // adding this month to the table of contents
                tableOfContentsWS.Cell(nextFreeCell, 1).Value = month.MonthName;
                tableOfContentsWS.Cell(nextFreeCell, 1).SetHyperlink(new XLHyperlink($"'{month.MonthName}'!A1"));

                monthSummaryWorksheet.Cell(1, 1).Value = "Total money spent on food";
                monthSummaryWorksheet.Cell(1, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(2, 1).FormulaA1 = foodFormula;

                monthSummaryWorksheet.Cell(1, 2).Value = "Food money budget remaining";
                monthSummaryWorksheet.Cell(1, 2).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(2, 2).Value = "To be added"; // edit this formula!

                monthSummaryWorksheet.Cell(4, 1).Value = "Total money spent this month";
                monthSummaryWorksheet.Cell(4, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(5, 1).FormulaA1 = totalSpendingFormula; 

                monthSummaryWorksheet.Cell(4, 2).Value = "Total income this month";
                monthSummaryWorksheet.Cell(4, 2).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(5, 2).FormulaA1 = totalIncomeFormula; 

                monthSummaryWorksheet.Cell(7, 1).Value = "Monthy budget balance";
                monthSummaryWorksheet.Cell(7, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(8, 1).Value = "To be added"; // edit this formula!

                monthSummaryWorksheet.Column(1).AdjustToContents();
                monthSummaryWorksheet.Column(2).AdjustToContents();

                monthSummaryWorksheet.Cell(7, 2).Value = "Go back to Table of Contents";
                monthSummaryWorksheet.Cell(7, 2).SetHyperlink(new XLHyperlink($"'Table of Contents'!A1"));

                var col1 = monthSummaryWorksheet.Column(1);
                col1.Style.NumberFormat.Format = "$#,##0.00";

                var col2 = monthSummaryWorksheet.Column(2);
                col2.Style.NumberFormat.Format = "$#,##0.00";

                nextFreeCell++;

                foreach (var week in month.Weeks)
                {
                    var weekWorksheet = workbook.Worksheets.Add(week.Label);
                    tableOfContentsWS.Cell(nextFreeCell, 1).Value = week.Label;
                    tableOfContentsWS.Cell(nextFreeCell, 1).SetHyperlink(new XLHyperlink($"'{week.Label}'!A1"));
                    nextFreeCell++;

                    weekWorksheet.Cell(1, 1).Value = week.Label;
                    weekWorksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    weekWorksheet.Cell(1, 1).Style.Font.Bold = true;

                    weekWorksheet.Range(
                            weekWorksheet.Cell(1, 1),
                            weekWorksheet.Cell(1, 5)
                        ).Merge();

                    weekWorksheet.Cell(1, 6).Value = "Go back to Table of Contents";
                    weekWorksheet.Cell(1, 6).SetHyperlink(new XLHyperlink($"'Table of Contents'!A1"));

                    weekWorksheet.Cell(2, 1).Value = "Petrol";
                    weekWorksheet.Cell(2, 2).Value = "Fitness";
                    weekWorksheet.Cell(2, 3).Value = "Bills";
                    weekWorksheet.Cell(2, 4).Value = "Food";
                    weekWorksheet.Cell(2, 5).Value = "Other";
                    weekWorksheet.Cell(2, 6).Value = "Description of other";
                    weekWorksheet.Column(6).Width = 3 * weekWorksheet.Column(6).Width;


                    weekWorksheet.Cell(2, 8).Value = "Income";
                    weekWorksheet.Cell(2, 9).Value = "Description";
                    weekWorksheet.Column(9).Width = 3 * weekWorksheet.Column(9).Width;


                    weekWorksheet.Cell(1, 11).Value = "Summary";
                    weekWorksheet.Cell(1, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    weekWorksheet.Cell(1, 11).Style.Font.Bold = true;

                    weekWorksheet.Range(
                            weekWorksheet.Cell(1, 11),
                            weekWorksheet.Cell(1, 12)
                        ).Merge();

                    weekWorksheet.Cell(2, 11).Value = "Money spent on food";
                    weekWorksheet.Cell(3, 11).FormulaA1 = "_xlfn.SUM(D:D)";

                    weekWorksheet.Cell(2, 12).Value = "Money spent on other";
                    weekWorksheet.Cell(3, 12).FormulaA1 = "_xlfn.SUM(E:E)";

                    weekWorksheet.Cell(4, 11).Value = "Total Outgoing";
                    weekWorksheet.Cell(5, 11).FormulaA1 = "_xlfn.SUM(A:E)";

                    weekWorksheet.Cell(4, 12).Value = "Total Income";
                    weekWorksheet.Cell(5, 12).FormulaA1 = "_xlfn.SUM(H:H)";

                    weekWorksheet.Column(11).AdjustToContents();
                    weekWorksheet.Column(12).AdjustToContents();

                    for (int i = 1; i <= 5; i++)
                    {
                        var column = weekWorksheet.Column(i);
                        column.Style.NumberFormat.Format = "$#,##0.00";
                    }

                    var column8 = weekWorksheet.Column(8);
                    column8.Style.NumberFormat.Format = "$#,##0.00";
                }

                nextFreeCell++;
            }

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IBudgetterSpreadsheet.xlsx";

            workbook.SaveAs(path);

            // array of all month names
            string[] monthNames = calendar.Months.Select(month => month.MonthName).ToArray();
            string[] weekLabels = calendar.Months.SelectMany(month => month.Weeks.Select(week => week.Label)).ToArray();

            GenerateCharts(path, monthNames, weekLabels);
        }

        private static void GenerateCharts(string path, string[] monthNames, string[] weekLabels)
        {
            var workBook = new Aspose.Cells.Workbook(path);

            var ws = workBook.Worksheets[0];

            foreach (var month in monthNames)
            {
                var workSheet = workBook.Worksheets.Where(s => s.Name == month).FirstOrDefault();

                int chartIndex = workSheet.Charts.Add(ChartType.Column, 3, 3, 20, 12);

                var chart = workSheet.Charts[chartIndex];

                chart.Title.Text = $"{month} Financial Summary";
                chart.ValueAxis.Title.Text = "Amount $AUD";

                chart.Legend.Position = LegendPositionType.Bottom;

                chart.Title.TextFont.Size = 14;
                chart.CategoryAxis.Title.Text = "";
                chart.ValueAxis.Title.TextFont.IsItalic = true;

                chart.ValueAxis.MajorUnit = 1000; // Set major unit for Y-axis
                chart.ValueAxis.MinorUnit = 100; // Set minor unit for Y-axis
                chart.ValueAxis.MajorGridLines.IsVisible = true; // Show major gridlines

                chart.NSeries.Add("A5", true); // Replace with your actual cell range
                chart.NSeries.Add("B5", true); // Replace with your actual cell range


                chart.NSeries[0].Name = "Total outgoing"; // Replace with your actual series name
                chart.NSeries[1].Name = "Total income"; // Replace with your actual series name
            }

            foreach (var week in weekLabels)
            {
                var workSheet = workBook.Worksheets.Where(s => s.Name == week).FirstOrDefault();

                int chartIndex = workSheet.Charts.Add(ChartType.Column, 6, 9, 18, 13);

                var chart = workSheet.Charts[chartIndex];

                chart.Title.Text = "Weekly Financial Summary";
                chart.ValueAxis.Title.Text = "Amount $AUD";

                chart.Legend.Position = LegendPositionType.Bottom;

                chart.Title.TextFont.Size = 14;
                chart.CategoryAxis.Title.Text = "";
                chart.ValueAxis.Title.TextFont.IsItalic = true;

                chart.ValueAxis.MajorUnit = 1000; // Set major unit for Y-axis
                chart.ValueAxis.MinorUnit = 100; // Set minor unit for Y-axis
                chart.ValueAxis.MajorGridLines.IsVisible = true; // Show major gridlines

                chart.NSeries.Add("K5", true); // Replace with your actual cell range
                chart.NSeries.Add("L5", true); // Replace with your actual cell range


                chart.NSeries[0].Name = "Total outgoing"; // Replace with your actual series name
                chart.NSeries[1].Name = "Total income"; // Replace with your actual series name
            }

            workBook.Save(path);

            Console.WriteLine($"Your excel spreadsheet is ready to open at \"{path}\"");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }
    }
}