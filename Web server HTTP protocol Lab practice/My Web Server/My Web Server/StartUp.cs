namespace My_Web_Server
{
    using My_Web_Server.Web_Server;
    using My_Web_Server.Web_Server.Contracts;
    using System.Text.RegularExpressions;

    public class StartUp
    {
        static void Main(string[] args)
        {
     //      string Test = @"GET / HTTP / 1.1\r\nHost: localhost: 1500\r\nConnection: keep - alive\r\nPragma: no - cache\r\nCache - Control: no - cache\r\nUpgrade - Insecure - Requests: 1\r\nUser - Agent: Mozilla / 5.0(Windows NT 6.1; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 75.0.3770.100 Safari / 537.36\r\nAccept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8,application/signed-exchange;v=b3\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: SES_ID=16c37be5-ac46-412a-a374-d62db3c71fef\r\n\r\n";
     //      string pattern = $@"SES_ID=(?<ID>.+)(;|$)";
     //      string sessionId = Regex.Match(Test, pattern).Groups["ID"].Value;


            IWebServer ws = new WebServer();
            ws.Start();
        }
    }
}