namespace IBudget.Core.RepositoryInterfaces
{
    public interface IImportExportRepository
    {
        Task<string> ExportData();
        Task ImportData(string filePath);
    }
}
