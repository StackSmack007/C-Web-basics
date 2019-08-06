namespace Chushka.Models.Models
{
    using Chushka.Models.Models.Enumerations;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public  class Product:BaseEntity<int>
    {
        public Product()
        {
            Orders = new HashSet<Order>();
        }
        
        [Required, MaxLength(32)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        [MaxLength(2560)]
        public string Description { get; set; }

        public virtual ProductType Type { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

    }
}
