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
                Amount = income.Amount,
                Source = income.Source,
                Date = DateOnly.FromDateTime(income.Date),
                Frequency = income.Frequency,
                Tags = income.Tags.Select(t => t.Name).ToList()
            };
        }        
        public static ExpenseDTO ConvertToDTO(Expense expense)
        {
            return new ExpenseDTO()
            {
                Amount = expense.Amount,
                Date = DateOnly.FromDateTime(expense.Date),
                Frequency = expense.Frequency,
                Notes = expense.Notes,
                Tags = expense.Tags.Select(t => t.Name).ToList()
            };
        }
    }
}