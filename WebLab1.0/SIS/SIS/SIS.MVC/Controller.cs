using SIS.HTTP.Cookies;
using SIS.HTTP.Headers;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses;
using SIS.HTTP.Responses.Contracts;
using SIS.MVC.Services;
using SIS.WebServer.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SIS.MVC
{
    public abstract class Controller
    {
        protected IDictionary<string, object> ViewData;

        protected const string loginCookieName = "LOG_IN";

        protected readonly Encrypter encrypter;

        protected readonly HashingManager hasher;

        public IHttpRequest Request { get; set; }//will be set in routing but not in constructor because in constructor other things will be given!

        protected IHttpResponse Response { get; set; }

        #region HardcorePathsByConvention
        private static string importLayoutPath = @"../../../Views/Layouts/_importLayout.html";
        private static string keywordForInsertingBodyInImportLayout = "@BodyContent@";
        private static string regexPattern = @"(?<=@).*?(?=@)";
        private static string locationOfViewsFolder = @"../../../Views/";
        #endregion

        private static string layout = File.ReadAllText(importLayoutPath);

        protected Controller()
        {
            hasher = new HashingManager();
            encrypter = new Encrypter();
            ViewData = new Dictionary<string, object>();
            Response = new HttpResponse(HttpStatusCode.OK);
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

        protected IHttpResponse ControllerError(string message, string redirectAdress = "/", string redirectName = "HomePage")
        {
            string warninghtml = $"<div class=\"alert alert-danger\" role=\"alert\">{message}! Go back to <a href=\"{redirectAdress}\"class=\"alert-link\">{redirectName}</a>.</div>";
            string htmlContent = layout.Replace(keywordForInsertingBodyInImportLayout, warninghtml);
            this.HtmlResult(htmlContent);
            return this.Response;
        }

        protected IHttpResponse ControllerSuccess(string message, string redirectAdress = "/", string redirectName = "HomePage")
        {
            string warninghtml = $"<div class=\"alert alert-success\" role=\"alert\">{message}! Go back to <a href=\"{redirectAdress}\"class=\"alert-link\">{redirectName}</a>.</div>";
            string htmlContent = layout.Replace(keywordForInsertingBodyInImportLayout, warninghtml);
            this.HtmlResult(htmlContent);
            return this.Response;
        }

        protected IHttpResponse View()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            string actionMethodName = methodBase.Name;
            string className = methodBase.ReflectedType.Name;
            string folderName = className.Replace("Controller", "/");
            string htmlName = actionMethodName + ".html";
            string path = locationOfViewsFolder + folderName + htmlName;
            string htmlContent = File.ReadAllText(path);
            htmlContent = InsertData(htmlContent);
            htmlContent = layout.Replace(keywordForInsertingBodyInImportLayout, htmlContent);
            this.HtmlResult(htmlContent);
            return this.Response;
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
    }

}