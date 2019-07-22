namespace SIS.HTTP.Cookies
{
    using System;
    public class HttpCookie
    {
        public const int HttpCookieDefaultExpirationDays = 3;
        public HttpCookie(string key, string value, int expires = HttpCookieDefaultExpirationDays)
        {
            this.Key = key;
            this.Value = value;
            this.Expires = DateTime.UtcNow.AddDays(expires);
            this.IsNew = true;//default
            IsHttpOnly = false;//default
            this.ForCurrentPathOnly=true;//default
        }
        public HttpCookie(string key, string value, bool isNew, int expires = HttpCookieDefaultExpirationDays, bool isHttpOnly = false,bool forCurrentPathOnly=true) : this(key, value, expires)
        {
            this.IsNew = isNew;
            this.IsHttpOnly = isHttpOnly;
            this.ForCurrentPathOnly = forCurrentPathOnly;//default
        }

        public bool IsHttpOnly { get; }

        public bool ForCurrentPathOnly { get; }

        public string Key { get; }

        public string Value { get; }

        public DateTime Expires { get; }

        public bool IsNew { get; }

        public override string ToString()
        {
            string HttoOnlySetting = IsHttpOnly ? "; HttpOnly" : "";
            string pathModiSetting = ForCurrentPathOnly ?  "": "; Path=/";
            return $"{Key}={Value}; Expires={this.Expires.ToString("R")}{HttoOnlySetting}{pathModiSetting}";
        }
    }
}