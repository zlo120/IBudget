using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace IBudget.Infrastructure.Repositories
{
    public class SummaryRepository : ISummaryRepository
    {
        private readonly Context _context;
        public SummaryRepository(Context context)
        {
            _context = context;
        }
        public async Task<Week> ReadWeek(DateTime date)
        {
            var weekRange = Calendar.GetWeekRange(date);
            var week = new Week(weekRange[0], weekRange[1], Calendar.GetWeekLabel(date));
            var weeks = await _context.Income.ToListAsync();
            var incomeThisWeek = await _context.Income.Where(i => i.Date > week.Start && i.Date < week.End).ToListAsync();
            var expenseThisWeek = await _context.Expenses.Where(e => e.Date > week.Start && e.Date < week.End).ToListAsync();

            week.Income = incomeThisWeek;
            week.Expenses = expenseThisWeek;

            return week;
        }
        public async Task<Month> ReadMonth(int month)
        {
            var monthObj = new Month(month);
            for (int i = 0; i < monthObj.Weeks.Count; i++)
            {
                var week = monthObj.Weeks[i];
                monthObj.Weeks[i] = await ReadWeek(week.Start);
            }

            return monthObj;
        }
    }
}