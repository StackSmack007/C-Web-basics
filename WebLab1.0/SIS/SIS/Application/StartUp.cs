namespace Application
{
    using Application.Controllers;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class StartUp
    {
        public static void Main()
        {
            //  var encrypter = new Encrypter();
            //  string attemptToDecriptRandom = encrypter.Decrypt("Dupeshink11");


            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/home/"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/MyProfile"] = request => new HomeController().MyProfile(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/AboutUs"] = request => new HomeController().AboutUs();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/AddCake"] = request => new HomeController().AddCake(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Home/AddCakeData"] = request => new HomeController().AddCakeData(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/Search"] = request => new HomeController().Search();

            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/DisplayCake"] = request => new HomeController().DisplayCake(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Orders/DisplayOrders"] = request => new OrdersController().DisplayOrders(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Orders/DisplayOrder"] = request => new OrdersController().DisplayOrder(request);


            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Authentication/Register"] = request => new AuthenticationController().Register();
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Authentication/Register"] = request => new AuthenticationController().Register(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Authentication/LogOf"] = request => new AuthenticationController().LogOf(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Authentication/LogIn"] = request => new AuthenticationController().LogIn(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Authentication/LogInData"] = request => new AuthenticationController().LogInData(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Orders/MakeOrder"] = request => new OrdersController().MakeOrder(request);

            Server server = new Server(2000, serverRoutingTable);
            server.Run();

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
