namespace SIS.MVC.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AuthorisedAttribute : Attribute
    {
        public string AltPath { get; }
        public string Role { get; }
        public AuthorisedAttribute()
        {
            AltPath = null;
            Role = null;
        }

        public AuthorisedAttribute(string role,string alternativePath=null)
        {
            Role = role;
            AltPath = alternativePath;
        }
    }
}