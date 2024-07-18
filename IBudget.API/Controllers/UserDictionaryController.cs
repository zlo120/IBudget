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

        [HttpGet("GetAllTags")]
        public async Task<IActionResult> GetAllTags()
        {
            var tags = await _tagService.GetAll();
            var tagStrings = tags.Select(tag => tag.Name.ToString());
            return Ok(tagStrings);
        }

        [HttpPost("CreateTag")]
        public async Task<IActionResult> CreateTag([FromBody] string tagName)
        {
            await _tagService.CreateTag(tagName);
            return Ok(new
            {
                Success = $"Tag {tagName} was created successfully!"
            });
        }

        [HttpPost("DeleteTag")]
        public async Task<IActionResult> DeleteTag([FromBody] string tagName)
        {
            await _tagService.DeleteTag(tagName);
            return Ok(new
            {
                Success = $"Tag {tagName} was deleted successfully!"
            });
        }
    }
}