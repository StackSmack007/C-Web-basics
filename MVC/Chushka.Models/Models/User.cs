namespace Chushka.Models.Models
{
    using Chushka.Models.Models.Enumerations;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class User : BaseEntity<int>
    {
        public User()
        {
            Orders = new HashSet<Order>();
            Role = UserRole.User;
        }

        [Required, MaxLength(32)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required, MaxLength(64)]
        public string FullName { get; set; }

        [Required, MaxLength(64), EmailAddress]
        public string Email { get; set; }

        public virtual UserRole Role { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}