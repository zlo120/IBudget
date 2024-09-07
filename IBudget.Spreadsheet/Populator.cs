using ClosedXML.Excel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Utils;
using IBudget.Spreadsheet.Interfaces;

namespace IBudget.Spreadsheet
{
    public class Populator : IPopulator
    {
        private readonly ISummaryService _summaryService;
        private readonly ITagService _tagService;
        private const int OTHER_COLUMN = 7;
        private const int INCOME_COLUMN = 10;

        public Populator(ISummaryService summaryService, ITagService tagService)
        {
            _summaryService = summaryService;
            _tagService = tagService;
        }
        public async Task<XLWorkbook> PopulateSpreadsheet(XLWorkbook workbook)
        {
            for (int worksheetNum = 1; worksheetNum <= workbook.Worksheets.Count; worksheetNum++)
            {
                var worksheet = workbook.Worksheet(worksheetNum);
                if (worksheet.Name == "Table of Contents" || worksheet.Name == "Monthly Budget") continue;

                // Month sheet
                string lowerCaseName = worksheet.Name.ToLower();
                if (lowerCaseName == "january" || lowerCaseName == "february" || lowerCaseName == "march" ||
                    lowerCaseName == "april" || lowerCaseName == "may" || lowerCaseName == "june" ||
                    lowerCaseName == "july" || lowerCaseName == "august" || lowerCaseName == "september" ||
                    lowerCaseName == "october" || lowerCaseName == "november" || lowerCaseName == "december")
                {
                    continue;
                }

                // Find week data
                var date = Calendar.ParseWeekStartFromWeekRange(worksheet.Name);
                var week = await _summaryService.ReadWeek(date);
                if (week.Income.Count == 0 && week.Expenses.Count == 0) continue;
                var remainingExpenses = new List<Expense>(week.Expenses);

                // get categories to hold all the tracked tags
                var categories = (await _tagService.GetAll())
                    .Where(tag => tag.IsTracked)
                    .Select(tag => tag.Name)
                    .ToArray();
                //var categories = new string[] { "Petrol", "Fitness", "Bills", "Public transport", "Food", "Alcohol" }; // replace with tracked tags

                // process the income
                var incomeQueue = new Queue<Income>(week.Income);
                PopulateColumn(new Queue<DataEntry>(incomeQueue.ToList()), ref worksheet, INCOME_COLUMN);

                // process the expenses that fall into the other category
                var otherExpenses = new Queue<DataEntry>(
                    remainingExpenses
                        .Where(expense => expense.Tags!.Count == 0 || expense.Tags
                            .Any(tag =>
                                !tag.IsTracked))
                        .ToList()
                    );

                PopulateColumn(otherExpenses, ref worksheet, OTHER_COLUMN);

                // process the expenses that fall into the set categories
                for (var columnCounter = 1; columnCounter <= categories.Length; columnCounter++)
                {
                    // using all the data that exists in the db for this week and for this category
                    //   populate this column
                    var remainingExpenseInColumn = new Queue<DataEntry>(
                        remainingExpenses
                            .Where(e => e.Tags
                            .Any(t => t.Name.ToLower().Contains(categories[columnCounter - 1].ToLower())))
                            .ToList()
                    );

                    PopulateColumn(remainingExpenseInColumn, ref worksheet, columnCounter);

                    for (int i = 1; i <= categories.Length; i++)
                    {
                        if (categories[i-1].Length > 10)
                        {
                            worksheet.Column(i).AdjustToContents();                            
                        }
                        var column = worksheet.Column(i);
                        column.Style.NumberFormat.Format = "$#,##0.00";
                    }

                    var incomeColumn = categories.Length + 4;
                    worksheet.Column(incomeColumn).AdjustToContents();
                    worksheet.Column(incomeColumn + 1).AdjustToContents();
                }
            }

            return workbook;
        }

        private static void PopulateColumn(Queue<DataEntry> remainingDataRecords, ref IXLWorksheet worksheet, int columnNum)
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

                cell.Value = Math.Abs((decimal) record?.Amount!);
                worksheet.Column(columnNum).Style.NumberFormat.Format = "$#,##0.00";

                // write the description
                if (OTHER_COLUMN == columnNum || INCOME_COLUMN == columnNum)
                {
                    var descriptionColumn = columnNum + 1;
                    var descriptionCell = worksheet.Cell(rowCounter, descriptionColumn);
                    var description = "";
                    foreach(var tag in record.Tags)
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