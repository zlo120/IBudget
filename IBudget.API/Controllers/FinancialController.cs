using IBudget.API.DTO;
using IBudget.API.Utils;
using IBudget.Core.Interfaces;
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

        [HttpGet("ReadYear")]
        public async Task<IActionResult> ReadYear()
        {
            var monthList = new List<MonthDTO>();
            for (int i = 1; i <= 12; i++)
            {
                var month = await _summaryService.ReadMonth(i);
                var monthDTO = DTOUtil.ConvertToDTO(month);
                monthList.Add(monthDTO);
            }

            return Ok(monthList);
        }
    }
}