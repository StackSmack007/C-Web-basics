namespace Chushka.App.Controllers
{
    using Chushka.Data;
    using SIS.MVC;
    public abstract class BaseController : Controller
    {
        protected ChushkaContext DB { get; set; }

        public BaseController()
        {
            DB = (ChushkaContext)WebHost.ServiceContainer.CreateInstance(typeof(ChushkaContext));
        }
    }
}