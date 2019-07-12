namespace SIS.MVC.Services
{
    using System;
    public class LoggedUser
    {
        public LoggedUser(string name, int id,DateTime expireDate)
        {
            UserName = name;
            Id = id;
            CookieExpireDateTime=expireDate;
        }

        public string UserName { get; }
        public int Id { get; }
        public DateTime CookieExpireDateTime { get; }
    }
}