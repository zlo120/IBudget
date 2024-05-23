using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace IBudget.Infrastructure
{
    public class MongoDBContext : DbContext
    {
        public DbSet<ExpenseDictionary> ExpenseDictionaries { get; set; }
        public static MongoDBContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<MongoDBContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);

        public MongoDBContext(DbContextOptions<MongoDBContext> options) : base(options)
        {
        }

        public MongoDBContext(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
