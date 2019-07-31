namespace SIS.MVC.Attributes
{
    using SIS.HTTP.Enums;
    public class HttpPostAttribute : HttpAttribute
    {
        public HttpPostAttribute(string path=null) : base(path)
        { MethodType = HttpRequestMethod.Post; }
    }
}