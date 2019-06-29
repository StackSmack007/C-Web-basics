namespace Infrastructure.Models.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Order : BaseEntity<int>
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}