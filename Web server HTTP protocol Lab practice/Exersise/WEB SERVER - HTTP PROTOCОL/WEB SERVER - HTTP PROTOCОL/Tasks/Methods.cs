namespace WEB_SERVER___HTTP_PROTOCОL.Tasks
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    public class Methods
    {
        private static StringBuilder sb;

        static Methods()
        {
            sb = new StringBuilder();
        }

        public static string DecodeUrl(string url)
        {
            string validUrl = WebUtility.UrlDecode(url);
            return validUrl;
        }

        public static string UrlParser(string url)
        {

            string validUrl = WebUtility.UrlDecode(url);
            string pattern = @"(?<Protocol>https*|ftp):\/\/(?<Host>[a-z\-]+\.[a-z]{2,3})(\:(?<Port>\d+))*((?<Path>[\w\.\/]+))*(\??(?<Fragment>(.+=.+)+))*";

            var match = Regex.Match(validUrl, pattern);
            if (match.Value != validUrl) return "Invalid URL";
            int port = 0;
            string protocol = match.Groups["Protocol"].Value.ToLower();
            string host = match.Groups["Host"].Value;
            string portProvided = match.Groups["Port"].Value;
            string path = match.Groups["Path"].Value;
            string query = match.Groups["Query"].Value;
            string fragent = match.Groups["Fragment"].Value;
            if ((protocol == "http" && portProvided == "443") || (protocol == "https" && portProvided == "80"))
            {
                return "Invalid URL";
            }
            if (string.IsNullOrEmpty(match.Groups["Port"].Value))
            {
                switch (protocol)
                {
                    case "http":
                        port = 80;
                        break;
                    case "https":
                        port = 443;
                        break;
                    case "ftp":
                        port = 20;
                        break;
                }
            }
            else
            {
                port = int.Parse(match.Groups["Port"].Value);
            }
            sb.Clear();
            sb.AppendLine($"Protocol: {protocol}");
            sb.AppendLine($"Host: {host}");
            sb.AppendLine($"Port: {port}");
            if (!string.IsNullOrEmpty(path))
            {
                sb.AppendLine($"Path: {path}");
            }
            if (!string.IsNullOrEmpty(query))
            {
                sb.AppendLine($"Query: {query}");
            }
            if (!string.IsNullOrEmpty(fragent))
            {
                sb.AppendLine($"Fragment: {fragent}");
            }
            return sb.ToString().Trim();
        }

        public static string WebServer()
        {
            Dictionary<string, HashSet<string>> methodPaths = new Dictionary<string, HashSet<string>>();
            string input;
            while ((input = Console.ReadLine()) != "END")
            {
                string method = input.Split('/').Last().ToUpper();
                if (!methodPaths.ContainsKey(method))
                {
                    methodPaths[method] = new HashSet<string>();
                }

                var path = "/" + string.Join('/', input.Split('/').Reverse().Skip(1).Reverse());
                methodPaths[method].Add(path);
            }
            string[] request = Console.ReadLine().Split();
            string methodRequired = request[0].ToUpper();
            string pathRequired = request[1];
            string protocol = request[2];

            string successfullMessage = $"{protocol} 200 OK\nContent-Length: 2\nContent-Type: text/plain\n\nOK\n";

            string errorMessage = $"{protocol} 404 NotFound\nContent-Length: 8\nContent-Type: text/plain\n\nNotFound\n";

            if (methodPaths.ContainsKey(methodRequired) && methodPaths[methodRequired].Contains(pathRequired))
            {
                return successfullMessage;
            }
            return errorMessage;
        }

    }
}