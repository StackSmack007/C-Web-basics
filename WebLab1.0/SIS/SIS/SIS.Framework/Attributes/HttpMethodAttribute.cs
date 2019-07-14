namespace SIS.Framework.Attributes
{
    using SIS.HTTP.Enums;
    using System;
    public abstract  class HttpMethodAttribute:Attribute
    {
        public abstract bool IsValid(HttpRequestMethod requestMethod);
    }
}
