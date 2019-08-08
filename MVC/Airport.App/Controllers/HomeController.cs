namespace Airport.App.Controllers
{
    using Airport.App.ViewModels.Home;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using SIS.MVC.Extensions;
    using System.Collections.Generic;
    using System.Linq;

    public class HomeController : BaseController
    {
        [HttpGet("/")]
        [HttpGet]
        public IHttpResponse Index()
        {
            HomeflightOutputDto[] flights = new HomeflightOutputDto[0];
            if (CurentUser!=null&&CurentUser.Role=="Admin")
            {
                flights = DB.Flights.Select(x => x.MapTo<HomeflightOutputDto>()).ToArray();
            }
            else
            {
                flights = DB.Flights.Where(x=>x.PublicFlag).Select(x => x.MapTo<HomeflightOutputDto>()).ToArray();
            }

            ViewData["Flights"] = flights;
            return View();
        }


    }
}