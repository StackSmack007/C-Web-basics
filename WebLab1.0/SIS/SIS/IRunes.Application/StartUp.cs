namespace IRunes.Application
{
    using IRunes.Application.Controllers;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class StartUp
    {
        private static ServerRoutingTable serverRoutingTable;
        static void Main(string[] args)
        {
            InitializeRouting();
            Server server = new Server(3000, serverRoutingTable);
            server.Run();
        }

        private static void InitializeRouting()
        {
            serverRoutingTable = new ServerRoutingTable();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/Index"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/About"] = request => new HomeController().About(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Users/Register"] = request => new UsersController().Register(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Users/RegisterData"] = request => new UsersController().RegisterData(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Users/Login"] = request => new UsersController().Login(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Users/LoginData"] = request => new UsersController().LoginData(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Users/Logout"] = request => new UsersController().Logout(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Albums/All"] = request => new AlbumsController().All(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Albums/Create"] = request => new AlbumsController().Create(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Albums/CreateData"] = request => new AlbumsController().CreateData(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Albums/Details"] = request => new AlbumsController().Details(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Albums/Delete"] = request => new AlbumsController().Delete(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Tracks/Create"] = request => new TracksController().Create(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Tracks/CreateData"] = request => new TracksController().CreateData(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Tracks/Details"] = request => new TracksController().Details(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Tracks/Detach"] = request => new TracksController().DetachTrackFromAlbum(request);
        }
    }
}