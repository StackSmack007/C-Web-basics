﻿namespace MishMashApplication.Models
{
    using MishMashApplication.Models.Enumerations;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User : BaseEntity<int>
    {
        public User()
        {
            UserChannels = new HashSet<ChannelUser>();
            Role = UserRole.User;
        }
        [Required, MaxLength(32)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [MaxLength(64)]
        public string Email { get; set; }
        public virtual UserRole Role { get; set; }

        public virtual ICollection<ChannelUser> UserChannels { get; set; }
    }
}