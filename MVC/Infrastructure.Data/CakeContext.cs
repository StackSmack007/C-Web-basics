namespace Infrastructure.Data
{
    using Infrastructure.Data.Configurations;
    using Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;

    public class CakeContext : DbContext
    {
       public DbSet<User> Users {get;set;}
       public DbSet<Order> Orders {get;set;}
       public DbSet<OrderProduct> OrdersProducts {get;set;} 
       public DbSet<Product> Products {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigDatabase.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderProductsConfiguration());
        }
    }
}