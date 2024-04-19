using Core.Model;

namespace Core.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> GetTag(string name);
    }
}
