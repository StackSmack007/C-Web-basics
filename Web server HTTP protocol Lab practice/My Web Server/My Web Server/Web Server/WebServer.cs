namespace My_Web_Server.Web_Server
{
    using My_Web_Server.Web_Server.Contracts;
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class WebServer : IWebServer
    {
        private readonly TcpListener tcpListener;
        private readonly RequestProcessor requestProcessor;
        private bool IsRunning;
        public WebServer(string ip, int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse(ip), port);
            this.requestProcessor = new RequestProcessor();
        }
        public WebServer() : this("127.0.0.1", 1500) { }


        public void Start()
        {
            Console.WriteLine("Web Server Has Started....");
            tcpListener.Start();
            IsRunning = true;
            while (IsRunning)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                requestProcessor.ProcessClient(client);
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }

    public class RequestProcessor
    {
        public async Task ProcessClient(TcpClient client)
        {
            byte[] buffer = new byte[10240];
            var stream = client.GetStream();
            int readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            string requestString = Encoding.UTF8.GetString(buffer, 0, readBytes);
            Console.WriteLine(new string('=', 30));

            Console.WriteLine(requestString);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK");


            Session session = SessionManager.ExtractSessionFromRequest(requestString);

            if (!SessionManager.ContainsSession(requestString,session.Id))
            {
                sb.AppendLine(SessionManager.SetSesion(session));
            }
            string timeFormat = "d-MM-yyyy : H: mm: ss";
            string responseText = $@"<h1>What is Lorem Ipsum ?</ h1 ><p>User Created Session at : {session.FirstCreated.ToString(timeFormat, CultureInfo.InvariantCulture)}</p>
                                    <p>User Session will end at : {session.EndDate.ToString(timeFormat, CultureInfo.InvariantCulture)}</p>
                                    <p>User Has loged in : {session.TimesLogedIn} times so far...</p>";

            sb.AppendLine("Content-Length: " + responseText.Length);
            sb.AppendLine("Content-Type: text/html");
            sb.AppendLine();
            sb.AppendLine(responseText);
            byte[] responceBytes = Encoding.UTF8.GetBytes(sb.ToString());
            sb.Clear();

            await stream.WriteAsync(responceBytes);
            #region otherOptionForSending
            // string respContent = File.ReadAllText("../../../Web Server/Responses/resp2.html");
            // var imgContent = File.ReadAllBytes("../../../Web Server/Responses/apple.png");
            // var imgstring = Encoding.Unicode.GetString(imgContent);
            // sb.AppendLine("HTTP/1.1 200 PEACHES");
            // sb.AppendLine("Content-Disposition: attachment;filename=pic.png");
            // sb.AppendLine("Content-Type: img/png");
            // sb.AppendLine("Content-Length: " + imgstring.Length);
            // sb.AppendLine();
            // sb.AppendLine(imgstring);
            // var responceBytes = Encoding.Unicode.GetBytes(sb.ToString());
            #endregion
        }




    }
}
