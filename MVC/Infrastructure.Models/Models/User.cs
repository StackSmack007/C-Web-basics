namespace Infrastructure.Models.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class User : BaseEntity<int>
    {
        public User()
        {
            Orders = new HashSet<Order>();
            RegisteredOn = DateTime.UtcNow;
        }
        [Required, MaxLength(32)]
        public string Username { get; set; }
        [Required,]
        public string Password { get; set; }
        public DateTime RegisteredOn { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}