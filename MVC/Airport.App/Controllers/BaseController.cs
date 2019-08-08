namespace Airport.App.Controllers
{
using Airport.Infrastructure.Data;
using SIS.MVC;
    public abstract  class BaseController: Controller
    {
        protected AirportContext DB { get;}
        public BaseController()
        {
            DB = (AirportContext)WebHost.ServiceContainer.CreateInstance(typeof(AirportContext));
        }
    }
}