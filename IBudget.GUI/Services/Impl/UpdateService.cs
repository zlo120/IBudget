using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace IBudget.GUI.Services.Impl
{
    public class UpdateService : IUpdateService
    {
        private readonly UpdateManager? _updateManager;
        
        public UpdateService()
        {
            try
            {
                _updateManager = new UpdateManager(
                    new GithubSource("https://github.com/zlo120/IBudget", null, false)
                );
            }
            catch
            {
                _updateManager = null;
            }
        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            if (_updateManager is null)
                return null;

            try
            {
                var updateInfo = await _updateManager.CheckForUpdatesAsync();
                
                if (updateInfo == null)
                    return null;

                return new UpdateInfo
                {
                    Version = updateInfo.TargetFullRelease.Version.ToString(),
                    NativeUpdateInfo = updateInfo
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
                return null;
            }
        }

        public async Task DownloadUpdateAsync(UpdateInfo updateInfo)
        {
            if (_updateManager is null)
                return;

            if (updateInfo.NativeUpdateInfo is Velopack.UpdateInfo nativeInfo)
            {
                await _updateManager.DownloadUpdatesAsync(nativeInfo);
            }
        }

        public void ApplyUpdateAndRestart(UpdateInfo updateInfo)
        {
            if (_updateManager is null)
                return;
    
            if (updateInfo.NativeUpdateInfo is Velopack.UpdateInfo nativeInfo)
            {
                _updateManager.ApplyUpdatesAndRestart(nativeInfo);
            }
        }

        public string GetCurrentVersion()
        {
            try
            {
                return _updateManager?.CurrentVersion?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
