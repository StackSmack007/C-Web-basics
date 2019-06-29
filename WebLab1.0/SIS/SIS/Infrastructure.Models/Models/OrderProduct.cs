namespace Infrastructure.Models.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class OrderProduct
    {
        public int Quantity { get; set; }

        public int OrderID { get; set; }
        [ForeignKey(nameof(OrderID))]
        public virtual Order Order { get; set; }

        public int ProductID { get; set; }
        [ForeignKey(nameof(ProductID))]
        public virtual Product Product { get; set; }
    }
}