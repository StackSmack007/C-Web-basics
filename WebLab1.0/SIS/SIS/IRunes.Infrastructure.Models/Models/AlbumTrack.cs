namespace IRunes.Infrastructure.Models.Models
{
using System.ComponentModel.DataAnnotations.Schema;
    public class AlbumTrack
    {
        public string AlbumId { get; set; }
        [ForeignKey(nameof(AlbumId))]
        public virtual Album Album { get; set; }

        public string TrackId { get; set; }
        [ForeignKey(nameof(TrackId))]
        public virtual Track Track { get; set; }
    }
}