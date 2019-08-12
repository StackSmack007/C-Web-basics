namespace Panda.App.ViewModels.Users
{
    public class UserLoginRegisterDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public UserLoginRegisterDTO(string username, string password)
        {
            Username = username;
            Password = password;
        }
        //Additional for register
        public string VerifyPassword { get; set; }
        public string Email { get; set; }

        public UserLoginRegisterDTO(string username, string password, string confirmPassword, string email) : this(username, password)
        {
            VerifyPassword = confirmPassword;
            Email = email;
        }
    }
}