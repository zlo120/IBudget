using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace IBudget.Infrastructure
{
    public class MongoDBEFContext : DbContext
    {
        public DbSet<UserDictionary> userExpenseDictionaries { get; set; }
        public static MongoDBEFContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<MongoDBEFContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);

        public MongoDBEFContext(DbContextOptions<MongoDBEFContext> options) : base(options)
        {
        }

        public MongoDBEFContext(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}