namespace Chushka.App.ViewModels.Users
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
        public string FullName { get; set; }
        public string Email { get; set; }

        public UserLoginRegisterDTO(string username, string password, string verifyPassword, string fullName, string email) : this(username, password)
        {
            VerifyPassword = verifyPassword;
            FullName = fullName;
            Email = email;
        }
    }
}