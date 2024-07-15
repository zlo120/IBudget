using IBudget.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IBudget.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDictionaryController : ControllerBase
    {
        private readonly IUserDictionaryService _userDictionaryService;
        private readonly ITagService _tagService;

        public UserDictionaryController(IUserDictionaryService userDictionaryService, ITagService tagService)
        {
            _userDictionaryService = userDictionaryService;
            _tagService = tagService;
        }

        [HttpPost("TagUntaggedRecords")]
        public async Task<IActionResult> TagUntaggedRecords()
        {
            // update existing mongodb records with these new records that now have tags
            throw new NotImplementedException();
        }

        [HttpGet("GetAllTags")]
        public async Task<IActionResult> GetAllTags()
        {
            var tags = await _tagService.GetAll();
            var tagStrings = tags.Select(tag => tag.Name.ToString());
            return Ok(tagStrings);
        }
    }
}