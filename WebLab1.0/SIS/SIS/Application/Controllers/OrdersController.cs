namespace Application.Controllers
{
    using Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class OrdersController : BaseController
    {
        private static int timeSpanInMinutesToConsiderSameOrder = 10;

        [HttpPost("/Orders/MakeOrder")]
        public IHttpResponse MakeOrder()
        {
            if (this.CurentUser is null)
            {
                return ControllerError("Guests are not authorised to make purchases", @"/Authentication/LogIn", "LogIn");
            }
            User buyer = db.Users.Include(x => x.Orders).ThenInclude(x => x.OrderProducts)
                .FirstOrDefault(x => x.Username == this.CurentUser.UserName);
            if (buyer is null) return ControllerError("User not found in database");

            int quantity = int.Parse(this.Request.FormData["count"].ToString());
            int cakeId = int.Parse(this.Request.FormData["cakeId"].ToString());

            DateTime nowTIme = DateTime.UtcNow;
            Order currentOrder = buyer.Orders.FirstOrDefault(x => x.DateOfCreation > nowTIme.AddMinutes(-timeSpanInMinutesToConsiderSameOrder));

            if (currentOrder != null && currentOrder.OrderProducts.Any(x => x.ProductID == cakeId))
            {
                currentOrder.OrderProducts.First(x => x.ProductID == cakeId).Quantity += quantity;
            }
            else if (currentOrder is null)
            {
                currentOrder = new Order()
                {
                    UserId = buyer.Id,
                    DateOfCreation = DateTime.UtcNow,
                    OrderProducts = new HashSet<OrderProduct> {new OrderProduct()
                {
                    ProductID = cakeId,
                    Quantity = quantity
                }
                    }
                };
                db.Orders.Add(currentOrder);
            }
            else
            {
                currentOrder.OrderProducts.Add(new OrderProduct()
                {
                    ProductID = cakeId,
                    Quantity = quantity
                });
            }
            db.SaveChanges();
            string userName = this.CurentUser.UserName;
            string cakeName = db.Products.First(x => x.Id == cakeId).ProductName;
            return ControllerSuccess($"Success: User {userName} ordered {quantity} pieces of cake {cakeName}", "/Home/Search", "Browse Cakes");
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
                .Select(x => new
                {
                    OrderId = x.Id,
                    CreatedOn = x.DateOfCreation.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    TotalPrice = x.OrderProducts.Select(op => op.Product.Price * op.Quantity).Sum()
                }).ToArray();
            StringBuilder sb = new StringBuilder();

            foreach (var item in userOrders)
            {
                sb.Append($"<tr><th><a href=\"/Orders/DisplayOrder?id={item.OrderId}\">Order: {item.OrderId}</th><th>{item.CreatedOn}</th><th>{item.TotalPrice:F2} (<strong>Euro</strong>)</th></tr>");
            }
            ViewData["username"] = this.CurentUser.UserName;
            ViewData["tableRows"] = sb.ToString();
            return View();
        }

        [HttpGet("/Orders/DisplayOrder")]
        public IHttpResponse DisplayOrder()
        {
            int orderId=default(int);
            Order order=null;
            try
            {
                orderId = int.Parse(this.Request.QueryData["id"].ToString());
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
            StringBuilder sb = new StringBuilder();
            decimal totalCost = 0;
            foreach (var orderproduct in order.OrderProducts.OrderByDescending(x => x.Quantity * x.Product.Price))
            {
                string productName = orderproduct.Product.ProductName;
                int productId = orderproduct.ProductID;
                int quantity = orderproduct.Quantity;
                decimal singlePrice = orderproduct.Product.Price;
                decimal totalPrice = orderproduct.Quantity * orderproduct.Product.Price;
                totalCost += totalPrice;
                sb.Append($"<tr><th class=\"longText\"><a href=\"/Home/DisplayCake?id={productId}\">{productName}</a></th><th>{quantity}</th><th>{singlePrice:F2} &#8364</th><th>{totalPrice:F2} &#8364</th></tr>");
            }
            ViewData["orderId"] = orderId;
            ViewData["totalCost"] = totalCost.ToString("F2");
            ViewData["ordersList"] = sb.ToString();
            return View();
        }
    }
}