namespace SIS.Framework.Attributes
{
    using SIS.HTTP.Enums;

    public class HttpPutAttribute : HttpMethodAttribute
    {
        public override bool IsValid(HttpRequestMethod requestMethod)
        {
            if (requestMethod is HttpRequestMethod.Put)
            {
                return true;
            }
            return false;
        }
    }
}