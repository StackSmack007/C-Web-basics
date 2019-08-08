namespace Airport.Infrastructure.Models.Models
{
    using Airport.Infrastructure.Models.Models.Enumerations;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class User : BaseEntity<int>
    {
        public User()
        {
            UserTickets = new HashSet<Ticket>();
            Role = UserRole.User;
        }

        [Required, MaxLength(32)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, MaxLength(64), EmailAddress]
        public string Email { get; set; }

        public virtual UserRole Role { get; set; }

        public virtual ICollection<Ticket> UserTickets { get; set; }
    }
}
//    	Users can register in the system.After successful registration,
//      the user has name, email, password, and list of tickets.