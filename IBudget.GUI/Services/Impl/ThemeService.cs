using Avalonia;
using Avalonia.Platform;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using IBudget.Core.Interfaces;

namespace IBudget.GUI.Services.Impl
{
    public partial class ThemeService : ObservableObject, IThemeService
    {
        private const string SettingsKey = "theme";
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private bool _isDarkMode;

        public ThemeService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            // Load saved preference, fall back to system
            var saved = _settingsService.GetTheme();
            IsDarkMode = saved switch
            {
                "dark"  => true,
                "light" => false,
                _        => IsSystemDark()
            };
            Apply();
        }

        public void Toggle()
        {
            IsDarkMode = !IsDarkMode;
            Apply();
            _settingsService.SetTheme(IsDarkMode ? "dark" : "light");
        }

        private void Apply()
        {
            Application.Current!.RequestedThemeVariant =
                IsDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
        }

        private static bool IsSystemDark()
        {
            // Reads the OS preference via Avalonia's platform API
            var variant = Application.Current?.PlatformSettings?
                .GetColorValues().ThemeVariant;
            return variant == PlatformThemeVariant.Dark;
        }
    }
}