namespace MishMashApplication.DTO.Users
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string VerifyPassword { get; set; }
        public string Email { get; set; }

        public UserDTO(string username, string password1, string password2, string email) : this(username, password1)
        {
            VerifyPassword = password2;
            Email = email;
        }

        public UserDTO(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}