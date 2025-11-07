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
        public async Task CheckForUpdatesAsync()
        {
            if (_updateManager is null) return;

            try
            {
                var updateInfo = await _updateManager.CheckForUpdatesAsync();
                if (updateInfo is not null)
                {
                    // Download and apply update
                    await _updateManager.DownloadUpdatesAsync(updateInfo);

                    // Notify user and restart
                    _updateManager.ApplyUpdatesAndRestart(updateInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
            }
        }
    }
}
