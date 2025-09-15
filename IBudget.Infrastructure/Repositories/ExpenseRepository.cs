using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;

namespace IBudget.Infrastructure.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly Context _context;
        public ExpenseRepository(Context context)
        {
            _context = context;
        }
        public async Task<bool> AddExpense(Expense expense)
        {
            try
            {
                var tags = expense.Tags!.ToArray();
                foreach (var tag in tags)
                {
                    var existingTag = _context.Tags.FirstOrDefault(t => t.Name == tag.Name);
                    if (existingTag is not null)
                    {
                        expense.Tags.Remove(tag);
                        expense.Tags.Add(existingTag);
                    }
                    else
                    {
                        _context.Tags.Add(tag);
                    }
                }

                _context.Expenses.Add(expense);

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.InnerException.Message.Contains("19"))
                    {
                        Console.WriteLine("Unique constraint has been triggered.");
                    }
                    else
                        Console.WriteLine($"{ex.Message} {ex.InnerException.Message}");
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine($"{ex.Message}");
                }

                return false;
            }
        }

        public async Task<bool> DeleteExpense(Expense expense)
        {
            _context.Expenses.Remove(expense);
            _context.SaveChanges();
            return true;
        }

        public Task<Expense> GetExpense(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Expense>> GetExpenseByWeek(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);
            return _context.Expenses.Where(e => e.Date >= startDate && e.Date <= endDate).ToList();
        }

        public async Task<List<Expense>> GetExpensesByMonth(int month)
        {
            var startDate = new DateTime(DateTime.Today.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return _context.Expenses.Where(e => e.Date >= startDate && e.Date <= endDate).ToList();
        }

        public async Task<bool> UpdateExpense(Expense expense)
        {
            _context.Expenses.Update(expense);
            _context.SaveChanges();
            return true;
        }
    }
}
