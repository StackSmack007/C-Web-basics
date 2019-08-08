namespace Airport.App.ViewModels.Users
{
    public class UserLoginRegisterDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public UserLoginRegisterDTO(string email, string password)
        {
            Email = email;
            Password = password;
        }
        //Additional for register
        public string RepeatPassword { get; set; }
        
        public string Email { get; set; }

        public UserLoginRegisterDTO(string username, string password, string repeatPassword, string email) : this(email, password)
        {
            RepeatPassword = repeatPassword;
            Username = username;
        }
    }
}