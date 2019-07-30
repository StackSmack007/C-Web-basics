namespace MishMashApplication.Controllers
{
    using MishMashApplication.Data;
    using MishMashApplication.Models.Enumerations;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC;
    using System.Linq;

    public abstract class BaseController : Controller
    {
        protected MishMashContext Db;

        protected BaseController() : base()
        {
            Db = new MishMashContext();
        }

        protected sealed override IHttpResponse View(string layoutName = "_importLayout.html")
        {
            if (CurentUser != null && IsAdmin() && layoutName == "_importLayout.html")
            {
                layoutName = "_adminLayout.html";
            }
            return base.View(layoutName);
        }

        protected sealed override IHttpResponse ViewFilePath(string subPath, string layoutName = "_importLayout.html")
        {
            if (CurentUser != null && IsAdmin() && layoutName == "_importLayout.html")
            {
                layoutName = "_adminLayout.html";
            }
            return base.ViewFilePath(subPath, layoutName);
        }

        protected bool IsAdmin()
        {
            if (Db.Users.First(x => x.Id == CurentUser.Id).Role == UserRole.Admin)
            {
                return true;
            }
            return false;
        }
    }
}