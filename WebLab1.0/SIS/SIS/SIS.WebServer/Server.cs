namespace SIS.WebServer
{
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
        private readonly ServerRoutingTable serverRoutingTable;
        private bool isRunning;

        public Server(int port, ServerRoutingTable serverRoutingTable)
        {
            this.port = port;
            this.serverRoutingTable = serverRoutingTable;
            listener = new TcpListener(IPAddress.Parse(LocalHostIpAddress), port);
        }

        public void Run()
        {
            listener.Start();
            isRunning = true;
            Console.WriteLine($"Server started at http://{LocalHostIpAddress}:{port}");

            var task = Task.Run(ListenLoop);
            Console.WriteLine("kur");    
            task.Wait();
        }

        public async Task ListenLoop()
        {
            while (this.isRunning)
            {
                var client = await this.listener.AcceptSocketAsync();
               
                ConnectionHandler connectionHandler = new ConnectionHandler(client, this.serverRoutingTable);
                Task responseTask = connectionHandler.ProcessRequestAsync();
              
                responseTask.Wait();
            }
        }
    }
}
