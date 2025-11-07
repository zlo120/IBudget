using System.Threading.Tasks;

namespace IBudget.GUI.Services
{
    public interface IPatchNotesService
    {
        /// <summary>
        /// Checks if patch notes should be shown for the current version
        /// </summary>
        Task<bool> ShouldShowPatchNotesAsync();
        
        /// <summary>
        /// Gets the patch notes content
        /// </summary>
        Task<string> GetPatchNotesAsync();
        
        /// <summary>
        /// Marks the current version's patch notes as shown
        /// </summary>
        Task MarkPatchNotesAsShownAsync();
        
        /// <summary>
        /// Gets the current application version
        /// </summary>
        string GetCurrentVersion();
    }
}
