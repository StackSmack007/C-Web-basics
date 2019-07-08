namespace Application.Controllers
{
    using Infrastructure.Models.Models;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class HomeController : BaseController
    {
        private string specialUserPrefix = "Admin";
        public IHttpResponse Index(IHttpRequest request)
        {
            string username = GetUserNameFromCookie(request.Cookies.GetCookie(loginCookieName));
            if (username is null)
            {
                ViewData["message"] = "No curently loged On user!";
            }
            else
            {
                ViewData["message"] = $"<em>Currently loged on as user: </em><span style=\"color: darkblue\"><strong>{username}</strong></span>";
            }

            return View();
        }

        public IHttpResponse MyProfile(IHttpRequest request)
        {
            string username = GetUserNameFromCookie(request.Cookies.GetCookie(loginCookieName));


            if (username is null)
            {
                return this.ControllerError($"No user loged in Log in first");
            }
            User user = db.Users.FirstOrDefault(x => x.Username == username);
            if (user is null)
            {
                return ControllerError("User not found in the database");
            }

            ViewData["username"] = username;
            ViewData["registrationDate"] = user.RegisteredOn.ToString("dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture);

            db.Entry(user).Collection(u => u.Orders).Load();
            ViewData["ordersCount"] = user.Orders.Count;

            return View();
        }

        public IHttpResponse AddCake(IHttpRequest request)
        {
            //Only the usernames starting with Admin can add cakes!

            var loginCookie = request.Cookies.GetCookie(loginCookieName);
            if (loginCookie is null)
            {
                return ControllerError("Admin must be loged in to add cakes!");
            }
            if (GetUserNameFromCookie(loginCookie).StartsWith(specialUserPrefix))
            {
                return View();
            }
            return new HomeController().Index(request);
        }

        public IHttpResponse AddCakeData(IHttpRequest request)
        {
            string name = request.FormData["cakeName"].ToString();
            decimal price = decimal.Parse(request.FormData["price"].ToString());
            string imgUrl = request.FormData["imgURL"].ToString();

            Product existingProduct = db.Products.FirstOrDefault(x => x.ProductName == name);
            if (existingProduct is null)
            {
                Product product = new Product() { ProductName = name, Price = price, ImageURL = imgUrl };
                db.Products.Add(product);
            }
            else
            {
                existingProduct.Price = price;
                existingProduct.ImageURL = imgUrl;
            }
            db.SaveChanges();
            return new HomeController().Index(request);
        }

        public IHttpResponse Search()
        {
            var products = db.Products.Select(x => new { x.Id, x.ProductName, x.Price }).OrderByDescending(x => x.Price).ToArray();
            StringBuilder sb = new StringBuilder();

            foreach (var product in products)
            {
                sb.Append($"<p><a href=\"/Home/DisplayCake?id={product.Id}\" >{ product.ProductName}</a> ${product.Price:F2}</p>");
            }
            ViewData["products"] = sb.ToString();
            return View();
        }

        public IHttpResponse DisplayCake(IHttpRequest request)
        {
            int productId = int.Parse(request.QueryData["id"].ToString());
            Product product = db.Products.Find(productId);
            ViewData["cakeName"] = product.ProductName;
            ViewData["price"] = product.Price;
            ViewData["imgURL"] = this.DecodeUrl(product.ImageURL);
            ViewData["cakeId"] = product.Id;               
            return View();
        }

        public IHttpResponse AboutUs()
        {
            return View();
        }


    }
}