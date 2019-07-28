using System.ComponentModel.DataAnnotations.Schema;

namespace MishMashApplication.Models
{
    public class ChannelUser
    {
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public int ChannelId { get; set; }
        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }
    }
}