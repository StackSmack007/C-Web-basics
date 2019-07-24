namespace Application.Controllers
{
    using Application.ViewModels.Orders;
    using Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OrdersController : BaseController
    {
        private static int timeSpanInMinutesToConsiderSameOrder = 10;

        [HttpPost("/Orders/MakeOrder")]
        public IHttpResponse MakeOrder(ProductDto product)
        {
            if (this.CurentUser is null)
            {
                return ControllerError("Guests are not authorised to make purchases", @"/Authentication/LogIn", "LogIn");
            }
            User buyer = db.Users.Include(x => x.Orders).ThenInclude(x => x.OrderProducts)
                .FirstOrDefault(x => x.Username == this.CurentUser.UserName);
            if (buyer is null) return ControllerError("User not found in database");

            DateTime nowTIme = DateTime.UtcNow;
            Order currentOrder = buyer.Orders.FirstOrDefault(x => x.DateOfCreation > nowTIme.AddMinutes(-timeSpanInMinutesToConsiderSameOrder));

            if (currentOrder != null && currentOrder.OrderProducts.Any(x => x.ProductID == product.ProductId))
            {
                currentOrder.OrderProducts.First(x => x.ProductID == product.ProductId).Quantity += product.Quantity;
            }
            else if (currentOrder is null)
            {
                currentOrder = new Order()
                {
                    UserId = buyer.Id,
                    DateOfCreation = DateTime.UtcNow,
                    OrderProducts = new HashSet<OrderProduct> {new OrderProduct()
                {
                    ProductID = product.ProductId,
                    Quantity = product.Quantity
                }
                    }
                };
                db.Orders.Add(currentOrder);
            }
            else
            {
                currentOrder.OrderProducts.Add(new OrderProduct()
                {
                    ProductID = product.ProductId,
                    Quantity = product.Quantity
                });
            }
            db.SaveChanges();
            string userName = this.CurentUser.UserName;
            string cakeName = db.Products.First(x => x.Id == product.ProductId).ProductName;
            return ControllerSuccess($"Success: User {userName} ordered {product.Quantity} pieces of cake {cakeName}", "/Cakes/Browse", "Browse Cakes");
        }

        [HttpGet("/Orders/DisplayOrders")]
        public IHttpResponse DisplayOrders()
        {
            if (this.CurentUser is null)
            {
                return this.ControllerError($"No user loged in Log in first");
            }

            var userOrders = db.Orders
                .Where(x => x.User.Username == this.CurentUser.UserName)
                .OrderByDescending(x => x.DateOfCreation)
                .Select(x => new OrderDto_exp
                {
                    OrderId = x.Id,
                    CreatedOn = x.DateOfCreation,
                    Products = x.OrderProducts.Select(y => new ProductDto() { SinglePrice = y.Product.Price, Quantity = y.Quantity }).ToList()
                }).ToArray();
            ViewData["Username"] = this.CurentUser.UserName;
            ViewData["Orders"] = userOrders;

            return View();
        }

        [HttpGet("/Orders/DisplayOrder")]
        public IHttpResponse DisplayOrder(string id)
        {
            int orderId = int.Parse(id);
            Order order = null;
            try
            {
                order = db.Orders.Include(o => o.User).Include(o => o.OrderProducts)
                                 .ThenInclude(op => op.Product).First(x => x.Id == orderId);

                if (order.User.Username != this.CurentUser.UserName)
                {
                    return ControllerError($"User {this.CurentUser.UserName} is not outhorised to view another user's orders");
                }
            }
            catch (Exception)
            {
                return ControllerError("Invalid OrderId in the link");
            }
           
            ViewData["Order"] = new OrderDto_exp
                ()
            {
                OrderId = orderId,
                Products = order.OrderProducts.OrderByDescending(x => x.Quantity * x.Product.Price).Select(x => new ProductDto()
                {
                    ProductId = x.ProductID,
                    ProductName = x.Product.ProductName.Replace("+", " "),
                    Quantity = x.Quantity,
                    SinglePrice = x.Product.Price
                }).ToArray()

            };
            return View();
        }
    }
}