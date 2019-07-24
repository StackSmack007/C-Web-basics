namespace SIS.MVC
{
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Contracts;
    using SIS.MVC.Models;
    using SIS.MVC.Services;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Web;

    public abstract class Controller
    {
        private IViewEngine ViewEngine;

        private LoggedUser curentUser;

        protected readonly IEncrypter encrypter;

        protected readonly HashingManager hasher;

        protected LoggedUser CurentUser
        {
            get
            {
                if (curentUser != null && curentUser.CookieExpireDateTime > DateTime.UtcNow)
                {
                    return curentUser;
                }
                SetUserFromRequest();
                return curentUser;
            }
        }

        private void SetUserFromRequest()
        {
            LoggedUser logedUser = null;
            if (Request.Cookies.ContainsCookie(cookieService.LoginCookieName))
            {
                var loginCookie = Request.Cookies.GetCookie(cookieService.LoginCookieName);
                try
                {
                    string[] nameAndId = encrypter.Decrypt(loginCookie.Value).Split();
                    DateTime expireDate = loginCookie.Expires;
                    logedUser = new LoggedUser(nameAndId[0], int.Parse(nameAndId[1]), expireDate);

                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid cookie!");
                    LogOffUser();
                }
            }
            this.curentUser = logedUser;
        }

        protected IDictionary<string, object> ViewData { get; set; }

        protected IHttpResponse Response { get; set; }

        public IHttpRequest Request { get; set; } //will be set in routing but not in constructor because in constructor other things will be given!

        #region HardcorePathsByConvention NeedTo Outsourse to Config Class;
        private static string layoutsFolderPath = @"../../../Views/Layouts/";
        private static string keywordForInsertingBodyInImportLayout = "@BodyContent@";
        private static string regexPattern = @"(?<=@).*?(?=@)";
        private static string locationOfViewsFolder = @"../../../Views/";
        #endregion

        protected static ICookieService cookieService;

        protected Controller()
        {
            ViewEngine = (IViewEngine)WebHost.ServiceContainer.CreateInstance(typeof(IViewEngine));
            curentUser = null;
            hasher = new HashingManager();
            Response = new HttpResponse(HttpStatusCode.OK);
            cookieService = new CookieService();
            encrypter = new Encrypter();
            ViewData = new Dictionary<string, object>();
        }

        protected void LogOffUser()
        {
            var logDataCookie = Request.Cookies.GetCookie(cookieService.LoginCookieName);
            var cookieDelete = new HttpCookie(logDataCookie.Key, logDataCookie.Value, true, -1, true, false);
            this.Response.AddCookie(cookieDelete);
        }

        protected void LogInUser(string userName, int id)
        {
            var loginCookie = cookieService.MakeLoginCookie(userName, id, encrypter);
            this.Response.AddCookie(loginCookie);
        }

        #region String And URL encoding/decoding
        protected string DecodeUrl(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        protected string EncodeUrl(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        protected string Encrypt(string str)
        {
            return encrypter.Encrypt(str);
        }

        protected string Decrypt(string str)
        {
            return encrypter.Decrypt(str);
        }
        #endregion

        #region Response Manipulator Methods
        protected void HtmlResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader("Content-type", "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
        }

        protected void RedirectResult(string location)
        {
            this.Response.Headers.Add(new HttpHeader("Location", location));
            this.Response.StatusCode = HttpStatusCode.Redirect;
        }

        protected void TextResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader("Content-type", "text/plain"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
        }
        #endregion

        protected IHttpResponse ControllerError(string message, string redirectAdress = "/", string redirectName = "HomePage", string layoutName = "_importLayout.html")
        {
            string layoutPath = layoutsFolderPath + layoutName;
            string layout = File.ReadAllText(layoutPath);
            string warninghtml = $"<div class=\"alert alert-danger\" role=\"alert\">{message}! Go back to <a href=\"{redirectAdress}\"class=\"alert-link\">{redirectName}</a>.</div>";
            string htmlContent = layout.Replace(keywordForInsertingBodyInImportLayout, warninghtml);
            this.HtmlResult(htmlContent);
            return this.Response;
        }

        protected IHttpResponse ControllerSuccess(string message, string redirectAdress = "/", string redirectName = "HomePage", string layoutName = "_importLayout.html")
        {
            string layoutPath = layoutsFolderPath + layoutName;
            string layout = File.ReadAllText(layoutPath);
            string warninghtml = $"<div class=\"alert alert-success\" role=\"alert\">{message}! Go back to <a href=\"{redirectAdress}\"class=\"alert-link\">{redirectName}</a>.</div>";
            string htmlContent = layout.Replace(keywordForInsertingBodyInImportLayout, warninghtml);
            this.HtmlResult(htmlContent);
            return this.Response;
        }

        protected IHttpResponse View(string layoutName = "_importLayout.html")
        {
            string layoutPath = layoutsFolderPath + layoutName;
            bool fileExist = File.Exists(layoutPath);
            string layout = File.ReadAllText(layoutPath);

            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            string actionMethodName = methodBase.Name;
            string className = this.GetType().Name;
            string folderName = className.Replace("Controller", "/");
            string htmlName = actionMethodName + ".html";
            string path = locationOfViewsFolder + folderName + htmlName;
            string htmlContent = File.ReadAllText(path);
            htmlContent = layout.Replace(keywordForInsertingBodyInImportLayout, htmlContent);
            htmlContent = ViewEngine.GetHtmlImbued(htmlContent, ViewData);
            this.HtmlResult(htmlContent);
            return this.Response;
        }
    }
}