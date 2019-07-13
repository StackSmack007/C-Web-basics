namespace SIS.MVC.Attributes
{
    using SIS.HTTP.Enums;
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public abstract class HttpAttribute : Attribute
    {
        public string Path { get; }
        public HttpRequestMethod MethodType { get; protected set; }

        protected HttpAttribute(string path)
        {
            Path = path;
        }
    }
}