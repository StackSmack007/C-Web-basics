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

        public LoggedUser()
        {
            Username = string.Empty;
            Role = string.Empty;
            Info = string.Empty;
        }

        public LoggedUser(string name, int id, DateTime expireDate,  object role = null,string info = null)
        {
            Username = name;
            Id = id;
            CookieExpireDateTime = expireDate;

            if (role != null) Role = role.ToString();
            else Role = string.Empty;

            if (info != null) Info = info.ToString();
            else Info = string.Empty;
        }

        public int Id { get; set; }
        public string Info { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime CookieExpireDateTime { get; set; }

        internal string EncryptUserData()
        {
            string data = $"{Id}{Separator}{Username}{Separator}{CookieExpireDateTime}{Separator}{Role}{Separator}{Info}";
            string encryptedData = encrypter.Encrypt(data);
            return encryptedData;
        }

        internal static LoggedUser Parse(string encryptedData)
        {
            string[] data = encrypter.Decrypt(encryptedData).Split(Separator);
            int id = int.Parse(data[0]);
            string name = data[1];
            DateTime expireDate = DateTime.Parse(data[2]);
            string role = data[3]==string.Empty?null: data[3];
            string info = data[4] == string.Empty ? null : data[4];
            return new LoggedUser(name, id, expireDate, role,info);
        }
    }
}