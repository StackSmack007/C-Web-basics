namespace IRunes.Infrastructure.Models.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User : BaseModel<string>
    {
        public User()
        {
            AlbumsCreated = new HashSet<Album>();
        }

        [Required,MaxLength(64)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public virtual ICollection<Album> AlbumsCreated { get; set; }
    }
}