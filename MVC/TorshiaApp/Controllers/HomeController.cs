using Microsoft.EntityFrameworkCore;
using SIS.HTTP.Responses.Contracts;
using SIS.MVC.Attributes;
using SIS.MVC.Extensions;
using System.Linq;
using TorshiaApp.DTO.Home;
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
            ViewData["UserStatus"] = curentRole.ToString();

            int currentUserId = CurentUser.Id;
            HomeTaskDTO[] tasksDTO = DB.Tasks.Include(x=>x.AffectedSectors).Where(x => !x.Reports.Any(r => r.reporterId == CurentUser.Id))
                                                                    .Select(x => x.MapTo<HomeTaskDTO>()).ToArray();
            ViewData["Tasks"] = tasksDTO;

            return ViewFilePath("/Home/IndexAdminUser.html");
        }
    }
}