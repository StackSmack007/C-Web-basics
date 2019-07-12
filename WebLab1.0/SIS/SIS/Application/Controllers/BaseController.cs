namespace Application.Controllers
{
    using Infrastructure.Data;
    using SIS.HTTP.Requests.Contracts;
    using SIS.MVC;
    using System;
    using System.Linq;

    public abstract class BaseController : Controller
    {

        protected CakeContext db;


        protected BaseController() : base()
        {
            db = new CakeContext();
        }


        protected bool VerifyMemberCookie(IHttpRequest request)
        {
            string cookieValue = request.Cookies.GetCookie(loginCookieName).Value;

            string userName = string.Empty;
            try
            {
                userName = encrypter.Decrypt(cookieValue);
            }
            catch (Exception)
            {
                return false;
            }
            return db.Users.Any(x => x.Username == userName);
        }


    }
}