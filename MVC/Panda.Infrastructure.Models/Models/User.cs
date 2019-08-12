namespace Panda.Indfrastructure.Models.Models
{
    using Panda.Infrastructure.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class User : BaseEntity<int>
    {
        public User()
        {
            Receipts = new HashSet<Receipt>();
            Packages = new HashSet<Package>();
            Role = UserRole.User;        }

        [Required, MaxLength(32)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, MaxLength(64), EmailAddress]
        public string Email { get; set; }

        public virtual UserRole Role { get; set; }

        public virtual ICollection<Receipt> Receipts { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
    }
}