namespace Panda.App.Controllers
{
    using Panda.Infrastructure.Data;
    using SIS.MVC;
    public abstract class BaseController : Controller
    {
        protected PandaContext DB { get; set; }

        public BaseController()
        {
            DB = (PandaContext)WebHost.ServiceContainer.CreateInstance(typeof(PandaContext));
        }
    }
}