﻿namespace SIS.HTTP.Responses.Contracts
{
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Headers.Contracts;
    using System.Net;
    public interface IHttpResponse
    {
        HttpStatusCode StatusCode { get; set; }
        IHttpHeaderCollection Headers { get; set; }
        byte[] Content { get; set; }
        void AddHeader(HttpHeader header);
        byte[] GetBytes();
        void AddCookie(HttpCookie cookie);
    }

}