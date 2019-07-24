namespace Application.Controllers
{
    using Infrastructure.Data;
    using SIS.MVC;
    using System.Linq;

    public abstract class BaseController : Controller
    {
        protected CakeContext db;

        protected BaseController() : base()
        {
            db = new CakeContext();
                   }

        protected int GetIdOfUserName(string username)
        {
            return db.Users.FirstOrDefault(x => x.Username == username).Id;
        }

    }
}