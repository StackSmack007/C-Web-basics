namespace SIS.MVC.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AuthorisedAttribute : Attribute
    {
        public string AltPath { get; }
        public AuthorisedAttribute()
        {
            AltPath = null;
        }

        public AuthorisedAttribute(string alternativePath)
        {
            AltPath = alternativePath;
        }
    }
}