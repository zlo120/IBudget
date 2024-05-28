using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IBudget.Infrastructure
{
    public class Context : DbContext
    {
        public DbSet<Income> Income { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Tag> Tags { get; set; }
        private readonly IConfiguration _config;
        public Context(IConfiguration configuration)
        {
            _config = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(_config.GetConnectionString("SQLite"));
            options.UseLazyLoadingProxies();
            options.EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Income>()
                .Property(i => i.ID)
                .HasColumnName("ID")
                .IsRequired();

            modelBuilder.Entity<Expense>()
                .Property(e => e.ID)
                .HasColumnName("ID")
                .IsRequired();

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();
        }
    }
}