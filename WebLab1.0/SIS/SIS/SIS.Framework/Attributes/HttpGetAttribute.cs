namespace SIS.Framework.Attributes
{
    using SIS.HTTP.Enums;
    public class HttpGetAttribute : HttpMethodAttribute
    {
        public override bool IsValid(HttpRequestMethod requestMethod)
        {
            if (requestMethod is HttpRequestMethod.Get)
            {
                return true;
            }
            return false;
        }
    }
}