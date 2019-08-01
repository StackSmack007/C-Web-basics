namespace TorshiaApp.DTO.User
{
 public   class UserRegisterDTO
    {
        public string Username { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string Email { get; set; }
        public UserRegisterDTO(string username, string password,string confirmPassword,string email)
        {
            this.Username = username;
            this.Password1 = password;
            this.Password2 = confirmPassword;
            this.Email = email;
        }
    }
}