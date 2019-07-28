namespace MishMashApplication.Models
{
    using MishMashApplication.Models.Enumerations;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Channel : BaseEntity<int>
    {
        public Channel()
        {
            ChannelTags = new HashSet<ChannelTag>();
            ChannelUsers = new HashSet<ChannelUser>();
        }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }

        public virtual ChannelType Type { get; set; }

        public virtual ICollection<ChannelTag> ChannelTags { get; set; }

        public virtual ICollection<ChannelUser> ChannelUsers { get; set; }
    }
}