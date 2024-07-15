using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ITagService
    {
        Task<List<Tag>> GetAll();
        Task<Tag> GetTag(string name);
    }
}
