namespace Panda.Infrastructure.Data
{
    using Microsoft.EntityFrameworkCore;
    using Panda.Indfrastructure.Models.Models;
    using Panda.Infrastructure.Data.ContextConfig;

    public class PandaContext : DbContext
    {
        public PandaContext()
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Package> Packages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configurations.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Receipt>().HasOne(r => r.Package).WithOne(p => p.Receipt);
        }

    }
}