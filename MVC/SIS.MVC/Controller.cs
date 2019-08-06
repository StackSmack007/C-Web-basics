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
    using System.Linq;
    using System.Net;
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
            if (Request.Cookies.ContainsCookie(cookieService.LoginCookieName))
            {
                var loginCookie = Request.Cookies.GetCookie(cookieService.LoginCookieName);
                try
                {
                    string loginCookieValue = loginCookie.Value;
                    this.curentUser = LoggedUser.Parse(loginCookieValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid cookie!"+Environment.NewLine+ex.Message);
                    LogOffUser();
                    this.curentUser = null;
                }
            }
        }

        protected Controller()
        {
            ViewEngine = (IViewEngine)WebHost.ServiceContainer.CreateInstance(typeof(IViewEngine));
            curentUser = null;
            hasher = new HashingManager();
            Response = new HttpResponse(HttpStatusCode.OK);
            cookieService = new CookieService();
            encrypter = (IEncrypter)WebHost.ServiceContainer.CreateInstance(typeof(IEncrypter));
            ViewData = new Dictionary<string, object>();
        }

        protected IDictionary<string, object> ViewData { get; set; }

        protected IHttpResponse Response { get; set; }

        public IHttpRequest Request { get; set; } //will be set in routing but not in constructor because in constructor other things will be given!

        protected static ICookieService cookieService;


        protected void LogOffUser()
        {
            var logDataCookie = Request.Cookies.GetCookie(cookieService.LoginCookieName);
            var cookieDelete = new HttpCookie(logDataCookie.Key, logDataCookie.Value, true, -1, true, false);
            curentUser = null;
            this.Response.AddCookie(cookieDelete);
        }

        protected void LogInUser(string userName, int id,object Role=null)
        {
            curentUser = new LoggedUser(userName, id, DateTime.UtcNow.AddDays(1),Role);
            var loginCookie = cookieService.MakeLoginCookie(curentUser.EncryptUserData());
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
        protected IHttpResponse HtmlResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader("Content-type", "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
            return this.Response;
        }

        protected IHttpResponse RedirectResult(string location)
        {
            this.Response.Headers.Add(new HttpHeader("Location", location));
            this.Response.StatusCode = HttpStatusCode.Redirect;
            return this.Response;
        }

        protected IHttpResponse TextResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader("Content-type", "text/plain"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
            return this.Response;
        }
        #endregion
  
        #region MessagesForUser
        protected virtual IHttpResponse MessageError(string message, string redirectAdress = "/", string redirectName = "HomePage", string layoutName = "DefaultLayout")
        {
            return ControllerMessage("danger", message, redirectAdress, redirectName, layoutName);
        }

        protected virtual IHttpResponse MessageSuccess(string message, string redirectAdress = "/", string redirectName = "HomePage", string layoutName = "DefaultLayout")
        {
            return ControllerMessage("success", message, redirectAdress, redirectName, layoutName);
        }

        private IHttpResponse ControllerMessage(string alertType, string message, string redirectAdress = "/", string redirectName = "HomePage", string layoutName = "DefaultLayout")
        {
            if (layoutName == "DefaultLayout")   layoutName = WebHost.Configurations.DefaultLayoutName;

            ViewData["USERNAME"] = this.CurentUser is null ? null : this.CurentUser.UserName;
            ViewData["ROLE"] = this.CurentUser is null ? null : this.CurentUser.Role;
            string layoutPath = Locator.GetPathOfFile(WebHost.Configurations.LayoutsFolderPath, layoutName);
            string layout = File.ReadAllText(layoutPath);
            string warninghtml = $"<div class=\"alert alert-{alertType}\" role=\"alert\">{message}! Go back to <a href=\"{redirectAdress}\"class=\"alert-link\">{redirectName}</a>.</div>";
            string htmlContent = layout.Replace(WebHost.Configurations.KeywordForInsertingBodyInImportLayout, warninghtml);
            htmlContent = ViewEngine.GetHtmlImbued(htmlContent, ViewData);
            this.HtmlResult(htmlContent);
            return this.Response;
        }

        /// <summary>
        /// You are in post method if the get method requires models you must fil ViewData with this models before calling the view
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isError">if not error then it is success</param>
        /// <param name="viewSubPath">Provide if you dont wish to get the View from get method with the same name</param>
        /// <param name="layoutName">Provide if you wish another layout but the default one</param>
        /// <returns></returns>
        protected virtual IHttpResponse MessageWithView(string message, bool isError = true, string viewSubPath = "Default", string layoutName = "DefaultLayout")
        {
            if (layoutName == "DefaultLayout") layoutName = WebHost.Configurations.DefaultLayoutName;

            ViewData["USERNAME"] = this.CurentUser is null ? null : this.CurentUser.UserName;
            var alertType = "danger";
            if (!isError) alertType = "success";
            string layoutPath = Locator.GetPathOfFile(WebHost.Configurations.LayoutsFolderPath, layoutName);
            string layout = File.ReadAllText(layoutPath);

            string warninghtml = $@"<div class=""alert alert-{alertType} alert - dismissible fade show"" role=""alert"">
                                    <strong> {message}</strong>
                                    <button type = ""button"" class=""close"" data -dismiss=""alert"" aria -label=""Close"">
                                    <span aria-hidden=""true""> &times;</span>
                                    </button>
                                    </div>";

            string htmlContent = layout.Replace(WebHost.Configurations.KeywordForInsertingBodyInImportLayout, warninghtml);
            htmlContent = ViewEngine.GetHtmlImbued(htmlContent, ViewData)
                + Environment.NewLine + WebHost.Configurations.KeywordForInsertingBodyInImportLayout; ;

            if (viewSubPath == "Default")
            {
                View("void");
            }
            else
            {
                ViewFilePath(viewSubPath, "void");
            }
            string viewContent = Encoding.UTF8.GetString(Response.Content);
            htmlContent = htmlContent.Replace(WebHost.Configurations.KeywordForInsertingBodyInImportLayout, viewContent);
            Response.Content = Encoding.UTF8.GetBytes(htmlContent);
            return this.Response;
        }

        #endregion

        protected virtual IHttpResponse View(string layoutName = "DefaultLayout")
        {
            StackTrace stackTrace = new StackTrace();
            string actionMethodName = stackTrace.GetFrames()
                .Select(x => x.GetMethod().Name)
                .Where(x => x != "View" && x != "MessageWithView").FirstOrDefault();
            string className = this.GetType().Name;
            string folderName = className.Replace("Controller", "/");
            string htmlName = actionMethodName + ".html";
            string subPath = folderName + htmlName;
            return ViewFilePath(subPath, layoutName);
        }

        protected virtual IHttpResponse ViewFilePath(string subPath, string layoutName = "DefaultLayout")
        {
            if (layoutName == "DefaultLayout") layoutName = WebHost.Configurations.DefaultLayoutName;
            ViewData["USERNAME"] = this.CurentUser is null ? null : this.CurentUser.UserName;
            ViewData["ROLE"] = this.CurentUser is null ? null : this.CurentUser.Role;
            if (subPath.StartsWith("/"))
            {
                subPath = subPath.Substring(1).Replace(".html", "") + ".html";
            }

            string layout = WebHost.Configurations.KeywordForInsertingBodyInImportLayout;
            if (layoutName.ToLower() != "void")
            {
                layout = GetLayoutContent(layoutName);
            }

            string path = Locator.GetPathOfFile(WebHost.Configurations.LocationOfViewsFolder + subPath);
            string htmlContent = File.ReadAllText(path);
            htmlContent = layout.Replace(WebHost.Configurations.KeywordForInsertingBodyInImportLayout, htmlContent);
            htmlContent = ViewEngine.GetHtmlImbued(htmlContent, ViewData);
            this.HtmlResult(htmlContent);
            return this.Response;
        }

        private string GetLayoutContent(string layoutName)
        {
            string layoutPath = Locator.GetPathOfFile(WebHost.Configurations.LayoutsFolderPath, layoutName);
            return File.ReadAllText(layoutPath);
        }
    }
}