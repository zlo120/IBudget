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

            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Stacks",
                $"{calendar.YearNumber}-Budget.xlsx"
            );

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
                int chartIndex = workSheet!.Charts.Add(ChartType.Column, 4, 4, 20, 12);

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

                var outgoingRowNumber = financialGoals.Count * 2 + 4;
                var incomingRowNumber = outgoingRowNumber + 2;

                chart.NSeries.Add($"B{outgoingRowNumber}", true); // Replace with your actual cell range
                chart.NSeries.Add($"A{incomingRowNumber}", true); // Replace with your actual cell range

                chart.NSeries[0].Name = "Total outgoing"; // Replace with your actual series name
                chart.NSeries[1].Name = "Total income"; // Replace with your actual series name
                chart.NSeries.CategoryData = ""; // Removes the default '1' label
            }

            foreach (var week in weekLabels)
            {
                var workSheet = workBook.Worksheets.Where(s => s.Name == week).FirstOrDefault();

                var startCellColumn = trackedTagsList.Count + 6;
                var endCelColumn = startCellColumn + 2;

                var startRow = (int) Math.Ceiling((decimal)financialGoals.Count/2)*2 + 6;
                var endRow = startRow + 10;

                int chartIndex = workSheet!.Charts.Add(ChartType.Column, startRow, startCellColumn, endRow, endCelColumn);

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
                var monthSummaryWorksheet = workbook.Worksheets.Add(month.MonthName);

                // adding this month to the table of contents
                tableOfContentsWS.Cell(nextFreeCell, 1).Value = month.MonthName;
                tableOfContentsWS.Cell(nextFreeCell, 1).SetHyperlink(new XLHyperlink($"'{month.MonthName}'!A1"));

                var offsetColumn = false;

                var totalRow = (int)Math.Ceiling((decimal)financialGoals.Count / 2) * 2 + 5;
                var totalOutgoingColumn = trackedTagsList.Count + 7;
                var totalIncomeColumn = totalOutgoingColumn + 1;

                var startToSumFormula = "_xlfn.SUM(";
                var endToSumFormula = ")";

                var spendingDictionary = new Dictionary<string, string>();
                var remainingDictionary = new Dictionary<string, string>();

                spendingDictionary.Add("Total Outgoing", startToSumFormula);
                spendingDictionary.Add("Total Income", startToSumFormula);

                foreach (var financialGoal in financialGoals)
                {
                    spendingDictionary.Add(financialGoal.Name, startToSumFormula);
                }

                foreach (var week in month.Weeks)
                {
                    offsetColumn = false;
                    var currentFormulaRow = 3;
                    // check is last week
                    var isLastWeek = week == month.Weeks.Last();
                    for (int i = 0; i < financialGoals.Count; i++)
                    {
                        var financialGoalName = financialGoals[i].Name;
                        var currentFormulaColumn = totalOutgoingColumn + (offsetColumn ? 1 : 0);

                        var comma = isLastWeek ? "" : ",";
                        spendingDictionary[financialGoalName] += $"'{week.Label}'!{CoordinatesToCellReference(currentFormulaRow, currentFormulaColumn)}{comma}";

                        if (offsetColumn)
                        {
                            currentFormulaRow += 2;
                            offsetColumn = false;
                        }
                        else
                        {
                            offsetColumn = true;
                        }
                    }

                    spendingDictionary["Total Outgoing"] += $"'{week.Label}'!{CoordinatesToCellReference(totalRow, totalOutgoingColumn)}{(isLastWeek ? "" : ",")}";
                    spendingDictionary["Total Income"] += $"'{week.Label}'!{CoordinatesToCellReference(totalRow, totalIncomeColumn)}{(isLastWeek ? "" : ",")}";
                }

                for (int i = 0; i < financialGoals.Count; i++)
                {
                    var financialGoal = financialGoals[i];

                    var monthlyBudgetCredit = $"'Monthly Budget'!B{i+2}";

                    var entireFormula = spendingDictionary[financialGoal.Name] + endToSumFormula;
                    spendingDictionary[financialGoal.Name] += endToSumFormula;
                    remainingDictionary[financialGoal.Name] = $"{monthlyBudgetCredit}-{entireFormula}";
                }

                spendingDictionary["Total Outgoing"] += endToSumFormula;
                spendingDictionary["Total Income"] += endToSumFormula;
                var monthlyBudgetRef = $"'Monthly Budget'!D2";
                remainingDictionary["Total Outgoing"] = $"{monthlyBudgetRef}-{spendingDictionary["Total Outgoing"]}";

                var currentRow = 1;
                for (int i = 0; i < financialGoals.Count; i++)
                {
                    var financialGoal = financialGoals[i];
                    var monthlyBudgetCredit = $"'Monthly Budget'!B{i + 2}";

                    monthSummaryWorksheet.Cell(currentRow, 1).Value = $"Budget for {financialGoal.Name}";
                    monthSummaryWorksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    monthSummaryWorksheet.Cell(currentRow + 1, 1).FormulaA1 = monthlyBudgetCredit;

                    monthSummaryWorksheet.Cell(currentRow, 2).Value = $"Total spending for {financialGoal.Name}";
                    monthSummaryWorksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    monthSummaryWorksheet.Cell(currentRow + 1, 2).FormulaA1 = spendingDictionary[financialGoal.Name];


                    monthSummaryWorksheet.Cell(currentRow, 3).Value = $"Remaining spending for {financialGoal.Name}";
                    monthSummaryWorksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    monthSummaryWorksheet.Cell(currentRow + 1, 3).FormulaA1 = remainingDictionary[financialGoal.Name];
                    currentRow += 2;
                }

                currentRow += 2;

                monthSummaryWorksheet.Cell(currentRow, 1).Value = "Total budget for this month";
                monthSummaryWorksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(currentRow + 1, 1).FormulaA1 = monthlyBudgetRef;

                monthSummaryWorksheet.Cell(currentRow, 2).Value = "Total money spent this month";
                monthSummaryWorksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(currentRow + 1, 2).FormulaA1 = spendingDictionary["Total Outgoing"];

                monthSummaryWorksheet.Cell(currentRow, 3).Value = "Remaining spending for this month";
                monthSummaryWorksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(currentRow+1, 3).FormulaA1 = remainingDictionary["Total Outgoing"];

                currentRow += 2;

                monthSummaryWorksheet.Cell(currentRow, 1).Value = "Total income this month";
                monthSummaryWorksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                monthSummaryWorksheet.Cell(currentRow + 1, 1).FormulaA1 = spendingDictionary["Total Income"];

                currentRow += 3;

                monthSummaryWorksheet.Column(1).AdjustToContents();
                monthSummaryWorksheet.Column(2).AdjustToContents();
                monthSummaryWorksheet.Column(3).AdjustToContents();

                monthSummaryWorksheet.Cell(currentRow, 1).Value = "Go back to Table of Contents";
                monthSummaryWorksheet.Cell(currentRow, 1).SetHyperlink(new XLHyperlink($"'Table of Contents'!A1"));

                var col1 = monthSummaryWorksheet.Column(1);
                col1.Style.NumberFormat.Format = "$#,##0.00";

                var col2 = monthSummaryWorksheet.Column(2);
                col2.Style.NumberFormat.Format = "$#,##0.00";

                var col3 = monthSummaryWorksheet.Column(3);
                col3.Style.NumberFormat.Format = "$#,##0.00";

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

                var otherTitleRow = financialGoals.Count == 0 ? currentTitleRow : currentTitleRow += 2;
                var otherValueRow = otherTitleRow + 1;   
                var otherColLetter = IntToChar(otherColumn);
                weekWorksheet.Cell(otherTitleRow, summaryColumn).Value = "Money spent on other";
                weekWorksheet.Cell(otherValueRow, summaryColumn).FormulaA1 = $"_xlfn.SUM({otherColLetter}:{otherColLetter})";

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

        /// <summary>
        /// Converts a column number to Excel column letters (1=A, 2=B, 27=AA, etc.)
        /// </summary>
        /// <param name="columnNumber">Column number starting from 1</param>
        /// <returns>Excel column letter(s)</returns>
        private static string ColumnNumberToLetter(int columnNumber)
        {
            if (columnNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(columnNumber), "Column number must be greater than 0.");

            string result = string.Empty;

            while (columnNumber > 0)
            {
                columnNumber--; // Make it 0-based
                result = (char)('A' + (columnNumber % 26)) + result;
                columnNumber /= 26;
            }

            return result;
        }

        /// <summary>
        /// Converts row and column coordinates to Excel cell reference (e.g., 2,2 => B2)
        /// </summary>
        /// <param name="row">Row number starting from 1</param>
        /// <param name="column">Column number starting from 1</param>
        /// <returns>Excel cell reference (e.g., B2, AA100)</returns>
        private static string CoordinatesToCellReference(int row, int column)
        {
            if (row <= 0)
                throw new ArgumentOutOfRangeException(nameof(row), "Row number must be greater than 0.");

            return $"{ColumnNumberToLetter(column)}{row}";
        }
    }
}