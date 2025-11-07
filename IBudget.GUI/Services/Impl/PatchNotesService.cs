using System;
using System.IO;
using System.Threading.Tasks;

namespace IBudget.GUI.Services.Impl
{
    public class PatchNotesService : IPatchNotesService
    {
        private readonly IUpdateService _updateService;
        private const string PATCH_NOTES_FILE = "PATCHNOTES.md";
        private const string LAST_SHOWN_VERSION_FILE = "last_shown_version.txt";
        
        public PatchNotesService(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        public async Task<bool> ShouldShowPatchNotesAsync()
        {
            try
            {
                var currentVersion = GetCurrentVersion();
                
                // If version is unknown, don't show patch notes
                if (currentVersion == "Unknown")
                    return false;

                var lastShownVersion = await GetLastShownVersionAsync();
                
                // Show patch notes if:
                // 1. We've never shown patch notes before (lastShownVersion is null/empty)
                // 2. The current version is different from the last shown version
                return string.IsNullOrEmpty(lastShownVersion) || lastShownVersion != currentVersion;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking if patch notes should be shown: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetPatchNotesAsync()
        {
            try
            {
                var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var patchNotesPath = Path.Combine(appDirectory, PATCH_NOTES_FILE);

                if (!File.Exists(patchNotesPath))
                {
                    return $"# What's New in Version {GetCurrentVersion()}\n\nNo patch notes available for this version.";
                }

                var content = await File.ReadAllTextAsync(patchNotesPath);
                
                // If the file is empty, return a default message
                if (string.IsNullOrWhiteSpace(content))
                {
                    return $"# What's New in Version {GetCurrentVersion()}\n\nNo patch notes available for this version.";
                }

                return content;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading patch notes: {ex.Message}");
                return $"# What's New in Version {GetCurrentVersion()}\n\nError loading patch notes: {ex.Message}";
            }
        }

        public async Task MarkPatchNotesAsShownAsync()
        {
            try
            {
                var currentVersion = GetCurrentVersion();
                var appDataDirectory = GetAppDataDirectory();
                var versionFilePath = Path.Combine(appDataDirectory, LAST_SHOWN_VERSION_FILE);

                await File.WriteAllTextAsync(versionFilePath, currentVersion);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking patch notes as shown: {ex.Message}");
            }
        }

        public string GetCurrentVersion()
        {
            return _updateService.GetCurrentVersion();
        }

        private async Task<string?> GetLastShownVersionAsync()
        {
            try
            {
                var appDataDirectory = GetAppDataDirectory();
                var versionFilePath = Path.Combine(appDataDirectory, LAST_SHOWN_VERSION_FILE);

                if (!File.Exists(versionFilePath))
                    return null;

                var version = await File.ReadAllTextAsync(versionFilePath);
                return version?.Trim();
            }
            catch
            {
                return null;
            }
        }

        private string GetAppDataDirectory()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDirectory = Path.Combine(appDataPath, "Stacks");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            return appDirectory;
        }
    }
}
