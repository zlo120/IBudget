using Core.Model;

namespace Core.Interfaces
{
    public interface ITagService
    {
        Task<Tag> GetTag(string name);
    }
}
