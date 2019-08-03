namespace SIS.MVC.Services
{
using SIS.HTTP.Cookies;
using SIS.MVC.Contracts;
    public class CookieService : ICookieService
    {
        public string LoginCookieName => "LOG_IN";
        private const int expireDays = 1;
        public HttpCookie MakeLoginCookie(string userDataContent)
        {
            return new HttpCookie(LoginCookieName, userDataContent, true, expireDays, true, false);
        }
    }
}
