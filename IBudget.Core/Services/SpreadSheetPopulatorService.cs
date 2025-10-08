using ClosedXML.Excel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Utils;
using IBudget.Spreadsheet.Interfaces;

namespace IBudget.Core.Services
{
    public class SpreadSheetPopulatorService : IPopulator
    {
        private readonly ISummaryService _summaryService;
        private readonly ITagService _tagService;

        public SpreadSheetPopulatorService(ISummaryService summaryService, ITagService tagService)
        {
            _summaryService = summaryService;
            _tagService = tagService;
        }
        private async Task<List<Tag>> GetAllTrackedTags()
        {
            return [.. (await _tagService.GetAll())
                .Where(tag => tag.IsTracked)
                .OrderBy(s => s.Name)
                .ToList()];
        }

        public async Task<XLWorkbook> PopulateSpreadsheet(XLWorkbook workbook)
        {
            var ignoredTag = await _tagService.GetTagByName("ignored");
            var trackedTagsList = await GetAllTrackedTags();
            var otherColumnIndex = trackedTagsList.Count + 1;
            var incomeColumnIndex = otherColumnIndex + 3;

            for (int worksheetNum = 1; worksheetNum <= workbook.Worksheets.Count; worksheetNum++)
            {
                var worksheet = workbook.Worksheet(worksheetNum);
                if (worksheet.Name == "Table of Contents" || worksheet.Name == "Monthly Budget") continue;

                // Ignore month sheets
                string lowerCaseName = worksheet.Name.ToLower();
                if (lowerCaseName == "january" || lowerCaseName == "february" || lowerCaseName == "march" ||
                     lowerCaseName == "april" || lowerCaseName == "may" || lowerCaseName == "june" ||
                     lowerCaseName == "july" || lowerCaseName == "august" || lowerCaseName == "september" ||
                     lowerCaseName == "october" || lowerCaseName == "november" || lowerCaseName == "december"
                   )
                {
                    continue;
                }

                // Find week data
                var date = Calendar.ParseWeekStartFromWeekRange(worksheet.Name);
                var week = await _summaryService.ReadWeek(date);
                if (week.Income.Count == 0 && week.Expenses.Count == 0) continue;
                var remainingExpenses = new List<Expense>();
                var weeklyIncome = week.Income.Where(income => !income.Tags.Contains(ignoredTag)).ToList();
                // populate the income
                var incomeQueue = new Queue<Income>(weeklyIncome);
                PopulateColumn(new Queue<FinancialRecord>(incomeQueue.ToList()), ref worksheet, incomeColumnIndex, true);

                // populate the expenses that fall into the "other" category
                var otherExpenses = new Queue<FinancialRecord>(week.Expenses
                    .Where(expense => (expense.Tags!.Count == 0 || expense.Tags.All(tag => !tag.IsTracked)) && !expense.Tags.Contains(ignoredTag))
                    .ToList()
                );

                remainingExpenses = [..week.Expenses
                    .Where(expense => !otherExpenses.Contains(expense) && !expense.Tags!.Contains(ignoredTag))];

                PopulateColumn(otherExpenses, ref worksheet, otherColumnIndex, true);

                // populate the expenses that fall into the tracked tags
                for (var columnCounter = 1; columnCounter <= trackedTagsList.Count; columnCounter++)
                {
                    var tagName = trackedTagsList[columnCounter - 1].Name;
                    // using all the data that exists in the db for this week and for this category
                    //   populate this column
                    var remainingExpenseInColumn = new Queue<FinancialRecord>(
                        [..remainingExpenses
                            .Where(e => e.Tags!.Any(t => t.Name.Contains(tagName, StringComparison.CurrentCultureIgnoreCase)))]
                    );

                    remainingExpenses.RemoveAll(e => remainingExpenseInColumn.Contains(e));

                    PopulateColumn(remainingExpenseInColumn, ref worksheet, columnCounter);

                    for (int i = 1; i <= trackedTagsList.Count; i++)
                    {
                        if (trackedTagsList[i - 1].Name.Length > 10)
                        {
                            worksheet.Column(i).AdjustToContents();
                        }
                        var column = worksheet.Column(i);
                        column.Style.NumberFormat.Format = "$#,##0.00";
                    }

                    var incomeColumn = trackedTagsList.Count + 4;
                    worksheet.Column(incomeColumn).AdjustToContents();
                    worksheet.Column(incomeColumn + 1).AdjustToContents();
                }
            }

            return workbook;
        }

        private void PopulateColumn(Queue<FinancialRecord> remainingDataRecords, ref IXLWorksheet worksheet, int columnNum, bool writeDescription = false)
        {
            var rowCounter = 3;
            while (remainingDataRecords.Any())
            {
                var record = remainingDataRecords.Dequeue();
                var cell = worksheet.Cell(rowCounter, columnNum);
                if (!cell.IsEmpty())
                {
                    rowCounter += 1;
                    continue;
                }

                cell.Value = Math.Abs((decimal)record?.Amount!);
                worksheet.Column(columnNum).Style.NumberFormat.Format = "$#,##0.00";

                if (writeDescription)
                {
                    var descriptionColumn = columnNum + 1;
                    var descriptionCell = worksheet.Cell(rowCounter, descriptionColumn);
                    var description = "";
                    foreach (var tag in record.Tags!)
                    {
                        description += tag.Name + " ";
                    }
                    descriptionCell.Value = description;
                }

                rowCounter += 1;
            }
        }
    }
}