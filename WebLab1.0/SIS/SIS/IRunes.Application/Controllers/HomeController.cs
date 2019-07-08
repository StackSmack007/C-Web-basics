namespace IRunes.Application.Controllers
{
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;

    public class HomeController : BaseController
    {
        public IHttpResponse Index(IHttpRequest request)
        {
            if (IsUserLogedIn(request))
            {
                ViewData["userName"] = this.GetCurrentSessionUserIdandName(request)[1];
                return View("IndexUser");
            }
            return View("IndexGuest");
        }
    }
}