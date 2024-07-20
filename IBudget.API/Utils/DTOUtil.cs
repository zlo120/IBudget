using IBudget.API.DTO;
using IBudget.Core.Model;

namespace IBudget.API.Utils
{
    public static class DTOUtil
    {
        public static MonthDTO ConvertToDTO(Month month)
        {
            var incomeDTOList = month.AllIncome.Select(income => ConvertToDTO(income)).ToList();
            var expenseDTOList = month.AllExpenses.Select(expense => ConvertToDTO(expense)).ToList();
            return new MonthDTO()
            {
                MonthName = month.MonthName,
                TotalIncome = incomeDTOList,
                TotalExpenses = expenseDTOList
            };
        }
        public static WeekDTO ConvertToDTO(Week week)
        {
            var incomeDTOList = week.Income.Select(income => ConvertToDTO(income)).ToList();
            var expenseDTOList = week.Expenses.Select(expense => ConvertToDTO(expense)).ToList();

            return new WeekDTO()
            {
                WeekLabel = week.Label,
                Start = DateOnly.FromDateTime(week.Start),
                End = DateOnly.FromDateTime(week.End),
                AllIncome = incomeDTOList,
                AllExpenses = expenseDTOList
            };
        }
        public static IncomeDTO ConvertToDTO(Income income)
        {
            return new IncomeDTO()
            {
                ID = income.ID,
                Amount = income.Amount,
                Date = DateOnly.FromDateTime(income.Date).ToString("dd/MM/yyyy"),
                Tags = income.Tags.Select(t => t.Name).ToList(),
                Description = income.Source
            };
        }
        public static ExpenseDTO ConvertToDTO(Expense expense)
        {
            return new ExpenseDTO()
            {
                ID = expense.ID,
                Amount = expense.Amount,
                Date = DateOnly.FromDateTime(expense.Date).ToString("dd/MM/yyyy"),
                Tags = expense.Tags.Select(t => t.Name).ToList(),
                Description = expense.Notes
            };
        }
    }
}