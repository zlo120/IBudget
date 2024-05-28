using IBudget.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IBudget.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDictionaryController : ControllerBase
    {
        private readonly IUserDictionaryService _userDictionaryService;

        public UserDictionaryController(IUserDictionaryService userDictionaryService)
        {
            _userDictionaryService = userDictionaryService;
        }

        [HttpPost("TagUntaggedRecords")]
        public async Task<IActionResult> TagUntaggedRecords()
        {
            // update existing mongodb records with these new records that now have tags
            throw new NotImplementedException();
        }
    }
}