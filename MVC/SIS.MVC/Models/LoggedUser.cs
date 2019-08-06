namespace SIS.MVC.Models
{
    using SIS.MVC.Contracts;
    using System;
    public class LoggedUser
    {
        protected static IEncrypter encrypter;
        private static string Separator = "<--|-->";

        static LoggedUser()
        {
            encrypter = (IEncrypter)WebHost.ServiceContainer.CreateInstance(typeof(IEncrypter));
        }

        public LoggedUser(string name, int id, DateTime expireDate, object role = null)
        {
            UserName = name;
            Id = id;
            CookieExpireDateTime = expireDate;

            if (role != null)
            {
                Role = role.ToString();
            }
            else
            {
                Role = string.Empty;
            }
        }

        public int Id { get; }
        public string UserName { get; }
        public string Role { get; }
        public DateTime CookieExpireDateTime { get; }

        internal string EncryptUserData()
        {
            string data = $"{Id}{Separator}{UserName}{Separator}{CookieExpireDateTime}{Separator}{Role}";
            string encryptedData = encrypter.Encrypt(data);
            return encryptedData;
        }

        internal static LoggedUser Parse(string encryptedData)
        {
            string[] data = encrypter.Decrypt(encryptedData).Split(Separator);
            int id = int.Parse(data[0]);
            string name = data[1];
            DateTime expireDate = DateTime.Parse(data[2]);
            string role = data[3];
            return new LoggedUser(name, id, expireDate, role);
        }
    }
}