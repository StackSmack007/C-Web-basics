namespace IRunes.Infrastructure.Models.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Album : BaseModel<string>
    {
        public Album()
        {
            AlbumTracks = new HashSet<AlbumTrack>();
        }

        public string UserCreatorID { get; set; }
        [ForeignKey(nameof(UserCreatorID))]
        public virtual User UserCreator { get; set; }


        [Required,MaxLength(64)]
        public string Name { get; set; }

        public string CoverImgUrl { get; set; }
        [NotMapped]
        public decimal Price => AlbumTracks.Select(x=>x.Track.Price).Sum() * 0.87m;

        public virtual ICollection<AlbumTrack> AlbumTracks { get; set; }
    }
}