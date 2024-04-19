using Core.Interfaces;
using Core.Model;

namespace Infrastructure.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {
        private readonly Context _context;
        public IncomeRepository(Context context)
        {
            _context = context;
        }

        public async Task<bool> AddIncome(Income income)
        {
            try
            {
                var tags = income.Tags.ToArray();
                foreach (var tag in tags)
                {
                    var existingTag = _context.Tags.FirstOrDefault(t => t.Name == tag.Name);
                    if (existingTag is not null)
                    {
                        income.Tags.Remove(tag);
                        income.Tags.Add(existingTag);
                    }
                    else
                    {
                        _context.Tags.Add(tag);
                    }
                }

                _context.Income.Add(income);

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

                Console.ReadKey();
                return false;
            }
        }

        public async Task<bool> DeleteIncome(Income income)
        {
            _context.Income.Remove(income);
            _context.SaveChanges();
            return true;
        }

        public Task<Income> GetIncome(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Income>> GetIncomeByMonth(int month)
        {
            var startDate = new DateTime(DateTime.Today.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return _context.Income.Where(i => i.Date >= startDate && i.Date <= endDate).ToList();
        }

        public async Task<List<Income>> GetIncomeByWeek(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);
            return _context.Income.Where(i => i.Date >= startDate && i.Date <= endDate).ToList();
        }

        public async Task<bool> UpdateIncome(Income income)
        {
            _context.Income.Update(income);
            _context.SaveChanges();
            return true;
        }
    }
}
