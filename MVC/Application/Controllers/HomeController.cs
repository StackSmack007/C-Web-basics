namespace Application.Controllers
{
    using Application.ViewModels.Home;
    using Infrastructure.Models.Models;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System.Globalization;
    using System.Linq;

    public class HomeController : BaseController
    {
        [HttpGet]
        [HttpGet("/")]
        [HttpGet("/Home")]
        public IHttpResponse Index()
        {
            return View();
        }

        public IHttpResponse MyProfile()
        {
            if (this.CurentUser is null)
            {
                return this.MessageError($"No user loged in Log in first");
            }
            User user = db.Users.FirstOrDefault(x => x.Username == this.CurentUser.UserName);
            if (user is null)
            {
                return MessageError("User not found in the database");
            }
            db.Entry(user).Collection(u => u.Orders).Load();
            this.ViewData["Profile"] = new ProfileDto_exp()
            {
                Username = this.CurentUser.UserName,
                RegisteredOn = user.RegisteredOn.ToString("dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture),
                OrdersCount = user.Orders.Count
            };
            return View();
        }

        public IHttpResponse AboutUs()
        {
            return View();
        }
    }
}