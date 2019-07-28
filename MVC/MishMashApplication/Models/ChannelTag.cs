namespace MishMashApplication.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class ChannelTag
    {
        public int ChannelId { get; set; }
        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }

        public int TagId { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual Tag Tag { get; set; }
    }
}