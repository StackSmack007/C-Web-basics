namespace IRunes.Infrastructure.Data.Configurations
{
    using IRunes.Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AlbumTracksConfiguration : IEntityTypeConfiguration<AlbumTrack>
    {
        public void Configure(EntityTypeBuilder<AlbumTrack> builder)
        {
                builder.HasKey(at => new { at.AlbumId, at.TrackId });
                builder.HasOne(at => at.Album).WithMany(a => a.AlbumTracks).HasForeignKey(at => at.AlbumId);
                builder.HasOne(at => at.Track).WithMany(t => t.TrackAlbums).HasForeignKey(at => at.TrackId);
          }
    }
}