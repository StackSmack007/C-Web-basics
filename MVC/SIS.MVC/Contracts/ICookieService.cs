namespace SIS.MVC.Contracts
{
    using SIS.HTTP.Cookies;
    public interface ICookieService
    {
        string LoginCookieName { get; }

        HttpCookie MakeLoginCookie(string userName, int id, IEncrypter encrypter);
    }
}