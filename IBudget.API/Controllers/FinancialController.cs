using API.DTO;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
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

        [HttpPost("AddIncome")]
        public async Task<IActionResult> AddIncome(IncomeDTO incomeDTO)
        {
            var income = new Income()
            {
                Amount = incomeDTO.Amount,
                Source = incomeDTO.Source,
                Date = incomeDTO.Date.ToDateTime(TimeOnly.Parse("12:00AM")),
                Frequency = incomeDTO.Frequency,
                Tags = new List<Tag>()
            };

            foreach(var tag in incomeDTO.Tags)
            {
                var tagObj = new Tag()
                {
                    Name = tag
                };

                income.Tags.Add(tagObj);
            }

            _incomeService.AddIncome(income);
            return Ok("Success");
        }

        [HttpPost("AddExpense")]
        public async Task<IActionResult> AddExpense(ExpenseDTO expenseDTO)
        {
            var expense = new Expense()
            {
                Amount = expenseDTO.Amount,
                Date = expenseDTO.Date.ToDateTime(TimeOnly.Parse("12:00AM")),
                Frequency = expenseDTO.Frequency,
                Notes = expenseDTO.Notes,
                Tags = new List<Tag>()
            };

            foreach (var tag in expenseDTO.Tags)
            {
                var tagObj = new Tag()
                {
                    Name = tag
                };

                expense.Tags.Add(tagObj);
            }

            _expenseService.AddExpense(expense);
            return Ok("Success");
        }

        [HttpGet("ReadWeek")]
        public async Task<IActionResult> ReadWeek(DateTime date)
        {
            var week = _summaryService.ReadWeek(date);
            return Ok(week);
        }
    }
}