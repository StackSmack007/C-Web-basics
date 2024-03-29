﻿namespace Application.Controllers
{
    using Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class OrdersController : BaseController
    {
        private const int timeSpanInMinutesToConsiderSameOrder = 10;

        public IHttpResponse MakeOrder(IHttpRequest request)
        {
            HttpCookie authenticationCookie = request.Cookies.GetCookie(loginCookieName);

            if (!VerifyMemberCookie(request))
            {
                return ControllerError("User not authorised to make purchases", @"/Authentication/LogIn", "LogIn");
            }
            User buyer = db.Users.Include(x => x.Orders).ThenInclude(x => x.OrderProducts)
                .FirstOrDefault(x => x.Username == GetUserNameFromCookie(authenticationCookie));
            if (buyer is null) return ControllerError("User not found in database");

            int quuantity = int.Parse(request.FormData["count"].ToString());
            int cakeId = int.Parse(request.FormData["cakeId"].ToString());

            DateTime nowTIme = DateTime.UtcNow;
            Order currentOrder = buyer.Orders.FirstOrDefault(x => x.DateOfCreation > nowTIme.AddMinutes(-timeSpanInMinutesToConsiderSameOrder));
            bool orderIsNew = false;

            if (currentOrder != null && currentOrder.OrderProducts.Any(x => x.ProductID == cakeId))
            {
                currentOrder.OrderProducts.First(x => x.ProductID == cakeId).Quantity += quuantity;
            }
            else if (currentOrder is null)
            {
                orderIsNew = true;
                currentOrder = new Order()
                {
                    UserId = buyer.Id,
                    DateOfCreation = DateTime.UtcNow,
                    OrderProducts = new HashSet<OrderProduct> {new OrderProduct()
                {
                    ProductID = cakeId,
                    Quantity = quuantity
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
                    Quantity = quuantity
                });
            }
            db.SaveChanges();
            return new HomeController().Search();
        }


        public IHttpResponse DisplayOrders(IHttpRequest request)
        {
            string username = GetUserNameFromCookie(request.Cookies.GetCookie(loginCookieName));
            if (username is null)
            {
                return this.ControllerError($"No user loged in Log in first");
            }
            var userOrders = db.Orders
                .Where(x => x.User.Username == username)
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
                sb.Append($"<tr><th><a href=\"/Orders/DisplayOrder?id={item.OrderId}\">{item.OrderId}</th><th>{item.CreatedOn}</th><th>{item.TotalPrice:F2} (<strong>Euro</strong>)</th></tr>");
            }
            ViewData["username"] = username;
            ViewData["tableRows"] = sb.ToString();
            return View();
        }
        internal IHttpResponse DisplayOrder(IHttpRequest request)
        {
           
            int orderId;
            Order order;
            try
            {
                var loginCookie = request.Cookies.GetCookie(loginCookieName);
                string userName = GetUserNameFromCookie(loginCookie);


                orderId = int.Parse(request.QueryData["id"].ToString());
                order = db.Orders.Include(o=>o.User).Include(o => o.OrderProducts)
                                 .ThenInclude(op => op.Product).First(x=>x.Id==orderId);    

                if (order.User.Username!=userName)
                {
                    return ControllerError($"User {userName} is not outhorised to view another user's orders");
                }
            }
            catch (Exception)
            {
                return ControllerError("Invalid OrderId in the link");
            }
            StringBuilder sb = new StringBuilder();
            decimal totalCost = 0;
            foreach (var orderproduct in order.OrderProducts.OrderByDescending(x=> x.Quantity * x.Product.Price))
            {
                string productName = orderproduct.Product.ProductName;
                int productId = orderproduct.ProductID;
                int quantity = orderproduct.Quantity;
                decimal totalPrice = orderproduct.Quantity * orderproduct.Product.Price;
                totalCost += totalPrice;
                sb.Append($"<tr><th class=\"longText\"><a href=\"/Home/DisplayCake?id={productId}\">{productName}</a></th><th>{quantity}</th><th>{totalPrice}</th></tr>");
            }
            ViewData["orderId"] = orderId;
            ViewData["totalCost"] = totalCost.ToString("F2");
            ViewData["ordersList"] = sb.ToString();
            return View();
        }

    }
}