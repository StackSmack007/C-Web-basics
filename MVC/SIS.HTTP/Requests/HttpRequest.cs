namespace SIS.HTTP.Requests
{
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Cookies.Contracts;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Extensions;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Sessions.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            FormData = new Dictionary<string, object>();
            QueryData = new Dictionary<string, object>();
            Headers = new HttpHeaderCollection();
            Cookies = new HttpCookieCollection();
            ParseRequest(requestString);
        }

        private void ParseRequest(string requestString)
        {
            string[] splitRequestContent = requestString.Split(Environment.NewLine);
            string[] requestLine = splitRequestContent[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }
            ParseRequestMethod(requestLine);
            ParseRequestUrl(requestLine);
            ParseRequestPath();
            ParseHeaders(splitRequestContent.Skip(1).ToArray());
            ParseCookies();

            string formData = string.Empty;

            if (splitRequestContent.Length > 2)
            {
                string lastMeaningfullLine = splitRequestContent.LastOrDefault(x => !string.IsNullOrEmpty(x));
                int indexOfTheLine = Array.IndexOf(splitRequestContent, lastMeaningfullLine);
                int indexOfFirstEmptyLine = Array.IndexOf(splitRequestContent, string.Empty);
                if (indexOfFirstEmptyLine < indexOfTheLine)
                {
                    formData = lastMeaningfullLine;
                }
            }
            ParseRequestParameters(formData);
        }

        private void ParseCookies()
        {
            var cookies = Headers.GetHeader("Cookie");
            if (cookies != null)
            {
                var coockieValues = cookies.Value.Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cookie in coockieValues)
                {
                    string[] cookieParams = cookie.Split('=', 2);
                    string key = cookieParams.First();
                    string value = cookieParams.Last();
                    try
                    {
                    this.Cookies.Add(new HttpCookie(key, value));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }//Done

        #region ParsingOfURLorBody
        private void ParseRequestParameters(string formData)
        {
            if (!string.IsNullOrEmpty(formData))
            {
                ParseFormDataParameters(formData);
            }
            ParseQueryParameters();
        }//done

        private void ParseFormDataParameters(string formData)
        {
            string[] forms = formData.Split('&');
            foreach (string form in forms)
            {
                AddRequestKVPtoDictionary(this.FormData, form);
            }
        }//done

        private void ParseQueryParameters()
        {

            // string queriesAll = this.Url.Split(new[] { '?', '#' }).Skip(1).First();
            string queriesAll = this.Url.Split(new[] { '?', '#' }).Skip(1).FirstOrDefault();

            if (string.IsNullOrEmpty(queriesAll))
            {
                return;
            }

            string[] queries = queriesAll.Split('&');

            foreach (string query in queries)
            {
                AddRequestKVPtoDictionary(QueryData, query);
            }
        }//done

        private void AddRequestKVPtoDictionary(Dictionary<string, object> target, string query, bool overwrite = false)
        {
            string[] kvp = query.Split('=');
            if (kvp.Length != 2 || kvp.Any(x => string.IsNullOrEmpty(x)))
            {
                throw new BadRequestException();
            }
            if (overwrite = false & this.QueryData.ContainsKey(kvp[0]))
            {
                throw new InvalidOperationException($"a kvp with key {kvp[0]} is already used!");
            }
            target[kvp[0]] = kvp[1];
        }//NSH
        #endregion

        private bool IsValidRequestLine(string[] requestLine)
        {
            string method = requestLine[0];
            if (requestLine.Length != 3 ||
                requestLine[2] != GlobalConstants.HttpOneProtocolFragment ||
                !Enum.TryParse(method, true, out HttpRequestMethod variable))
            {
                return false;
            }
            return true;
        }//done

        private void ParseRequestMethod(string[] requestLine)
        {
            if (!requestLine.Any())
            {
                throw new BadRequestException();
            }

            string methodName = requestLine[0];

            if (!Enum.TryParse<HttpRequestMethod>(methodName, true, out var method))
            {
                throw new BadRequestException();
            }
            this.RequestMethod = method;
        }//done

        private void ParseRequestUrl(string[] requestLine)
        {
            this.Url = requestLine[1];
        }//done

        private void ParseRequestPath()
        {
            string path = this.Url.Split(new[] { '?', '#' }, StringSplitOptions.RemoveEmptyEntries)[0];
            this.Path = path;
        }//done

        private void ParseHeaders(string[] requestHeaders)
        {
            Dictionary<string, string> headersAll = new Dictionary<string, string>();
            bool hostFound = false;
            if (!requestHeaders.Any())
            {
                throw new BadRequestException();
            }

            foreach (var row in requestHeaders)
            {
                if (string.IsNullOrEmpty(row)) break;
                string[] headerParts = row.Split(": ", 2);
                string headerName = headerParts[0];
                string headerContent = headerParts[1];

                if (headerName == "Host")
                {
                    hostFound = true;
                }
                var header = new HttpHeader(headerName, headerContent);
                Headers.Add(header);
            }
            if (!hostFound)
            {
                throw new BadRequestException();
            }
        }//done

        private bool IsValidRequestQueryString(string queryString, string[] queryParameters)
        {
            return string.IsNullOrEmpty(queryString) && queryParameters.Any();
        }//TOCHECK where to use?

        public IHttpCookieCollection Cookies { get; private set; }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IHttpSession Session { get; set ; }
    }
}