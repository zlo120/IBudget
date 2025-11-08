using IBudget.Core.Interfaces;
using IBudget.Core.RepositoryInterfaces;

namespace IBudget.Core.Services
{
    public class ImportExportService(IImportExportRepository importExportRepository) : IImportExportService
    {
        private readonly IImportExportRepository _importExportRepository = importExportRepository;

        public async Task<string> ExportData()
        {
            return await _importExportRepository.ExportData();
        }
    }
}
