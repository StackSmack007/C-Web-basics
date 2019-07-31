﻿namespace SIS.MVC.Attributes
{
    using SIS.HTTP.Enums;
    public class HttpGetAttribute : HttpAttribute

    {
        public HttpGetAttribute(string path=null) : base(path)
        { MethodType = HttpRequestMethod.Get;}
    }
}