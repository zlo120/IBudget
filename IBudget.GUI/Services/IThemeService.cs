using CommunityToolkit.Mvvm.Input;

namespace IBudget.GUI.Services
{
    public interface IThemeService
    {
        bool IsDarkMode { get; }
        string ThemeIcon { get; }
        string ThemeLabel { get; }
        IRelayCommand ToggleCommand { get; }
        void Toggle();
    }
}