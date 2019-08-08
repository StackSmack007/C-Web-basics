namespace Airport.Infrastructure.Data
{
    using Airport.Infrastructure.Data.Settings;
    using Airport.Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    public class AirportContext : DbContext
    {
        public AirportContext()
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Seat> Seats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Seat>().HasKey(t => new { t.FlightId, t.Class });
            modelBuilder.Entity<Seat>().HasOne(t => t.Flight).WithMany(f => f.FlightSeats).HasForeignKey(t => t.FlightId);
            modelBuilder.Entity<Ticket>().HasOne(t => t.User).WithMany(u => u.UserTickets).HasForeignKey(t => t.UserId);
            modelBuilder.Entity<Ticket>().HasOne(t => t.Seat).WithMany(s => s.Tickets).HasForeignKey(t =>new { t.FlightId, t.ClassId });
        }
    }
}