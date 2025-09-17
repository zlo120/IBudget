using Aspose.Cells.Charts;
using ClosedXML.Excel;
using IBudget.Core.DTO;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Utils;
using IBudget.Spreadsheet.Interfaces;

namespace IBudget.Core.Services
{
    public class SpreadSheetGeneratorService : ISpreadSheetGeneratorService
    {
        private readonly IPopulator _populator;
        private readonly ITagService _tagService;
        private readonly IFinancialGoalService _financialGoalService;

        public SpreadSheetGeneratorService(IPopulator populator, ITagService tagService, IFinancialGoalService financialGoalService)
        {
            _populator = populator;
            _tagService = tagService;
            _financialGoalService = financialGoalService;
        }

        private async Task<List<Tag>> GetAllTrackedTags()
        {
            return [.. (await _tagService.GetAll())
                .Where(tag => tag.IsTracked)
                .OrderBy(s => s.Name)
                .ToList()];
        }
        public async Task<string> GenerateSpreadsheet()
        {
            var trackedTagsList = await GetAllTrackedTags();
            var financialGoals = await _financialGoalService.GetAll();
            var calendar = Calendar.InitiateCalendar();
            var workbook = new XLWorkbook();

            var tableOfContentsWS = workbook.Worksheets.Add("Table of Contents");
            tableOfContentsWS.Cell(1, 1).Value = "Table of Contents";
            tableOfContentsWS.Cell(1, 1).Style.Font.Bold = true;
            tableOfContentsWS.Column(1).Width = 5 * tableOfContentsWS.Column(1).Width;

            GenerateBudgetSheet(workbook, financialGoals);

            int nextFreeCell = 2;

            GenerateMonths(calendar, workbook, tableOfContentsWS, ref nextFreeCell, trackedTagsList, financialGoals);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\{calendar.YearNumber} - IBudgetSheet.xlsx";

            workbook.SaveAs(path);

            // array of all month names
            string[] monthNames = calendar.Months.Select(month => month.MonthName).ToArray();
            string[] weekLabels = calendar.Months.SelectMany(month => month.Weeks.Select(week => week.Label)).ToArray();

            var numOfTrackedTags = trackedTagsList.Count;
            GenerateCharts(path, monthNames, weekLabels, numOfTrackedTags, trackedTagsList, financialGoals);

            // deleting the last sheet
            workbook = new XLWorkbook(path);
            var lastSheet = workbook.Worksheets.Last();
            lastSheet.Delete();

            var firstSheet = workbook.Worksheets.First();

            workbook = await _populator.PopulateSpreadsheet(workbook);

            workbook.Save();

            return path;
        }

