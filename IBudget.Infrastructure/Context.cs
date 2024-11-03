using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IBudget.Infrastructure
{
    public class Context : DbContext
    {
        public DbSet<Income> Income { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Tag> Tags { get; set; }
        private readonly IConfiguration? _config;
        private readonly ILogger? _logger;

        public Context(IConfiguration configuration, ILogger<Context> logger)
        {
            _config = configuration;
            _logger = logger;
        }
        public Context() // empty constructor used when there is no configuration
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string stacksPath = Path.Combine(appDataPath, "Stacks");
            string stacksDbString = $"Data Source={stacksPath}\\Stacks.db";

            var connString = _config?.GetConnectionString("SQLite") ?? stacksDbString; // TO DO: add a secondary option if the config is null
            options.UseSqlite(connString);
            _logger?.LogInformation($"Starting connection to db at: {connString}");
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

            modelBuilder.Entity<Tag>().HasData(
                new Tag()
                {
                    ID = 1,
                    Name = "Ignored",
                    IsTracked = false
                }
            );
        }
    }
}