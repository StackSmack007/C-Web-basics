namespace Panda.App.Controllers
{
    using Panda.App.ViewModels.Home;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
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
                return ViewFilePath("/Home/IndexGuest.html");
            }

            var packagesOfUser = DB.Packages.Where(x => x.UserId == CurentUser.Id && x.Status != Infrastructure.Models.Enums.Status.Acquired)
                .Select(x => new packageIdIndexDto(x.Id, x.Description, x.Status)).ToArray();

             ViewData["Packages"] = packagesOfUser;

            return ViewFilePath("/Home/IndexUser.html");
        }
    }
}