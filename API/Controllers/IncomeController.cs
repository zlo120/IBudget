using API.DTO;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;
        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        [HttpPost("AddIncome")]
        public async Task<IActionResult> AddIncome(IncomeDTO incomeDTO)
        {
            //_incomeService.AddIncome();
            return Ok();
        }
    }
}
