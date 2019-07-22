namespace Application.ViewModels.Authentication
{
  public  class UserDto
    {
        public string UserName { get; }
        public string Password { get; }
        public string PasswordVerify { get; }

        public UserDto(string username, string password)
        {
            this.UserName = username;
            this.Password = password;
        }

        public UserDto(string username, string password,string verifyPassword):this(username,password)
        {
            PasswordVerify = verifyPassword;
        }
    }
}