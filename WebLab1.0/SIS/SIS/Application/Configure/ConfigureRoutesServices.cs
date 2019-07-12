namespace Application.Configure
{
    using Application.Controllers;
    using SIS.HTTP.Enums;
    using SIS.MVC.Contracts;
    using SIS.WebServer.Routing;
    public class ConfigureRoutesServices : IConfiguratorRoutes_Services
    {
        public void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        {
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/"] = request => new HomeController() { Request = request }.Index();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/home/"] = request => new HomeController() { Request = request }.Index();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/"] = request => new HomeController() { Request = request }.Index();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home"] = request => new HomeController() { Request = request }.Index();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/MyProfile"] = request => new HomeController() { Request = request }.MyProfile();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/AboutUs"] = request => new HomeController() { Request = request }.AboutUs();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/AddCake"] = request => new HomeController() { Request = request }.AddCake();
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Home/AddCakeData"] = request => new HomeController() { Request = request }.AddCakeData();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/Search"] = request => new HomeController() { Request = request }.Search();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Home/DisplayCake"] = request => new HomeController() { Request = request }.DisplayCake();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Orders/DisplayOrders"] = request => new OrdersController() { Request = request }.DisplayOrders();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Orders/DisplayOrder"] = request => new OrdersController() { Request = request }.DisplayOrder();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Authentication/Register"] = request => new AuthenticationController() { Request = request }.Register();
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Authentication/RegisterData"] = request => new AuthenticationController() { Request = request }.RegisterData();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Authentication/LogOf"] = request => new AuthenticationController() { Request = request }.LogOf();
            serverRoutingTable.Routes[HttpRequestMethod.Get][@"/Authentication/LogIn"] = request => new AuthenticationController() { Request = request }.LogIn();
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Authentication/LogInData"] = request => new AuthenticationController() { Request = request }.LogInData();
            serverRoutingTable.Routes[HttpRequestMethod.Post][@"/Orders/MakeOrder"] = request => new OrdersController() { Request = request }.MakeOrder();
        }

        public void ConfigureServices()
        {

        }
    }
}
