using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace IBudget.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly Context _context;
        public TagRepository(Context context)
        {
            _context = context;
        }
        public async Task<Tag> GetTag(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}