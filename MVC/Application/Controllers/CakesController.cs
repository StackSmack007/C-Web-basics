namespace Application.Controllers
{
    using Application.ViewModels.Home;
    using Infrastructure.Models.Models;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System.Linq;
    public class CakesController:BaseController
    {
        private string specialUserPrefix = "Admin";

        [HttpGet("/Cakes/Browse")]
        public IHttpResponse Browse()
        {
            CakeDto[] products = db.Products.Select(x => new CakeDto()
            {
                CakeId = x.Id,
                CakeName = x.ProductName.Replace("+", " "),
                Price = x.Price,
                Manufacturer = x.ProviderName.Replace("+", " ")
            }).OrderByDescending(x => x.Price).ToArray();

            ViewData["Products"] = products;

            return View();
        }

        [HttpGet("/Cakes/AddCake")]
        public IHttpResponse AddCake()
        {
            //Only the usernames starting with Admin can add cakes!
            if (CurentUser is null)
            {
                return MessageError("Admin must be loged in to add cakes!");
            }
            if (this.CurentUser.UserName.StartsWith(specialUserPrefix))
            {
                return View();
            }
            return MessageError("User is not authorised to add cakes! His name must start with admin to do that");
        }

        [HttpPost("/Cakes/AddCakeData")]
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

        [HttpGet("/Cakes/DisplayCake")]
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
            ViewData["Cake"] = resultDto;
            return View();
        }

        [HttpGet("/Cakes/Search")]
        public IHttpResponse Search(string searchPhrase)
        {
            var cakesFound = db.Products.Where(x => x.ProductName.ToLower().Contains(searchPhrase.ToLower())).Select(x => new CakeDto
            {
                CakeId = x.Id,
                CakeName = x.ProductName.Replace("+", " "),
                Price = x.Price,
                Manufacturer = x.ProviderName.Replace("+", " ")
            }).OrderByDescending(x => x.Price).ToArray();

            ViewData["Products"] = cakesFound;
            ViewData["SearchPhrase"] = searchPhrase;
            return View();
        }
    }
}
