using ClosedXML.Excel;
using Core.Utils;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using IronXL;
using IronXL.Drawing.Charts;

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

            foreach(var month in calendar.Months)
            {
                var monthSummaryWorksheet = workbook.Worksheets.Add(month.MonthName);

                // adding this month to the table of contents
                tableOfContentsWS.Cell(nextFreeCell, 1).Value = month.MonthName;
                tableOfContentsWS.Cell(nextFreeCell, 1).SetHyperlink(new XLHyperlink($"'{month.MonthName}'!A1"));

                nextFreeCell++;

                foreach (var week in month.Weeks)
                {
                    var weekWorksheet = workbook.Worksheets.Add(week.Label);
                    tableOfContentsWS.Cell(nextFreeCell, 1).Value = week.Label;
                    tableOfContentsWS.Cell(nextFreeCell, 1).SetHyperlink(new XLHyperlink($"'{week.Label}'!A1"));
                    nextFreeCell++;

                    weekWorksheet.Cell(1,1).Value = week.Label;
                    weekWorksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    weekWorksheet.Cell(1, 1).Style.Font.Bold = true;

                    weekWorksheet.Range(
                            weekWorksheet.Cell(1, 1),
                            weekWorksheet.Cell(1, 5)
                        ).Merge();

                    weekWorksheet.Cell(1,6).Value = "Go back to Table of Contents";
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

                    for(int i = 1; i <= 5; i++)
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

            WorkBook wb = WorkBook.Load(path);
            int counter = 0;

            // array of all month names
            string[] monthNames = calendar.Months.Select(month => month.MonthName).ToArray();

            string[] weekLabels = calendar.Months.SelectMany(month => month.Weeks.Select(week => week.Label)).ToArray();


            while (wb.WorkSheets.Count > counter)
            {
                var workSheet = wb.WorkSheets[counter];
                if (monthNames.Contains(workSheet.Name))
                {
                    // this worksheet is a month summary

                }
                else if (weekLabels.Contains(workSheet.Name))
                {
                    Console.WriteLine($"Plotting graph for sheet {workSheet.Name}");
                    // this worksheet is a week summary
                    string xAxis = "K4:L4";
                    string yAxis = "K5:L5";

                    var chart = workSheet.CreateChart(ChartType.Bar, 5, 5, 20, 20);

                    var series = chart.AddSeries(yAxis, xAxis);

                    chart.SetTitle("Financial Summary");
                    chart.SetLegendPosition(LegendPosition.Bottom);
                    chart.Plot();
                }
                counter++;
            }


            Console.WriteLine($"Your excel spreadsheet is ready to open at \"{path}\"");
            Console.ReadKey();
            return;

        }
    }
}