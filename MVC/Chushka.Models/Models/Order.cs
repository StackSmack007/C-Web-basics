namespace Chushka.Models.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Order : BaseEntity<int>
    {

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User Client { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        public DateTime OrderedOn { get; set; }


    }
}