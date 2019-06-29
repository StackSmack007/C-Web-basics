namespace Application.Controllers
{
    using Infrastructure.Data;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public abstract class BaseController
    {
        protected CakeContext db;
        protected BaseController()
        {
            db = new CakeContext();
        }
        protected IHttpResponse ControllerError(string message,string redirectAdress= "/", string redirectName= "HomePage")
        {
            string htmlContent = $@"<h1 style=""color: yellow; background:black"">Error: {message} !</h1>
                                 <h2>Return <a href=""{redirectAdress}"">Back to {redirectName}</a></h2>";

            IHttpResponse htmlResponse = new HtmlResult(htmlContent, System.Net.HttpStatusCode.OK);
            return htmlResponse;
        }

        protected IHttpResponse View()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            string actionMethodName = methodBase.Name;
            string className = methodBase.ReflectedType.Name;
            string originalDestination = @"../../../Views/";
            string folderName = className.Replace("Controller", "/");
            string htmlName = actionMethodName + ".html";
            string path = originalDestination + folderName + htmlName;
            string htmlContent = File.ReadAllText(path);
            IHttpResponse htmlResponse = new HtmlResult(htmlContent, System.Net.HttpStatusCode.OK);
            return htmlResponse;
        }



    }
}