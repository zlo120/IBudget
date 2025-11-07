using System;
using System.IO;
using System.Threading.Tasks;

namespace IBudget.GUI.Services.Impl
{
    /// <summary>
    /// A test version of PatchNotesService that allows testing without Velopack deployments.
    /// To use: Replace PatchNotesService with TestPatchNotesService in ServiceCollectionExtensions.cs
    /// </summary>
    public class TestPatchNotesService : IPatchNotesService
    {
        private readonly IUpdateService _updateService;
        private const string PATCH_NOTES_FILE = "PATCHNOTES.md";
        private const string LAST_SHOWN_VERSION_FILE = "last_shown_version.txt";
        
        // Override this to simulate different versions for testing
        private const string TEST_VERSION = "1.0.0-test";
        
        public TestPatchNotesService(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        public async Task<bool> ShouldShowPatchNotesAsync()
        {
            try
            {
                var currentVersion = GetCurrentVersion();
                var lastShownVersion = await GetLastShownVersionAsync();
                
                // For testing: always show if last shown version is different or doesn't exist
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
            // For testing: return a hardcoded version instead of relying on Velopack
            return TEST_VERSION;
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
            
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            return appDirectory;
        }
    }
}
