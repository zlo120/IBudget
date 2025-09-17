using System.Threading.Tasks;

namespace IBudget.GUI.Services
{
    public interface IMessageService
    {
        Task ShowMessageAsync(string title, string message);
        Task ShowErrorAsync(string message);
        Task ShowSuccessAsync(string message);
        Task<bool> ShowConfirmationAsync(string title, string message);
    }
}
