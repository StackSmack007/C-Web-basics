using SIS.HTTP.Headers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIS.HTTP.Headers
{
    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private Dictionary<string, HttpHeader> headers;
        public HttpHeaderCollection()
        {
            headers = new Dictionary<string, HttpHeader>();
        }


        public void Add(HttpHeader header)
        {

            if (header is null || header.Key is null || header.Value is null)
            {
                throw new InvalidOperationException("header has null parameters or is null");
            }
            if (headers.ContainsKey(header.Key))
            {
                throw new InvalidOperationException("the key is already present NSH");
            }
            headers.Add(header.Key, header);
        }

        public bool ContainsHeader(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("key can not be null!");
            }


            return headers.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            if (headers.ContainsKey(key))
            {
                return headers[key];
            }
            return null;
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, headers.Values);
        }
    }
}
