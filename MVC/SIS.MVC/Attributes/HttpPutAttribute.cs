namespace SIS.MVC.Attributes
{
    using SIS.HTTP.Enums;
    public class HttpPutAttribute : HttpAttribute
    {
        public HttpPutAttribute(string path) : base(path)
        {
            MethodType = HttpRequestMethod.Put;
        }
    }
}