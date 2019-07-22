namespace SIS.MVC.Attributes
{
    using SIS.HTTP.Enums;
    public class HttpDeleteAttribute : HttpAttribute
    {
        public HttpDeleteAttribute(string path) : base(path)
        {
            MethodType = HttpRequestMethod.Delete;
        }
    }
}