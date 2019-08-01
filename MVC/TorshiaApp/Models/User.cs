using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TorshiaApp.Models.Enumerations;

namespace TorshiaApp.Models
{
    public class User : BaseEntity<int>
    {
        public User()
        {
            Reports = new HashSet<Report>();
            Role = UserRole.User;
        }

        [Required, MaxLength(64)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required, MaxLength(128)]
        public string Email { get; set; }

        public virtual UserRole Role { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }
}