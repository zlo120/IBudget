using IBudget.Core.Enums;

namespace IBudget.Core.Interfaces
{
    public interface ISettingsService
    {
        string GetDbConnectionString();
        void SetDbConnectionString(string connectionString);
        void ResetDbConnectionString();
        void SetDatabaseType(DatabaseType databaseType);
        DatabaseType GetDatabaseType();
    }
}
