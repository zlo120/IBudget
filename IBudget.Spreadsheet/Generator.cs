using Aspose.Cells.Charts;
using ClosedXML.Excel;
using IBudget.Core.Model;
using IBudget.Core.Utils;

namespace IBudget.Spreadsheet
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

            GenerateBudgetSheet(workbook);

            int nextFreeCell = 2;

            GenerateMonths(calendar, workbook, tableOfContentsWS, ref nextFreeCell);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\{calendar.YearNumber} - IBudgetSheet.xlsx";

            workbook.SaveAs(path);

            // array of all month names
            string[] monthNames = calendar.Months.Select(month => month.MonthName).ToArray();
            string[] weekLabels = calendar.Months.SelectMany(month => month.Weeks.Select(week => week.Label)).ToArray();

            GenerateCharts(path, monthNames, weekLabels);

            // deleting the last sheet
            workbook = new XLWorkbook(path);
            var lastSheet = workbook.Worksheets.Last();
            lastSheet.Delete();

            var firstSheet = workbook.Worksheets.First();
            workbook.Save();

            Console.WriteLine($"Your excel spreadsheet is ready to open at \"{path}\"");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
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

                chart.NSeries.Add("A11", true); // Replace with your actual cell range
                chart.NSeries.Add("B11", true); // Replace with your actual cell range


                chart.NSeries[0].Name = "Total outgoing"; // Replace with your actual series name
                chart.NSeries[1].Name = "Total income"; // Replace with your actual series name
            }

            foreach (var week in weekLabels)
            {
                var workSheet = workBook.Worksheets.Where(s => s.Name == week).FirstOrDefault();

                int chartIndex = workSheet.Charts.Add(ChartType.Column, 7, 11, 20, 15);

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

                chart.NSeries.Add("M7", true); // Replace with your actual cell range
                chart.NSeries.Add("N7", true); // Replace with your actual cell range

                chart.NSeries[0].Name = "Total outgoing"; // Replace with your actual series name
                chart.NSeries[1].Name = "Total income"; // Replace with your actual series name
            }

            workBook.Save(path);
        }
    
        private static void GenerateMonths(Year calendar, XLWorkbook workbook, IXLWorksheet tableOfContentsWS, ref int nextFreeCell)
        {
            foreach (var month in calendar.Months)
            {
                // calculating formulas
                var foodFormula = $"_xlfn.SUM(";
                var totalSpendingFormula = $"_xlfn.SUM(";
                var totalIncomeFormula = $"_xlfn.SUM(";
                var totalPublicTransportFormula = $"_xlfn.SUM(";
                var totalAlcoholFormula = $"_xlfn.SUM(";
                foreach (var week in month.Weeks)
                {
                    if (week == month.Weeks.Last())
                    {
                        foodFormula += $"'{week.Label}'!M3";
                        totalSpendingFormula += $"'{week.Label}'!M7";
                        totalIncomeFormula += $"'{week.Label}'!N7";
                        totalPublicTransportFormula += $"'{week.Label}'!N5";
                        totalAlcoholFormula += $"'{week.Label}'!M5";
                    }
                    else
                    {
                        foodFormula += $"'{week.Label}'!M3,";
                        totalSpendingFormula += $"'{week.Label}'!M7,";
                        totalIncomeFormula += $"'{week.Label}'!N7,";
                        totalPublicTransportFormula += $"'{week.Label}'!N5,";
                        totalAlcoholFormula += $"'{week.Label}'!M5,";
                    }
                }

                foodFormula += ")";
                totalSpendingFormula += ")";
                totalIncomeFormula += ")";
                totalPublicTransportFormula += ")";
                totalAlcoholFormula += ")";

                var monthSummaryWorksheet = workbook.Worksheets.Add(month.MonthName);

                // adding this month to the table of contents
                tableOfContentsWS.Cell(nextFreeCell, 1).Value = month.MonthName;
                tableOfContentsWS.Cell(nextFreeCell, 1).SetHyperlink(new XLHyperlink($"'{month.MonthName}'!A1"));

                monthSummaryWorksheet.Cell(1, 1).Value = "Total money spent on food";
                monthSummaryWorksheet.Cell(1, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(2, 1).FormulaA1 = foodFormula;

                monthSummaryWorksheet.Cell(1, 2).Value = "Total money spent on alcohol";
                monthSummaryWorksheet.Cell(1, 2).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(2, 2).FormulaA1 = totalAlcoholFormula;

                monthSummaryWorksheet.Cell(4, 1).Value = "Total money spent on public transport";
                monthSummaryWorksheet.Cell(4, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(5, 1).FormulaA1 = totalPublicTransportFormula;

                monthSummaryWorksheet.Cell(4, 2).Value = "Public transport budget balance";
                monthSummaryWorksheet.Cell(4, 2).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(5, 2).FormulaA1 = "='Monthly Budget'!B4 - A5";

                monthSummaryWorksheet.Cell(7, 1).Value = "Monthly budget balance";
                monthSummaryWorksheet.Cell(7, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(8, 1).FormulaA1 = "='Monthly Budget'!D2 - A11";

                monthSummaryWorksheet.Cell(7, 2).Value = "Monthy food/alcohol budget balance";
                monthSummaryWorksheet.Cell(7, 2).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(8, 2).FormulaA1 = "='Monthly Budget'!B2 + 'Monthly Budget'!B3 - B2 - A2";

                monthSummaryWorksheet.Cell(10, 1).Value = "Total money spent this month";
                monthSummaryWorksheet.Cell(10, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(11, 1).FormulaA1 = totalSpendingFormula;

                monthSummaryWorksheet.Cell(10, 2).Value = "Total income this month";
                monthSummaryWorksheet.Cell(10, 2).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(11, 2).FormulaA1 = totalIncomeFormula;

                monthSummaryWorksheet.Column(1).AdjustToContents();
                monthSummaryWorksheet.Column(2).AdjustToContents();

                monthSummaryWorksheet.Cell(13, 1).Value = "Go back to Table of Contents";
                monthSummaryWorksheet.Cell(13, 1).SetHyperlink(new XLHyperlink($"'Table of Contents'!A1"));

                var col1 = monthSummaryWorksheet.Column(1);
                col1.Style.NumberFormat.Format = "$#,##0.00";

                var col2 = monthSummaryWorksheet.Column(2);
                col2.Style.NumberFormat.Format = "$#,##0.00";

                nextFreeCell++;

                GenerateWeeks(month, workbook, tableOfContentsWS, ref nextFreeCell);

                nextFreeCell++;
            }
        }

        private static void GenerateWeeks(Core.Model.Month month, XLWorkbook workbook, IXLWorksheet tableOfContentsWS, ref int nextFreeCell)
        {
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
                weekWorksheet.Cell(2, 4).Value = "Public transport";
                weekWorksheet.Cell(2, 5).Value = "Food";
                weekWorksheet.Cell(2, 6).Value = "Alcohol";
                weekWorksheet.Cell(2, 7).Value = "Other";
                weekWorksheet.Cell(2, 8).Value = "Description of other";
                weekWorksheet.Column(8).Width = 3 * weekWorksheet.Column(6).Width;


                weekWorksheet.Cell(2, 10).Value = "Income";
                weekWorksheet.Cell(2, 11).Value = "Description";
                weekWorksheet.Column(11).Width = 3 * weekWorksheet.Column(9).Width;


                weekWorksheet.Cell(1, 13).Value = "Summary";
                weekWorksheet.Cell(1, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                weekWorksheet.Cell(1, 13).Style.Font.Bold = true;

                weekWorksheet.Range(
                        weekWorksheet.Cell(1, 13),
                        weekWorksheet.Cell(1, 14)
                    ).Merge();

                weekWorksheet.Cell(2, 13).Value = "Money spent on food";
                weekWorksheet.Cell(3, 13).FormulaA1 = "_xlfn.SUM(E:E)";

                weekWorksheet.Cell(2, 14).Value = "Money spent on other";
                weekWorksheet.Cell(3, 14).FormulaA1 = "_xlfn.SUM(G:G)";

                weekWorksheet.Cell(4, 13).Value = "Money spent on alcohol";
                weekWorksheet.Cell(5, 13).FormulaA1 = "_xlfn.SUM(F:F)";

                weekWorksheet.Cell(4, 14).Value = "Money spent on public transport";
                weekWorksheet.Cell(5, 14).FormulaA1 = "_xlfn.SUM(D:D)";

                weekWorksheet.Cell(6, 13).Value = "Total Outgoing";
                weekWorksheet.Cell(7, 13).FormulaA1 = "_xlfn.SUM(A:G)";

                weekWorksheet.Cell(6, 14).Value = "Total Income";
                weekWorksheet.Cell(7, 14).FormulaA1 = "_xlfn.SUM(J:J)";

                weekWorksheet.Column(4).AdjustToContents();
                weekWorksheet.Column(13).AdjustToContents();
                weekWorksheet.Column(14).AdjustToContents();

                for (int i = 1; i <= 7; i++)
                {
                    var column = weekWorksheet.Column(i);
                    column.Style.NumberFormat.Format = "$#,##0.00";
                }

                var column10 = weekWorksheet.Column(10);
                column10.Style.NumberFormat.Format = "$#,##0.00";


                var column13 = weekWorksheet.Column(13);
                column13.Style.NumberFormat.Format = "$#,##0.00";

                var column14 = weekWorksheet.Column(14);
                column14.Style.NumberFormat.Format = "$#,##0.00";
            }
        }

        private static void GenerateBudgetSheet(XLWorkbook workbook)
        {
            var budgetSheet = workbook.Worksheets.Add("Monthly Budget");
            budgetSheet.Cell(1, 1).Value = "Name";
            budgetSheet.Cell(1, 1).Style.Font.Bold = true;
            budgetSheet.Cell(2, 1).Value = "Food";
            budgetSheet.Cell(3, 1).Value = "Alcohol";
            budgetSheet.Cell(4, 1).Value = "Public transport";

            budgetSheet.Cell(1,2).Value = "Amount";
            budgetSheet.Cell(1,2).Style.Font.Bold = true;

            var column2 = budgetSheet.Column(2);
            column2.Style.NumberFormat.Format = "$#,##0.00";

            budgetSheet.Cell(1,4).Value = "Total Monthly Expense Budget";
            budgetSheet.Cell(1,4).Style.Font.Bold = true;
            budgetSheet.Cell(2,4).FormulaA1 = "_xlfn.SUM(B:B)";

            var column4 = budgetSheet.Column(4);
            column4.Style.NumberFormat.Format = "$#,##0.00";


            budgetSheet.Column(1).AdjustToContents();
        }
    }
}