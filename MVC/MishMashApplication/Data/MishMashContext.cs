namespace MishMashApplication.Data
{
    using Microsoft.EntityFrameworkCore;
    using MishMashApplication.Models;

    public class MishMashContext:DbContext
    {
        public MishMashContext() {}    

        public DbSet<User> Users { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ChannelUser> ChannelsUsers { get; set; }
        public DbSet<ChannelTag> ChannelsTags { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigContext.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChannelTag>().HasKey(x => new { x.ChannelId, x.TagId });
            modelBuilder.Entity<ChannelTag>().HasOne(x => x.Tag).WithMany(t=>t.TagChannels).HasForeignKey(x=>x.TagId);
            modelBuilder.Entity<ChannelTag>().HasOne(x => x.Channel).WithMany(t=>t.ChannelTags).HasForeignKey(x=>x.ChannelId);

            modelBuilder.Entity<ChannelUser>().HasKey(x => new { x.UserId, x.ChannelId });
            modelBuilder.Entity<ChannelUser>().HasOne(x => x.Channel).WithMany(c => c.ChannelUsers).HasForeignKey(x => x.ChannelId);
            modelBuilder.Entity<ChannelUser>().HasOne(x => x.User).WithMany(u => u.UserChannels).HasForeignKey(x => x.UserId);
        }
    }
}