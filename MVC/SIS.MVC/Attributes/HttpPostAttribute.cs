namespace SIS.MVC.Attributes
{
    using SIS.HTTP.Enums;
    public class HttpPostAttribute : HttpAttribute
    {
        public HttpPostAttribute(string path) : base(path)
        { MethodType = HttpRequestMethod.Post; }
        public HttpPostAttribute() : base(null)
        { MethodType = HttpRequestMethod.Post; }
    }
}