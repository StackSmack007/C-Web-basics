namespace Application
{
    using Application.Controllers;
    using Application.Services;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
    using System;

  public class StartUp
    {
        public static void Main()
        {
         

            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/"] = request => new HomeController().Index();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/home/"] = request => new HomeController().Index();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Register"] = request => new HomeController().Register();

            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Register"] = request => new HomeController().Register(request);
            Server server = new Server(2000, serverRoutingTable);
            server.Run();







            Console.WriteLine("Hello World!");
        }

        //  public static void  TestMethod()
        //  {
        //      StackTrace stackTrace = new StackTrace();
        //      MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
        //      string controllerName = methodBase.Name;
        //      string className = methodBase.ReflectedType.Name;
        //  }

    }

}
