using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.HTTP.Cookies
{
    public class HttpCookie
    {
        public const int HttpCookieDefaultExpirationDays = 3;
        public HttpCookie(string key, string value, int expires = HttpCookieDefaultExpirationDays)
        {
            this.Key = key;
            this.Value = value;
            this.Expires = DateTime.UtcNow.AddDays(expires);
            this.IsNew = true;//default
        }
        public HttpCookie(string key, string value, bool isNew, int expires = HttpCookieDefaultExpirationDays) : this(key, value, expires)
        {
            this.IsNew = isNew;

        }


        public string Key { get; }

        public string Value { get; }

        public DateTime Expires { get; }


        public bool IsNew { get; }

        public override string ToString()
        {
            return $"{Key}={Value}; Expires={this.Expires.ToString("R")}";
        }
    }
}