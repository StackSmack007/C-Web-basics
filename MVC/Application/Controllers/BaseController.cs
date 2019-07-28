namespace Application.Controllers
{
    using Infrastructure.Data;
    using SIS.MVC;

    public abstract class BaseController : Controller
    {
        protected CakeContext db;

        protected BaseController() : base()
        {
            db = new CakeContext();

        }
    }
}