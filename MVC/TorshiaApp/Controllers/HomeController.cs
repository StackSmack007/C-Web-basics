using SIS.HTTP.Responses.Contracts;
using SIS.MVC.Attributes;
using TorshiaApp.Models.Enumerations;

namespace TorshiaApp.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet("/")]
        [HttpGet("/Home")]
        [HttpGet("/Home/Index")]
        public IHttpResponse Index()
        {
            UserRole curentRole = GetRole();
            if (curentRole is UserRole.Guest)
            {
                return ViewFilePath("/Home/IndexGuest.html");
            }
            ViewData["Status"] = curentRole.ToString();                 

            return ViewFilePath("/Home/IndexAdminUser.html");
        }
    }
}