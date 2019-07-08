namespace Application.Controllers
{
    using Application.Services;
    using Infrastructure.Data;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web;

    public abstract class BaseController
    {
        private const string regexPattern = @"(?<=@).*?(?=@)";
        protected IDictionary<string, object> ViewData;
        protected const string loginCookieName = "LOG_IN";
        protected CakeContext db;
        private readonly Encrypter encrypter;
        protected readonly HashingManager hasher;
  
        protected BaseController()
        {
            hasher = new HashingManager();
            encrypter = new Encrypter();
            db = new CakeContext();
            ViewData = new Dictionary<string, object>();
        }
        protected IHttpResponse ControllerError(string message, string redirectAdress = "/", string redirectName = "HomePage")
        {
            string htmlContent = $@"<h1 style=""color: yellow; background:black"">Error: {message} !</h1>
                                 <h2>Return Back to <a href=""{redirectAdress}"">{redirectName}</a></h2>";

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
            htmlContent = InsertData(htmlContent);
            IHttpResponse htmlResponse = new HtmlResult(htmlContent, System.Net.HttpStatusCode.OK);
            return htmlResponse;
        }

        protected HttpCookie GeLoginCookie(string userName)
        {
            string hashedUserName = encrypter.Encrypt(userName);
            return new HttpCookie(loginCookieName, hashedUserName, true, 1, true, false);
        }

        protected string GetUserNameFromCookie(HttpCookie cookie)
        {
            try
            {
                return encrypter.Decrypt(cookie.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string InsertData(string htmlContent)
        {
            MatchCollection matches = Regex.Matches(htmlContent, regexPattern);
            foreach (Match match in matches)
            {
                if (this.ViewData.ContainsKey(match.Value))
                {
                    htmlContent = htmlContent.Replace($"@{match.Value}@", this.ViewData[match.Value].ToString());
                }
            }
            return htmlContent;
        }

        protected bool VerifyMemberCookie(IHttpRequest request)
        {
            string cookieValue = request.Cookies.GetCookie(loginCookieName).Value;

            string userName = string.Empty;
            try
            {
                userName = encrypter.Decrypt(cookieValue);
            }
            catch (Exception)
            {
                return false;
            }
            return db.Users.Any(x => x.Username == userName);
        }

        protected string DecodeUrl(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        protected string EncodeUrl(string str)
        {
            return HttpUtility.UrlEncode(str);
        }
    }
}