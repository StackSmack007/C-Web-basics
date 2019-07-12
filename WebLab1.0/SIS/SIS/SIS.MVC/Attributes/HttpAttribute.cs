namespace SIS.MVC.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public abstract class HttpAttribute : Attribute
    {
        public string Path { get; }
        public string MethodName { get; }
        
        protected HttpAttribute(string path)
        {
            Path = path;
        }
    }
}