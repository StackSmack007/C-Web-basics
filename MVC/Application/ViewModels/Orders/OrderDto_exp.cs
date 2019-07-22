namespace Application.ViewModels.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OrderDto_exp
    {
        public int OrderId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal TotalCost => Products.Sum(x => x.TotalPrice);

        public IList<ProductDto> Products { get; set; } = new List<ProductDto>();

    }
}