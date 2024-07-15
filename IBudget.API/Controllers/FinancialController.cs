using IBudget.API.DTO;
using IBudget.API.Utils;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace IBudget.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialController : ControllerBase
    {
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;
        private readonly ISummaryService _summaryService;

        public FinancialController(IIncomeService incomeService, IExpenseService expenseService, ISummaryService summaryService)
        {
            _incomeService = incomeService;
            _expenseService = expenseService;
            _summaryService = summaryService;
        }

        [HttpGet("ReadWeek")]
        public async Task<IActionResult> ReadWeek(DateTime date)
        {
            var week = await _summaryService.ReadWeek(date);
            var weekDTO = DTOUtil.ConvertToDTO(week);
            return Ok(weekDTO);
        }

        [HttpGet("ReadMonth")]
        public async Task<IActionResult> ReadMonth(CalendarEnum monthEnum)
        {
            var month = await _summaryService.ReadMonth((int)monthEnum);
            var monthDTO = DTOUtil.ConvertToDTO(month);
            return Ok(monthDTO);
        }
    }
}