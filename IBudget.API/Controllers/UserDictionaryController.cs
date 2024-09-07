using IBudget.API.DTO;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
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
            var tags = (await _tagService.GetAll()).Select(t => new TagDTO()
            {
                TagName = t.Name,
                IsTracked = t.IsTracked
            }).Reverse().ToList();
            return Ok(tags);
        }

        [HttpPost("CreateTag")]
        public async Task<IActionResult> CreateTag([FromBody] TagDTO tagDto)
        {
            var tag = new Tag()
            {
                Name = tagDto.TagName,
                IsTracked = tagDto.IsTracked
            };

            await _tagService.CreateTag(tag);
            return Ok(new
            {
                Success = $"Tag {tag.Name} was created successfully!"
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