namespace Infrastructure.Models.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Product : BaseEntity<int>
    {
        public Product()
        {
            ProductOrders = new List<OrderProduct>();
        }

        [Required, MaxLength(32)]
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<OrderProduct> ProductOrders { get; set; }
    }
}