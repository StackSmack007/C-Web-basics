using Panda.App.ViewModels.Packages;
using Panda.Indfrastructure.Models.Models;
using Panda.Infrastructure.Models.Enums;
using SIS.HTTP.Responses.Contracts;
using SIS.MVC;
using SIS.MVC.Attributes;
using SIS.MVC.Extensions;
using System;
using System.Globalization;
using System.Linq;

namespace Panda.App.Controllers
{
    public class PackagesController : BaseController
    {
        private Random random;
        public PackagesController(Random random)
        {
            this.random = random;
        }

        public IHttpResponse Create()
        {
            var users = DB.Users.Select(x => new userInfoPackageReceiverDTO
            {
                Name = x.Username,
                Id = x.Id

            }).ToArray();

            ViewData["Users"] = users;
            return View();
        }

        [HttpPost]
        public IHttpResponse Create(inputPackageDto package)
        {
            Package newPack = package.MapTo<Package>();
            DB.Packages.Add(newPack);
            DB.SaveChanges();
            return RedirectResult("/");
        }


        [Authorised("Admin")]
        public IHttpResponse Pending()
        {
            var packages = DB.Packages.Where(x => x.Status == Status.Pending).Select(x => new outputPendingDeliveredPackageDto
            {
                Id = x.Id,
                Description = x.Description,
                Weight = x.Weight,
                ShippingAddress = x.ShippingAddress,
                RecipientName = x.User.Username
            }).ToArray();
            ViewData["Packages"] = packages;
            return View();
        }

        [Authorised("Admin")]
        public IHttpResponse Ship(int id)
        {
            var package = DB.Packages.FirstOrDefault(x => x.Id == id);
            package.Status = Status.Shipped;
            var shipmantDays = random.Next(20, 40);
            package.EstimatedDeliveryDate = DateTime.UtcNow.AddDays(shipmantDays);
            DB.SaveChanges();
            return RedirectResult("/Packages/Pending");
        }


        [Authorised("Admin")]
        public IHttpResponse Shipped()
        {
            var packages = DB.Packages.Where(x => x.Status == Status.Shipped).Select(x => new outputShippedPackageDto
            {
                Id = x.Id,
                Description = x.Description,
                Weight = x.Weight,
                RecipientName = x.User.Username,
                DeliveryDate = x.EstimatedDeliveryDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
            }).ToArray();
            ViewData["Packages"] = packages;
            return View();
        }

        [Authorised("Admin")]
        public IHttpResponse Deliver(int id)
        {
            var package = DB.Packages.FirstOrDefault(x => x.Id == id);
            package.Status = Status.Delivered;
            DB.SaveChanges();
            return RedirectResult("/Packages/Shipped");
        }


        [Authorised("Admin")]
        public IHttpResponse Delivered()
        {
            var packages = DB.Packages.Where(x => x.Status == Status.Delivered||x.Status == Status.Acquired).Select(x => new outputPendingDeliveredPackageDto
            {
                Id = x.Id,
                Description = x.Description,
                Weight = x.Weight,
                ShippingAddress = x.ShippingAddress,
                RecipientName = x.User.Username
            }).ToArray();
            ViewData["Packages"] = packages;
            return View();
        }


        [Authorised]
        public IHttpResponse Details(int id)
        {
            var package = DB.Packages.Where(x => x.Id == id)
                .Select(x => new outputDetailsPackageDto
                {
                    Description = x.Description,
                    Weight = x.Weight,
                    Status = x.Status.ToString(),
                    ShippingAddress = x.ShippingAddress,
                    RecipientName = x.User.Username,
                    DeliveryDate = x.EstimatedDeliveryDate == null ? "N/A" : x.EstimatedDeliveryDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                }).FirstOrDefault();
            if (package is null)
            {
                return MessageError("Unfound Product");
            }
            ViewData["Package"] = package;
            return View();
        }


        [Authorised]
        public IHttpResponse Acquire(int id)
        {
            Package package = DB.Packages.FirstOrDefault(x => x.Id == id);
            if (package is null)
            {
                return MessageError("Unfound Product");
            }
            if (package.UserId != CurentUser.Id)
            {
                return MessageError("Unauthorised User");
            }
            package.Status = Status.Acquired;
            DB.SaveChanges();
            ReceiptsController receiptSController = (ReceiptsController)WebHost.ServiceContainer.CreateInstance(typeof(ReceiptsController));
            receiptSController.Create(package);
            return RedirectResult("/");
        }

    }
}