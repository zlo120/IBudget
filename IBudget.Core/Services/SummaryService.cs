using IBudget.Core.DTO;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Core.Utils;

namespace IBudget.Core.Services
{
    public class SummaryService(IIncomeRepository incomeRepository, IExpenseRepository expenseRepository) : ISummaryService
    {
        private readonly IIncomeRepository _incomeRepository = incomeRepository;
        private readonly IExpenseRepository _expenseRepository = expenseRepository;

        private (List<Income>, List<Expense>) ApplyOffset(List<Income> incomes, List<Expense> expenses)
        {
            var taggedIncomes = new Dictionary<string, (List<Income> Records, double Total)>();
            var taggedExpenses = new Dictionary<string, (List<Expense> Records, double Total)>();

            foreach (var income in incomes)
            {
                foreach (var tag in income.Tags)
                {
                    if (!taggedIncomes.ContainsKey(tag.Name))
                    {
                        taggedIncomes[tag.Name] = (new List<Income>(), 0);
                    }
                    taggedIncomes[tag.Name] = (
                        taggedIncomes[tag.Name].Records.Concat(new[] { income }).ToList(),
                        taggedIncomes[tag.Name].Total + income.Amount
                    );
                }
            }

            foreach (var expense in expenses)
            {
                foreach (var tag in expense.Tags)
                {
                    if (!taggedExpenses.ContainsKey(tag.Name))
                    {
                        taggedExpenses[tag.Name] = (new List<Expense>(), 0);
                    }
                    taggedExpenses[tag.Name] = (
                        taggedExpenses[tag.Name].Records.Concat(new[] { expense }).ToList(),
                        taggedExpenses[tag.Name].Total + expense.Amount
                    );
                }
            }

            var allTags = taggedIncomes.Keys.Union(taggedExpenses.Keys).Distinct();
            var offsetIncomes = new List<Income>();
            var offsetExpenses = new List<Expense>();

            foreach (var tagName in allTags)
            {
                var totalIncome = taggedIncomes.ContainsKey(tagName) ? taggedIncomes[tagName].Total : 0;
                var totalExpense = taggedExpenses.ContainsKey(tagName) ? taggedExpenses[tagName].Total : 0;

                var incomeAmount = Math.Max(0, totalIncome - totalExpense);
                var expenseAmount = Math.Max(0, totalExpense - totalIncome);

                if (incomeAmount > 0 && taggedIncomes.ContainsKey(tagName))
                {
                    var records = taggedIncomes[tagName].Records;
                    var scale = incomeAmount / totalIncome;

                    foreach (var record in records)
                    {
                        var scaledIncome = new Income
                        {
                            Id = record.Id,
                            Date = record.Date,
                            Amount = record.Amount * scale,
                            Frequency = record.Frequency,
                            Tags = record.Tags,
                            BatchHash = record.BatchHash,
                            CreatedAt = record.CreatedAt,
                            IsIgnored = record.IsIgnored,
                            Source = record.Source
                        };
                        offsetIncomes.Add(scaledIncome);
                    }
                }

                if (expenseAmount > 0 && taggedExpenses.ContainsKey(tagName))
                {
                    var records = taggedExpenses[tagName].Records;
                    var scale = expenseAmount / totalExpense;

                    foreach (var record in records)
                    {
                        var scaledExpense = new Expense
                        {
                            Id = record.Id,
                            Date = record.Date,
                            Amount = record.Amount * scale,
                            Frequency = record.Frequency,
                            Tags = record.Tags,
                            BatchHash = record.BatchHash,
                            CreatedAt = record.CreatedAt,
                            IsIgnored = record.IsIgnored,
                            Notes = record.Notes
                        };
                        offsetExpenses.Add(scaledExpense);
                    }
                }
            }

            return (offsetIncomes.DistinctBy(i => i.Id).ToList(), offsetExpenses.DistinctBy(e => e.Id).ToList());
        }

        public async Task<MonthDTO> ReadMonth(int month, bool getOffset = false)
        {
            var incomes = (await _incomeRepository.GetIncomeByMonth(month)).Where(i => !i.IsIgnored).ToList();
            var expenses = (await _expenseRepository.GetExpensesByMonth(month)).Where(e => !e.IsIgnored).ToList();

            var (offsetIncomes, offsetExpenses) = getOffset ? ApplyOffset(incomes, expenses) : (incomes, expenses);

            return new MonthDTO(month)
            {
                AllIncome = offsetIncomes,
                AllExpenses = offsetExpenses
            };
        }

        public async Task<WeekDTO> ReadWeek(DateTime date, bool getOffset = false)
        {
            var weekRange = Calendar.GetWeekRange(date);
            var startOfWeek = weekRange[0];
            var endOfWeek = weekRange[1];
            var label = Calendar.GetWeekLabel(date);
            var incomes = await _incomeRepository.GetIncomeByWeek(startOfWeek);
            var expenses = await _expenseRepository.GetExpenseByWeek(startOfWeek);

            var (offsetIncomes, offsetExpenses) = getOffset ? ApplyOffset(incomes, expenses) : (incomes, expenses);

            return new WeekDTO(startOfWeek, endOfWeek, label)
            {
                Income = offsetIncomes,
                Expenses = offsetExpenses
            };
        }
    }
}
