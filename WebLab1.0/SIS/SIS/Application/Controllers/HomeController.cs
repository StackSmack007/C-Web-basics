namespace Application.Controllers
{
    using Infrastructure.Models.Models;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class HomeController : BaseController
    {
        private string specialUserPrefix = "Admin";

        [HttpGet("/")]
        [HttpGet("/Home")]
        public IHttpResponse Index()
        {
            if (this.CurentUser is null)
            {
                ViewData["message"] = "No curently loged On user!";
            }
            else
            {
                ViewData["message"] = $"<em>Currently loged on as user: </em><span style=\"color: darkblue\"><strong>{this.CurentUser.UserName}</strong></span>";
            }

            return View();
        }

        [HttpGet("/Home/MyProfile")]
        public IHttpResponse MyProfile()
        {
            if (this.CurentUser is null)
            {
                return this.ControllerError($"No user loged in Log in first");
            }
            User user = db.Users.FirstOrDefault(x => x.Username == this.CurentUser.UserName);
            if (user is null)
            {
                return ControllerError("User not found in the database");
            }

            ViewData["username"] = this.CurentUser.UserName;
            ViewData["registrationDate"] = user.RegisteredOn.ToString("dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture);

            db.Entry(user).Collection(u => u.Orders).Load();
            ViewData["ordersCount"] = user.Orders.Count;

            return View();
        }

        [HttpGet("/Home/AddCake")]
        public IHttpResponse AddCake()
        {
            //Only the usernames starting with Admin can add cakes!
            var loginCookie = Request.Cookies.GetCookie(loginCookieName);
            if (loginCookie is null)
            {
                return ControllerError("Admin must be loged in to add cakes!");
            }
            if (this.CurentUser.UserName.StartsWith(specialUserPrefix))
            {
                return View();
            }
            return ControllerError("User is not authorised to add cakes! His name must start with admin to do that");
        }

        [HttpPost("/Home/AddCakeData")]
        public IHttpResponse AddCakeData()
        {
            string name = this.Request.FormData["cakeName"].ToString();
            decimal price = decimal.Parse(this.Request.FormData["price"].ToString());
            string imgUrl = this.Request.FormData["imgURL"].ToString();
            string manufacturer = this.Request.FormData["manufacturer"].ToString();
            Product existingProduct = db.Products.FirstOrDefault(x => x.ProductName == name);
            if (existingProduct is null)
            {
                Product product = new Product() { ProductName = name, Price = price, ImageURL = imgUrl, ProviderName = manufacturer };
                db.Products.Add(product);
            }
            else
            {
                existingProduct.Price = price;
                existingProduct.ImageURL = imgUrl;
                existingProduct.ProviderName = manufacturer;
            }
            db.SaveChanges();
            RedirectResult("/");
            return this.Response;
        }

        [HttpGet("/Home/Search")]
        public IHttpResponse Search()
        {
            var products = db.Products.Select(x => new { x.Id, x.ProductName, x.Price, x.ProviderName }).OrderByDescending(x => x.Price).ToArray();
            StringBuilder sb = new StringBuilder();

            if (products.Any())
            {
                sb.Append("<table class=\"table table-striped\"><thead class=\"thead-dark\"><tr><th scope=\"col\" style=\"width:30%\">Provider</th><th scope=\"col\" style=\"width:40%\">CakeName</th><th scope=\"col\" style=\"width:30%\">Cake Price (<strong>Euro</strong>)</th></tr></thead><tbody>");
                foreach (var product in products)
                {
                    sb.Append($"<tr><td><i class=\"fa fa-birthday-cake\"></i> {product.ProviderName}</td><td><a href=\"/Home/DisplayCake?id={product.Id}\">{product.ProductName}</a></td><td> {product.Price:F2}<strong> &#8364 </strong></td></tr>");
                }
                sb.Append("</tbody></table>");
            }
            ViewData["products"] = sb.ToString();
            return View();
        }

        [HttpGet("/Home/DisplayCake")]
        public IHttpResponse DisplayCake()
        {
            int productId = int.Parse(this.Request.QueryData["id"].ToString());
            Product product = db.Products.Find(productId);
            ViewData["cakeName"] = product.ProductName;
            ViewData["price"] = product.Price;
            ViewData["imgURL"] = this.DecodeUrl(product.ImageURL);
            ViewData["cakeId"] = product.Id;
            return View();
        }

        [HttpGet("/Home/AboutUs")]
        public IHttpResponse AboutUs()
        {
            return View();
        }
    }
}