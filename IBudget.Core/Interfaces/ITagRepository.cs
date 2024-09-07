using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAll();
        Task<Tag> GetTag(string name);
        Task CreateTag(Tag tag);
        Task DeleteTag(string name);
        Task<List<string>> FindTagByDescription(string description);
    }
}
