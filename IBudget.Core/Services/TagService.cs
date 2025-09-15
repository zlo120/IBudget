using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;

namespace IBudget.Core.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task CreateTag(Tag tag)
        {
            await _tagRepository.CreateTag(tag);
        }

        public async Task DeleteTag(string name)
        {
            await _tagRepository.DeleteTag(name);
        }

        public async Task<List<string>> FindTagByDescription(string description)
        {
            return await _tagRepository.FindTagByDescription(description);
        }

        public async Task<List<Tag>> GetAll()
        {
            return await _tagRepository.GetAll();
        }

        public async Task<Tag> GetTag(string name)
        {
            return await _tagRepository.GetTag(name);
        }

        public async Task UpdateTag(Tag tag)
        {
            await _tagRepository.UpdateTag(tag);
        }
    }
}
