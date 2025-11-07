using System.Threading.Tasks;

namespace IBudget.GUI.Services
{
    public interface IUpdateService
    {
        Task<UpdateInfo?> CheckForUpdatesAsync();
        Task DownloadUpdateAsync(UpdateInfo updateInfo);
        void ApplyUpdateAndRestart(UpdateInfo updateInfo);
        string GetCurrentVersion();
    }

    public class UpdateInfo
    {
        public string Version { get; set; } = string.Empty;
        public object NativeUpdateInfo { get; set; } = null!;
    }
}
