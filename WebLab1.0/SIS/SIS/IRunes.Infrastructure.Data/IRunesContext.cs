namespace IRunes.Infrastructure.Data
{
    using IRunes.Infrastructure.Data.Configurations;
    using IRunes.Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;

    public class IRunesContext : DbContext
    {
       public DbSet<User> Users {get;set;}
       public DbSet<Track> Tracks {get;set;}
       public DbSet<Album> Albums {get;set;} 
       public DbSet<AlbumTrack> AlbumsTracks {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigDatabase.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AlbumTracksConfiguration());
        }
    }
}