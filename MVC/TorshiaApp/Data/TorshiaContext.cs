namespace TorshiaApp.Data
{
    using Microsoft.EntityFrameworkCore;
    using TorshiaApp.Data.Configs;
    using TorshiaApp.Models;

    public class TorshiaContext : DbContext
    {
        public TorshiaContext()
        { }

       public DbSet<User> Users { get; set; }
       public DbSet<Participant> Participants { get; set; }
       public DbSet<Report> Reports { get; set; }
       public DbSet<Sector> Sectors { get; set; }
       public DbSet<Task> Tasks { get; set; }
       public DbSet<TaskParticipant> TasksParticipants { get; set; }
       public DbSet<TaskSector> taskSectors { get; set; }

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
            modelBuilder.ApplyConfiguration(new SectorCFG());
            modelBuilder.ApplyConfiguration(new TaskParticipantCFG());
            modelBuilder.ApplyConfiguration(new TaskSectorCFG());
            base.OnModelCreating(modelBuilder);
        }
    }
}