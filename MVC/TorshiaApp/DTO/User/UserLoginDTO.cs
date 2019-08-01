namespace TorshiaApp.DTO.User
{
    public class UserLoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserLoginDTO(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}