        private void GenerateCharts(string path, string[] monthNames, string[] weekLabels, int numOfTrackedTags, List<Tag> trackedTagsList, List<FinancialGoal> financialGoals)
        {
            var totalOutgoingColumnNum = numOfTrackedTags + 7;
            var workBook = new Aspose.Cells.Workbook(path);

            var ws = workBook.Worksheets[0];

            foreach (var month in monthNames)
            {
                var workSheet = workBook.Worksheets.Where(s => s.Name == month).FirstOrDefault();
                int chartIndex = workSheet!.Charts.Add(ChartType.Column, 3, 3, 20, 12);

                var chart = workSheet.Charts[chartIndex];

                chart.Title.Text = $"{month} Financial Summary";
                chart.ValueAxis.Title.Text = "Amount $AUD";

                chart.Legend.Position = LegendPositionType.Bottom;

                chart.Title.Font.Size = 14;
                chart.CategoryAxis.Title.Text = "";
                chart.ValueAxis.Title.Font.IsItalic = true;

                chart.ValueAxis.MajorUnit = 1000; // Set major unit for Y-axis
                chart.ValueAxis.MinorUnit = 100; // Set minor unit for Y-axis
                chart.ValueAxis.MajorGridLines.IsVisible = true; // Show major gridlines

                chart.NSeries.Add("A11", true); // Replace with your actual cell range
                chart.NSeries.Add("B11", true); // Replace with your actual cell range


                chart.NSeries[0].Name = "Total outgoing"; // Replace with your actual series name
                chart.NSeries[1].Name = "Total income"; // Replace with your actual series name
                chart.NSeries.CategoryData = ""; // Removes the default '1' label
            }

            foreach (var week in weekLabels)
            {
                var workSheet = workBook.Worksheets.Where(s => s.Name == week).FirstOrDefault();

                var startCellColumn = trackedTagsList.Count + 6;
                var endCelColumn = startCellColumn + 2;

                var startRow = (int) Math.Ceiling((decimal)trackedTagsList.Count/2)*2 + 2;
                var endRow = startRow + 10;

                int chartIndex = workSheet!.Charts.Add(ChartType.Column, startRow, startCellColumn, endRow, endCelColumn); // 8: because we have a fixed number of cells from the top, *: determined by the number of tracked tags,
                                                                                                               // 20: because we have a fixed height for the chart, *: determined by the number of tracked tags

                var chart = workSheet.Charts[chartIndex];

                chart.Title.Text = "Weekly Financial Summary";
                chart.ValueAxis.Title.Text = "Amount $AUD";

                chart.Legend.Position = LegendPositionType.Bottom;

                chart.Title.Font.Size = 14;
                chart.CategoryAxis.Title.Text = "";
                chart.ValueAxis.Title.Font.IsItalic = true;

                chart.ValueAxis.MajorUnit = 1000; // Set major unit for Y-axis
                chart.ValueAxis.MinorUnit = 100; // Set minor unit for Y-axis
                chart.ValueAxis.MajorGridLines.IsVisible = true; // Show major gridlines

                //var totalRows = (int)Math.Ceiling((decimal)trackedTagsList.Count / 2); // `total` as in the "Total Outgoing" and "Total Income" rows

                var totalOutgoingColumn = $"{IntToChar(totalOutgoingColumnNum)}{startRow-1}";
                var totalIncomingColumn = $"{IntToChar(totalOutgoingColumnNum + 1)}{startRow-1}";

                chart.NSeries.Add(totalOutgoingColumn, true); // Replace with your actual cell range
                chart.NSeries.Add(totalIncomingColumn, true); // Replace with your actual cell range

                chart.NSeries[0].Name = "Total outgoing"; // Replace with your actual series name
                chart.NSeries[1].Name = "Total income"; // Replace with your actual series name
                chart.NSeries.CategoryData = ""; // Removes the default '1' label
            }

            workBook.Save(path);
        }

        private void GenerateMonths(YearDTO calendar, XLWorkbook workbook, IXLWorksheet tableOfContentsWS, ref int nextFreeCell, List<Tag> trackedTagsList, List<FinancialGoal> financialGoals)
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

                GenerateWeeks(month, workbook, tableOfContentsWS, ref nextFreeCell, trackedTagsList, financialGoals);

