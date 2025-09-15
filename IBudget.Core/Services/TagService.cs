using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;

namespace IBudget.Core.Services
{
    public class TagService : ITagService
    {
        private readonly ITagsRepository _tagRepository;
        public TagService(ITagsRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task CreateTag(Tag tag)
        {
            await _tagRepository.CreateTag(tag);
        }

        public async Task DeleteTagByName(string name)
        {
            await _tagRepository.DeleteTagByName(name);
        }

        public async Task DeleteTagById(ObjectId id)
        {
            await _tagRepository.DeleteTagById(id);
        }

        public async Task<List<string>> FindTagsByDescription(string description)
        {
            return await _tagRepository.FindTagsByDescription(description);
        }

        public async Task<List<Tag>> GetAll()
        {
            return await _tagRepository.GetAll();
        }

        public async Task<Tag> GetTagByName(string name)
        {
            return await _tagRepository.GetTagByName(name);
        }

        public async Task UpdateTag(Tag tag)
        {
            await _tagRepository.UpdateTag(tag);
        }
    }
}
