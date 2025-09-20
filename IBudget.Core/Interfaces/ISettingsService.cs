namespace IBudget.Core.Interfaces
{
    public interface ISettingsService
    {
        string GetDbConnectionString();
        void SetDbConnectionString(string connectionString);
    }
}
