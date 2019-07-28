using SIS.HTTP.Responses.Contracts;
using SIS.MVC.Attributes;

namespace MishMashApplication.Controllers
{
   public class HomeController:BaseController
    {
        [HttpGet("/")]
        [HttpGet("/Home/")]
        [HttpGet("/Home/Index")]
        [HttpGet("/Index")]
        public IHttpResponse Index()
        {
            return this.View();
        }






    }
}
