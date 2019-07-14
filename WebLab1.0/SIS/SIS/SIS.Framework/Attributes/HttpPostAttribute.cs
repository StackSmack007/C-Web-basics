namespace SIS.Framework.Attributes
{
    using SIS.HTTP.Enums;
    public class HttpPostAttribute : HttpMethodAttribute
    {
        public override bool IsValid(HttpRequestMethod requestMethod)
        {
            if (requestMethod is HttpRequestMethod.Post)
            {
                return true;
            }
            return false;
        }
    }
}
