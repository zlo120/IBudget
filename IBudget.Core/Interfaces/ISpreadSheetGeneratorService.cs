namespace IBudget.Spreadsheet.Interfaces
{
    public interface ISpreadSheetGeneratorService
    {
        Task<string> GenerateSpreadsheet();
    }
}
