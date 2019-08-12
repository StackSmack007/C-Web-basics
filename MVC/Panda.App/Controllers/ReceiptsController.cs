namespace Panda.App.Controllers
{
    using Panda.App.ViewModels.Receipts;
    using Panda.Indfrastructure.Models.Models;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System;
    using System.Globalization;
    using System.Linq;

    public class ReceiptsController : BaseController
    {

        public void Create(Package package)
        {
            var receipt = new Receipt
            {
                Fee = package.Weight * 2.67m,
                IssuedOn = DateTime.UtcNow,
                RecipientId = package.UserId,
                PackageId = package.Id,
            };
            DB.Receipts.Add(receipt);
            DB.SaveChanges();
        }

        [Authorised]
        public IHttpResponse Index()
        {
            var receipts = DB.Receipts.Where(x => x.RecipientId == CurentUser.Id).Select(x => new outputReceiptIndexDto
            {
                Id = x.Id,
                Fee = x.Fee,
                IssuedOn = x.IssuedOn.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Recipient = x.Recipient.Username
            }).ToArray();
            ViewData["Receipts"] = receipts;
            return View();
        }

        [Authorised]
        public IHttpResponse Details(int id)
        {
            var receipt = DB.Receipts.Where(x => x.RecipientId == CurentUser.Id && x.Id == id).Select(x => new outputReceiptDetailsDto
            {
                Id=id,
                Recepient = x.Recipient.Username,
                PackageWeight = x.Package.Weight,
                PackageDescription = x.Package.Description,
                DeliveryAddress = x.Package.ShippingAddress,
                IssuedOn = x.IssuedOn.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Fee = x.Fee
            }).FirstOrDefault();

            if (receipt is null)
            {
                return MessageWithView("Unfound receipt",true,"/Views/Receipts/Index.html");
            }
            ViewData["Receipt"] = receipt;
            return View();
        }
    }
}