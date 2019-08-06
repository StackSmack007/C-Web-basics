namespace Chushka.App.Controllers
{
    using Chushka.App.ViewModels.Home;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using SIS.MVC.Extensions;
    using System.Linq;

    public class HomeController : BaseController
    {
        [HttpGet("/")]
        [HttpGet("/Home/Index")]
        [HttpGet("/Home")]
        public IHttpResponse Index()
        {
            if (CurentUser is null)
            {
                return ViewFilePath("/Home/HomeGuest.html");
            }

            ProductInfoHomeDTO[] products = DB.Products.Select(x => x.MapTo<ProductInfoHomeDTO>()).ToArray();

            ViewData["Products"] = products;

            return ViewFilePath("/Home/HomeLogged.html");
        }
    }
}