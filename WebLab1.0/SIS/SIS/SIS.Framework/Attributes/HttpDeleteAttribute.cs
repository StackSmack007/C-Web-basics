namespace SIS.Framework.Attributes
{
    using SIS.HTTP.Enums;

    public class HttpDeleteAttribute : HttpMethodAttribute
    {
        public override bool IsValid(HttpRequestMethod requestMethod)
        {
            if (requestMethod is HttpRequestMethod.Delete)
            {
                return true;
            }
            return false;
        }
    }
}
