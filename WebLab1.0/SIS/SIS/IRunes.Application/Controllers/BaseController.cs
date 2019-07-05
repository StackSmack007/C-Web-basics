namespace IRunes.Application.Controllers
{
    using IRunes.Application.Services;
    using IRunes.Infrastructure.Data;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.HTTP.Sessions;
    using SIS.HTTP.Sessions.Contracts;
    using SIS.WebServer.Results;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web;

    public abstract class BaseController
    {
        private const string regexPattern = @"(?<=@).*?(?=@)";

        private const string userInfo = "userInfo";

        protected IDictionary<string, object> ViewData;

        protected const string sessionCookieName = "SES_ID";

        protected IRunesContext db;

        protected readonly HashingManager hasher;

        protected BaseController()
        {
            hasher = new HashingManager();

            db = new IRunesContext();

            ViewData = new Dictionary<string, object>();
        }

        protected IHttpResponse ResposeErrorMessageAndRedirect(string message, string redirectAdress = "/", string redirectName = "HomePage")
        {
            string htmlContent = $@"<h1 style=""color: yellow; background:black"">Error: {message} !</h1>
                                 <h2> &#x261C Return Back to <a href=""{redirectAdress}"">{redirectName}</a></h2>";

            IHttpResponse htmlResponse = new HtmlResult(htmlContent, System.Net.HttpStatusCode.OK);
            return htmlResponse;
        }
        /// <summary>
        /// Returns View from a folder with name same as the name of the 
        /// controller and html file with name same as the name of the method 
        /// unless name is given!
        /// </summary>
        /// <param name="htmlName">If name is given it will use it 
        /// instead of the name of the method</param>
        /// <returns></returns>
        protected IHttpResponse View(string htmlName = null)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            string actionMethodName = methodBase.Name;
            string className = this.GetType().Name;
            string originalDestination = @"../../../Views/";
            string folderName = className.Replace("Controller", "/");
            if (htmlName is null)
            {
                htmlName = actionMethodName;
            }
            htmlName += ".html";
            string path = originalDestination + folderName + htmlName;
            string htmlContent = File.ReadAllText(path);
            htmlContent = InsertData(htmlContent);
            IHttpResponse htmlResponse = new HtmlResult(htmlContent, System.Net.HttpStatusCode.OK);
            return htmlResponse;
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

        protected bool IsUserLogedIn(IHttpRequest request)
        {
            IHttpSession session = GetCurrentSession(request);
            if (session.ContainsParameter(userInfo))
            {
                return true;
            }
            return false;
        }


        protected string[] GetCurrentSessionUserIdandName(IHttpRequest request)
        {
            string[] result = null;
            IHttpSession session = GetCurrentSession(request);
            if (session.ContainsParameter(userInfo))
            {
                result = (string[])session.GetParameter(userInfo);
            }
            return result;
        }

        protected void LogInUser(IHttpRequest request, string userId, string userName)
        {
            IHttpSession session = GetCurrentSession(request);
            if (!session.ContainsParameter(userInfo))
            {
                string[] userIdNameArray = { userId, userName };
                session.AddParameter(userInfo, userIdNameArray);
                return;
            }
            throw new InvalidOperationException($"User {GetCurrentSessionUserIdandName(request)[1]} is already loged in! Log out First");
        }

        protected void LogOutUser(IHttpRequest request)
        {
            IHttpSession session = GetCurrentSession(request);
            if (session.ContainsParameter(userInfo))
            {
                session.ClearParameters();
                return;
            }
            throw new InvalidOperationException($"There is no loged in user at the moment!");
        }

        private IHttpSession GetCurrentSession(IHttpRequest request)
        {
            string sessionId = request.Cookies.GetCookie(sessionCookieName).Value;
            return HttpSessionStorage.GetSession(sessionId);
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