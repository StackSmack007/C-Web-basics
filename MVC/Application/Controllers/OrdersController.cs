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

        [HttpPost("/Orders/AddToOrder")]
        public IHttpResponse AddToOrder(ProductDto product)
        {
            int productId = product.ProductId;
            int quantity = product.Quantity;

            if (this.CurentUser is null)
            {
                return MessageError("Guests are not authorised to make purchases", @"/Authentication/LogIn", "LogIn");//Redundant but still
            }
            User buyer = db.Users.Include(x => x.Orders).ThenInclude(x => x.OrderProducts)
                .FirstOrDefault(x => x.Username == this.CurentUser.Username);
            if (buyer is null) return MessageError("User not found in database");//Redundant but still

            Order currentOrder = buyer.Orders.OrderBy(x => x.Id).LastOrDefault();

            if (currentOrder != null && currentOrder.OrderProducts.Any(x => x.ProductID == product.ProductId))
            {
                currentOrder.OrderProducts.First(x => x.ProductID == product.ProductId).Quantity += product.Quantity;
            }
            else if (currentOrder is null)
            {
                currentOrder = new Order
                {
                    UserId = buyer.Id,
                    DateOfCreation = DateTime.UtcNow,
                    OrderProducts = new HashSet<OrderProduct> {new OrderProduct
                                                                              {
                                                                                  ProductID = product.ProductId,
                                                                                  Quantity = product.Quantity
                                                                              } }
                };
                db.Orders.Add(currentOrder);
            }
            else//there is current order but there are no such products in it
            {
                currentOrder.OrderProducts.Add(new OrderProduct()
                {
                    ProductID = product.ProductId,
                    Quantity = product.Quantity
                });
            }
            db.SaveChanges();
            string userName = this.CurentUser.Username;
            string cakeName = db.Products.First(x => x.Id == product.ProductId).ProductName;
            return MessageSuccess($"Success: User {userName} ordered {product.Quantity} pieces of cake {cakeName}", "/Cakes/Browse", "Browse Cakes");
        }

        [HttpGet("/Orders/DisplayOrders")]
        public IHttpResponse DisplayOrders()
        {
            if (this.CurentUser is null)
            {
                return this.MessageError($"No user loged in Log in first");
            }

            var userOrders = db.Orders
                .Where(x => x.UserId == this.CurentUser.Id)
                .OrderBy(x => x.Id)
                .Select(x => new OrderDto_exp
                {
                    OrderId = x.Id,
                    CreatedOn = x.DateOfCreation,
                    Products = x.OrderProducts.Select(y => new ProductDto() { SinglePrice = y.Product.Price, Quantity = y.Quantity }).ToList()
                }).ToArray();
            ViewData["Username"] = this.CurentUser.Username;
            ViewData["Orders"] = userOrders;

            return View();
        }

        [HttpPost("/Orders/FinaliseOrder")]
        public IHttpResponse FinaliseOrder(int orderId)
        {
            var order = db.Orders.Where(x=>x.UserId==CurentUser.Id).Include(x => x.OrderProducts).Last();
            if (order.Id != orderId)
            {
                return MessageError("Order is not cart, Order already finished!");
            }
            if (order.UserId != this.CurentUser.Id)
            {
                return MessageError("This User is not authorised to complete this order!");
            }
            if (!order.OrderProducts.Any())
            {
                return MessageError("You can not submit empty Order!");//redundant but still
            }
            order.DateOfCreation = DateTime.UtcNow;
            db.Orders.Add(new Order
            {
                UserId = CurentUser.Id,
                DateOfCreation = DateTime.UtcNow
            });
            db.SaveChanges();
            return MessageSuccess($"Successfully added new order to user {CurentUser.Username}", "/Orders/DisplayOrders", $"{CurentUser.Username}'s Orders");
        }

        [HttpGet("/Orders/DisplayOrder")]
        public IHttpResponse DisplayOrder(string id, bool isCart)
        {
            int orderId = int.Parse(id);
            Order order = null;
            try
            {
                order = db.Orders.Include(o => o.User).Include(o => o.OrderProducts)
                                 .ThenInclude(op => op.Product).First(x => x.Id == orderId);

                if (order.User.Username != this.CurentUser.Username)
                {
                    return MessageError($"User {this.CurentUser.Username} is not outhorised to view another user's orders");
                }
            }
            catch (Exception)
            {
                return MessageError("Invalid OrderId in the link");
            }
            ViewData["IsCart"] = isCart;
            ViewData["Order"] = new OrderDto_exp()
            {
                OrderId = orderId,
                Products = order.OrderProducts.OrderByDescending(x => x.Quantity * x.Product.Price).Select(x => new ProductDto()
                {
                    ProductId = x.ProductID,
                    ProductName = x.Product.ProductName,
                    Quantity = x.Quantity,
                    SinglePrice = x.Product.Price
                }).ToArray()

            };
            return View();
        }

        [HttpPost("/Orders/DeleteOrderProducts")]
        public IHttpResponse DeleteOrderProducts(int orderId)
        {
            var order = db.Orders.Where(x => x.UserId == CurentUser.Id).Include(x => x.OrderProducts).Last();
            if (order.Id != orderId)
            {
                return MessageError("Order is not cart it is finished!");
            }
            if (order.UserId != this.CurentUser.Id)
            {
                return MessageError("This User is not authorised to complete this order!");
            }
            if (!order.OrderProducts.Any())
            {
                return MessageError("You can not delete products empty Order!");//redundant but still
            }
            db.OrdersProducts.RemoveRange(db.OrdersProducts.Where(x => x.OrderID == orderId));
            db.SaveChanges();
            return MessageSuccess($"Successfully cleared all products from {CurentUser.Username}'s cart", "/Orders/DisplayOrders", $"{CurentUser.Username}'s Orders");
        }
    }
}