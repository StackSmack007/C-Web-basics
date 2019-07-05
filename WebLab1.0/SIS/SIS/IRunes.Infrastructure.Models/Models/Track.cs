namespace IRunes.Infrastructure.Models.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Track : BaseModel<string>
    {
        public Track()
        {
            TrackAlbums = new HashSet<AlbumTrack>();
        }

        [Required]
        public string Name { get; set; }

        public string LinkURL { get; set; }

        [Range(typeof(decimal),"0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public virtual ICollection<AlbumTrack> TrackAlbums { get; set; }
    }
}