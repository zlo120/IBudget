using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> GetTag(string name);
    }
}
