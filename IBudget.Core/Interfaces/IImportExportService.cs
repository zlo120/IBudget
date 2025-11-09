namespace IBudget.Core.Interfaces
{
    public interface IImportExportService
    {
        Task<string> ExportData();
        Task ImportData(string filePath);
    }
}
