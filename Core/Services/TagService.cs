using Core.Interfaces;
using Core.Model;

namespace Core.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<Tag> GetTag(string name)
        {
            return await _tagRepository.GetTag(name);
        }
    }
}
