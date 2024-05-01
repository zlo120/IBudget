using ClosedXML.Excel;

namespace IBudget.Spreadsheet.Interfaces
{
    public interface IPopulator
    {
        Task<XLWorkbook> PopulateSpreadsheet(XLWorkbook workbook);
    }
}
