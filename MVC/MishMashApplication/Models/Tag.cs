using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MishMashApplication.Models
{
    public class Tag:BaseEntity<int>
    {
        public Tag()
        {
            TagChannels = new HashSet<ChannelTag>();
        }

        [Required, MaxLength(32)]
        public string Name { get; set;}

        public virtual ICollection<ChannelTag> TagChannels { get; set; }
    }
}