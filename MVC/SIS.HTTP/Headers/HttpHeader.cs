﻿namespace SIS.HTTP.Headers
{
    public class HttpHeader
    {
        public static string ContentLengthKey = "Content-Length";
        public static string ContentDispositionKey = "Content-Disposition";
        public HttpHeader(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }
        public string Value { get; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Key, this.Value);
        }
    }
}