                nextFreeCell++;
            }
        }

        private void GenerateWeeks(MonthDTO month, XLWorkbook workbook, IXLWorksheet tableOfContentsWS, ref int nextFreeCell, List<Tag> trackedTagsList, List<FinancialGoal> financialGoals)
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

                for (int columnCounter = 1; columnCounter <= trackedTagsList.Count; columnCounter++)
                {
                    weekWorksheet.Cell(2, columnCounter).Value = trackedTagsList[columnCounter - 1].Name;
                }

                var otherColumn = trackedTagsList.Count + 1;
                weekWorksheet.Cell(2, otherColumn).Value = "Other";
                weekWorksheet.Cell(2, otherColumn + 1).Value = "Description of other";
                weekWorksheet.Column(otherColumn + 1).Width = 3 * weekWorksheet.Column(6).Width;

                var incomeColumn = otherColumn + 3;
                weekWorksheet.Cell(2, incomeColumn).Value = "Income";
                weekWorksheet.Cell(2, incomeColumn + 1).Value = "Description";
                weekWorksheet.Column(incomeColumn + 1).Width = 3 * weekWorksheet.Column(9).Width;

                var summaryColumn = incomeColumn + 3;
                weekWorksheet.Cell(1, summaryColumn).Value = "Summary";
                weekWorksheet.Cell(1, summaryColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                weekWorksheet.Cell(1, summaryColumn).Style.Font.Bold = true;

                weekWorksheet.Range(
                        weekWorksheet.Cell(1, summaryColumn),
                        weekWorksheet.Cell(1, summaryColumn + 1)
                    ).Merge();

                // financial goals summary
                var currentTitleRow = 2;
                var offsetColumn = false;
                foreach(var financialGoal in financialGoals)
                {
                    var titleRow = currentTitleRow;
                    var valueRow = titleRow + 1;

                    var tagColumn = trackedTagsList.FindIndex(tag => tag.Name.Equals(financialGoal.Name));
                    var columnLetter = IntToChar(tagColumn + 1);
                    weekWorksheet.Cell(titleRow, summaryColumn + (offsetColumn ? 1 : 0)).Value = $"Money spent on {financialGoal.Name}";
                    weekWorksheet.Cell(valueRow, summaryColumn + (offsetColumn ? 1 : 0)).FormulaA1 = $"_xlfn.SUM({columnLetter}:{columnLetter})";
                    if (offsetColumn)
                    {
                        currentTitleRow += 2;
                        offsetColumn = false;
                    }
                    else
                    {
                        offsetColumn = true;
                    }
                }

                var otherTitleRow = currentTitleRow;
                var otherValueRow = otherTitleRow + 1;   
                var otherColLetter = IntToChar(otherColumn);
                weekWorksheet.Cell(otherTitleRow, summaryColumn + (offsetColumn ? 1 : 0)).Value = "Money spent on other";
                weekWorksheet.Cell(otherValueRow, summaryColumn + (offsetColumn ? 1 : 0)).FormulaA1 = $"_xlfn.SUM({otherColLetter}:{otherColLetter})";

                var outgoingIncomingRow = otherTitleRow + 2;
                var outgoingIncomingValueRow = outgoingIncomingRow + 1;
                weekWorksheet.Cell(outgoingIncomingRow, summaryColumn).Value = "Total Outgoing";
                weekWorksheet.Cell(outgoingIncomingValueRow, summaryColumn).FormulaA1 = $"_xlfn.SUM(A:{otherColLetter})";

                var incomeColumnLetter = IntToChar(incomeColumn);
                weekWorksheet.Cell(outgoingIncomingRow, summaryColumn + 1).Value = "Total Income";
                weekWorksheet.Cell(outgoingIncomingValueRow, summaryColumn + 1).FormulaA1 = $"_xlfn.SUM({incomeColumnLetter}:{incomeColumnLetter})";

                weekWorksheet.Column(summaryColumn).AdjustToContents();
                weekWorksheet.Column(summaryColumn + 1).AdjustToContents();

                var column10 = weekWorksheet.Column(incomeColumn);
                column10.Style.NumberFormat.Format = "$#,##0.00";


                var column13 = weekWorksheet.Column(summaryColumn);
                column13.Style.NumberFormat.Format = "$#,##0.00";

                var column14 = weekWorksheet.Column(summaryColumn + 1);
                column14.Style.NumberFormat.Format = "$#,##0.00";
            }
        }

        private static void GenerateBudgetSheet(XLWorkbook workbook, List<FinancialGoal> financialGoals)
        {
            var budgetSheet = workbook.Worksheets.Add("Monthly Budget");
            budgetSheet.Cell(1, 1).Value = "Name";
            budgetSheet.Cell(1, 1).Style.Font.Bold = true;
            budgetSheet.Cell(1, 2).Value = "Amount";
            budgetSheet.Cell(1, 2).Style.Font.Bold = true;

            for (int i = 0; i < financialGoals.Count; i++)
            {
                budgetSheet.Cell(i + 2, 1).Value = financialGoals[i].Name;
                budgetSheet.Cell(i + 2, 2).Value = financialGoals[i].TargetAmount;
            }

            var column2 = budgetSheet.Column(2);
            column2.Style.NumberFormat.Format = "$#,##0.00";

            budgetSheet.Cell(1, 4).Value = "Total Monthly Expense Budget";
            budgetSheet.Cell(1, 4).Style.Font.Bold = true;
            budgetSheet.Cell(2, 4).FormulaA1 = "_xlfn.SUM(B:B)";

            var column4 = budgetSheet.Column(4);
            column4.Style.NumberFormat.Format = "$#,##0.00";


            budgetSheet.Column(1).AdjustToContents();
        }

        private static string IntToChar(int n)
        {
            if (n >= 1 && n <= 26)
            {
                return ((char)(n + 64)).ToString().ToUpper();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(n), "Value must be between 1 and 26.");
            }
        }
    }
}