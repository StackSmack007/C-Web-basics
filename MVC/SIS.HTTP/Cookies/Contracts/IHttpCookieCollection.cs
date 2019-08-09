namespace SIS.HTTP.Cookies.Contracts
{
    public  interface IHttpCookieCollection
    {
        void Add(HttpCookie cookie);
        void Remove(string key);
        bool ContainsCookie(string key);
        HttpCookie GetCookie(string key);
        bool HasCookies();
        string ToStringSet();
    }
}