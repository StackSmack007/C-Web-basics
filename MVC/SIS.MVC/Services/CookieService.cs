namespace SIS.MVC.Services
{
using SIS.HTTP.Cookies;
using SIS.MVC.Contracts;
    public class CookieService : ICookieService
    {
        public string LoginCookieName => "LOG_IN";
             
        public HttpCookie MakeLoginCookie(string userName, int id,IEncrypter encrypter)
        {
            string nameAndId = userName + " " + id;
            string hashedUserNameAndId = encrypter.Encrypt(nameAndId);
            return new HttpCookie(LoginCookieName, hashedUserNameAndId, true, 1, true, false);
        }
    }
}
