using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ITagService
    {
        Task<Tag> GetTag(string name);
    }
}
