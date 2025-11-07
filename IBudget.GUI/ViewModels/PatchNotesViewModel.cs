using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IBudget.GUI.Services;
using System.Threading.Tasks;

namespace IBudget.GUI.ViewModels
{
    public partial class PatchNotesViewModel : ViewModelBase
    {
        private readonly IPatchNotesService _patchNotesService;

        [ObservableProperty]
        private string _version = string.Empty;

        [ObservableProperty]
        private string _patchNotesContent = string.Empty;

        [ObservableProperty]
        private bool _isLoading = true;

        public PatchNotesViewModel(IPatchNotesService patchNotesService)
        {
            _patchNotesService = patchNotesService;
        }

        public async Task LoadPatchNotesAsync()
        {
            IsLoading = true;
            
            Version = _patchNotesService.GetCurrentVersion();
            PatchNotesContent = await _patchNotesService.GetPatchNotesAsync();
            
            IsLoading = false;
        }

        [RelayCommand]
        private async Task Close()
        {
            // Mark patch notes as shown before closing
            await _patchNotesService.MarkPatchNotesAsShownAsync();
        }
    }
}
