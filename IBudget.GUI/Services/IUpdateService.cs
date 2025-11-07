using System.Threading.Tasks;

namespace IBudget.GUI.Services
{
    public interface IUpdateService
    {
        Task CheckForUpdatesAsync();
    }
}
