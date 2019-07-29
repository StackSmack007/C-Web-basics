namespace MishMashApplication.Controllers
{
    using MishMashApplication.Data;
    using MishMashApplication.Models.Enumerations;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC;
    using System.Linq;

    public abstract class BaseController : Controller
    {
        protected MishMashContext db;

        protected BaseController() : base()
        {
            db = new MishMashContext();
        }

        protected override IHttpResponse View(string layoutName = "_importLayout.html")
        {
            if (CurentUser !=null && IsAdmin())
            {
                layoutName = "_adminLayout.html";
            }
            return base.View(layoutName);
        }

        protected bool IsAdmin()
        {
            if (db.Users.First(x => x.Id == CurentUser.Id).Role == UserRole.Admin)
            {
                return true;
            }
            return false;
        }
    }
}