using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.RepositoryInterfaces
{
    public interface ITagsRepository
    {
        Task<List<Tag>> GetAll();
        Task<Tag> GetTagByName(string name);
        Task CreateTag(Tag tag);
        Task DeleteTagByName(string name);
        Task DeleteTagById(ObjectId id);
        /// <summary>
        /// Finds a list of tags based off the description parameter. It will first try to match any expense names, if not it will try to match any expense rules.
        /// </summary>
        /// <param name="description"></param>
        /// <returns>Returns a List of tags from the expense name/rule match.</returns>
        Task<List<Tag>> FindTagsByDescription(string description);
        Task UpdateTag(Tag tag);
        Task ClearCollection();
    }
}
