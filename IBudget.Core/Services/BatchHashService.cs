using System.Security.Cryptography;
using System.Text;
using IBudget.Core.Interfaces;

namespace IBudget.Core.Services
{
    public class BatchHashService : IBatchHashService
    {
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
    }
}
