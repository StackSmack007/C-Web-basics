namespace MishMashApplication.Controllers
{
    using MishMashApplication.Data;
    using SIS.MVC;

    public abstract class BaseController : Controller
    {
        protected MishMashContext db;

        protected BaseController() : base()
        {
            db = new MishMashContext();
        }
    }
}