using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IBudget.Core.Interfaces;

namespace IBudget.Core.Services
{
    public class BatchHashService(IIncomeService incomeService, IExpenseService expenseService) : IBatchHashService
    {
        private readonly IIncomeService _incomeService = incomeService;
        private readonly IExpenseService _expenseService = expenseService;

        public string ComputeBatchHash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = SHA256.HashData(bytes);
            StringBuilder hashStringBuilder = new StringBuilder();
            foreach (var b in hashBytes)
            {
                hashStringBuilder.Append(b.ToString("x2"));
            }
            return hashStringBuilder.ToString();
        }

        public async Task<bool> DoesBatchHashExist(string hash)
        {
            var incomeTask = _incomeService.DoesBatchHashExist(hash);
            var expenseTask = _expenseService.DoesBatchHashExist(hash);

            var tasks = new[] { incomeTask, expenseTask };

            while (tasks.Length > 0)
            {
                var completed = await Task.WhenAny(tasks);
                if (await completed)
                    return true;

                tasks = tasks.Where(t => t != completed).ToArray();
            }

            return false;
        }
    }
}
