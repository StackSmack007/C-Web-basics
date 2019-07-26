namespace SIS.WebServer
{
    using SIS.WebServer.Api;
    using SIS.WebServer.Api.Contracts;
    using SIS.WebServer.Routing;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    public class Server
    {
        private const string LocalHostIpAddress = "127.0.0.1";
        private readonly int port;
        private readonly TcpListener listener;

        private readonly IHttpHandler handler;

        private bool isRunning;

        public Server(int port, ServerRoutingTable serverRoutingTable)
        {
            this.port = port;
            listener = new TcpListener(IPAddress.Parse(LocalHostIpAddress), port);
            handler = new HttpHandler(serverRoutingTable);
        }

        public Server(int port, IHttpHandler handler)
        {
            this.port = port;
            listener = new TcpListener(IPAddress.Parse(LocalHostIpAddress), port);
            this.handler = handler;
        }

        public void Run()
        {
            listener.Start();
            isRunning = true;
            Console.WriteLine($"Server started at http://{LocalHostIpAddress}:{port}");
            ListenLoop();
        }

        public async Task ListenLoop()
        {
            while (this.isRunning)
            {
                var client = listener.AcceptSocket();
                ConnectionHandler connectionHandler = new ConnectionHandler(client, handler);
                connectionHandler.ProcessRequestAsync();
            }
        }
    }
}