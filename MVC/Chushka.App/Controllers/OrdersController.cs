namespace Chushka.App.Controllers
{
    using Chushka.App.ViewModels.Orders;
    using Chushka.Models.Models;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System;
    using System.Globalization;
    using System.Linq;
    public class OrdersController:BaseController
    {
        [Authorised]
        public IHttpResponse Create(int id)
        {
            var product = DB.Products.FirstOrDefault(x => x.Id == id);
            if (product is null)
            {
                return MessageError("Product not existing");
            }

            product.Orders.Add(new Order()
            {
                UserId = CurentUser.Id,
                OrderedOn = DateTime.Now
            });
            DB.SaveChanges();
            return RedirectResult("/Home/Index");
        }

        [Authorised]
        public IHttpResponse All()
        {
            if (CurentUser.Role != "Admin")
            {
                return MessageError("Only Admins can edit products!");
            }

            ordersDisplayAllDTO[] orders = DB.Orders.Select(x => new ordersDisplayAllDTO
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Customer = x.Client.FullName,
                OrderedOn = x.OrderedOn.ToString("HH:mm dd/MM/yyyy", CultureInfo.InvariantCulture)
            }).ToArray();

            ViewData["Orders"] = orders;
            DB.SaveChanges();
            return View();
        }
    }
}