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
        private string specialUserPrefix = "Admin";

        [HttpGet("/")]
        [HttpGet("/Home")]
        public IHttpResponse Index()
        {
            ViewModel = this.CurentUser is null ? null : this.CurentUser.UserName;
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
            db.Entry(user).Collection(u => u.Orders).Load();
            this.ViewModel = new ProfileDto_exp()
            {
                Username = this.CurentUser.UserName,
                RegisteredOn = user.RegisteredOn.ToString("dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture),
                OrdersCount = user.Orders.Count
            };
            return View();
        }

        [HttpGet("/Home/AddCake")]
        public IHttpResponse AddCake()
        {
            //Only the usernames starting with Admin can add cakes!
            if (CurentUser is null)
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
        public IHttpResponse AddCakeData(CakeDto newCake)
        {
            Product existingProduct = db.Products.FirstOrDefault(x => x.ProductName == newCake.CakeName);
           
            if (existingProduct is null)
            {
                Product product = new Product() { ProductName = newCake.CakeName, Price = newCake.Price, ImageURL = newCake.ImgUrl, ProviderName = newCake.Manufacturer };
                db.Products.Add(product);
            }
            else
            {
                existingProduct.Price = newCake.Price;
                existingProduct.ImageURL = newCake.ImgUrl;
                existingProduct.ProviderName = newCake.Manufacturer;
            }
            db.SaveChanges();
            RedirectResult("/");
            return this.Response;
        }

        [HttpGet("/Home/Search")]
        public IHttpResponse Search()
        {
            CakeDto[] products = db.Products.Select(x => new CakeDto()
            {
                CakeId = x.Id,
                CakeName = x.ProductName.Replace("+"," "),
                Price = x.Price,
                Manufacturer = x.ProviderName.Replace("+", " ")
            }).OrderByDescending(x => x.Price).ToArray();

            ViewModel = products;

            return View();
        }

        [HttpGet("/Home/DisplayCake")]
        public IHttpResponse DisplayCake(int id)
        {
            Product product = db.Products.Find(id);
            var resultDto = new CakeDto(
                product.ProductName.Replace("+", " "),
                product.Price,
                this.DecodeUrl(product.ImageURL),
                product.ProviderName.Replace("+", " "),
                product.Id
                );
            ViewModel = resultDto;
            return View();
        }

        [HttpGet("/Home/AboutUs")]
        public IHttpResponse AboutUs()
        {
            return View();
        }
    }
}