namespace Chushka.Data
{
    using Chushka.Data.ContextConfig;
    using Chushka.Models.Models;
    using Microsoft.EntityFrameworkCore;
  public  class ChushkaContext : DbContext
    {
        public ChushkaContext()
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configurations.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}