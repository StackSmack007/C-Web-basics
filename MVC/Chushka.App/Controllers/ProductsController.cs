namespace Chushka.App.Controllers
{
    using Chushka.App.ViewModels.Products;
    using Chushka.Models.Models;
    using Chushka.Models.Models.Enumerations;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System;
    using System.Linq;

    public class ProductsController : BaseController
    {
        [Authorised]
        public IHttpResponse Details(int id)
        {
            productDTO product = DB.Products.Where(x => x.Id == id)
                               .Select(x => new productDTO
                               {
                                   Id = x.Id,
                                   Name = x.Name,
                                   Price = x.Price,
                                   Description = x.Description,
                                   Type = x.Type.ToString()
                               })
                               .FirstOrDefault();

            if (product is null)
            {
                return MessageError("Product not existing");
            }

            ViewData["Product"] = product;
            return View();
        }

        [Authorised]
        public IHttpResponse Create()
        {
            if (CurentUser.Role != "Admin")
            {
                return MessageError("Only Admins can create products!");
            }
            return View();
        }

        [HttpPost]
        [Authorised]
        public IHttpResponse Create(productDTO pr)
        {
            if (DB.Products.Any(x => x.Name == pr.Name))
            {
                return MessageWithView("Product name already used!");
            }
            Product newProduct = MakeProduct(pr);
            DB.Products.Add(newProduct);
            DB.SaveChanges();
            return RedirectResult("/Home/Index");
        }

        [Authorised]
        public IHttpResponse Edit(int id)
        {
            productDTO product = DB.Products.Where(x => x.Id == id).Select(x => new productDTO
            {
                Id = id,
                Name = x.Name.Replace("+", " "),
                Price = x.Price,
                Description=x.Description,
                Type = x.Type.ToString()
            }
                ).FirstOrDefault();

            if (product is null)
            {
                return MessageError("Product not existing");
            }
            if (CurentUser.Role != "Admin")
            {
                return MessageError("Only Admins can edit products!");
            }
            ViewData["Product"] = product;
            return View();
        }

        [HttpPost]
        [Authorised]
        public IHttpResponse Edit(productDTO pr)
        {
            var foundProduct = DB.Products.FirstOrDefault(x => x.Id == pr.Id);
            foundProduct.Name = pr.Name;
            foundProduct.Price = pr.Price;
            foundProduct.Description = pr.Description;
            foundProduct.Type = Enum.Parse<ProductType>(pr.Type);
            DB.SaveChanges();
            return RedirectResult("/Home/Index");
        }

        private Product MakeProduct(productDTO pr)
        {
            Product newProduct = new Product
            {
                Name = pr.Name,
                Price = pr.Price,
                Description = pr.Description,
                Type = Enum.Parse<ProductType>(pr.Type)
            };
            return newProduct;
        }



        [Authorised]
        public IHttpResponse Delete(int id)
        {
            productDTO product = DB.Products.Where(x => x.Id == id).Select(x => new productDTO
            {
                Id = id,
                Name = x.Name.Replace("+", " "),
                Price = x.Price,
                Description = x.Description.Replace("+", " "),
                Type = x.Type.ToString()
            }
                ).FirstOrDefault();

            if (product is null)
            {
                return MessageError("Product not existing");
            }
            if (CurentUser.Role != "Admin")
            {
                return MessageError("Only Admins can delete products!");
            }
            ViewData["Product"] = product;
            return View();
        }

        [HttpPost]
        [Authorised]
        public IHttpResponse Delete(uint id)
        {
            var product = DB.Products.FirstOrDefault(x => x.Id == id);

            if (product is null)
            {
                return MessageError("Product not existing");
            }
            if (CurentUser.Role != "Admin")
            {
                return MessageError("Only Admins can delete products!");
            }

            DB.Orders.RemoveRange(DB.Orders.Where(x => x.ProductId == id).ToArray());
            DB.Products.Remove(product);
            DB.SaveChanges();
            return RedirectResult("/Home/Index");
        }

    }
}