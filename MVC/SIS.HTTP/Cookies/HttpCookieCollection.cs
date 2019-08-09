namespace SIS.HTTP.Cookies
{
    using SIS.HTTP.Cookies.Contracts;
    using System.Collections.Generic;
    using System.Text;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private IDictionary<string, HttpCookie> cookies;

        public HttpCookieCollection()
        {
            cookies = new Dictionary<string, HttpCookie>();
        }

        public void Add(HttpCookie cookie)
        {
            cookies[cookie.Key] = cookie;
        }

        public bool ContainsCookie(string key)
        {
            if (cookies.ContainsKey(key))
            {
                return true;
            }
            return false;
        }

        public HttpCookie GetCookie(string key)
        {
            if (!cookies.ContainsKey(key))
            {
                return null;
            }
            return cookies[key];
        }

        public bool HasCookies()
        {
            return cookies.Count > 0;
        }

        public void Remove(string key)
        {
            if (ContainsCookie(key))
            {
                cookies.Remove(key);
            }
        }

        public override string ToString()
        {
            return string.Join("; ",cookies.Values);
        }


        public string ToStringSet()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in cookies.Values)
            {
                sb.AppendLine($"Set-Cookie: {item.ToString()}");
            }

            return sb.ToString().Trim() ;
        }//NSH
    }
}