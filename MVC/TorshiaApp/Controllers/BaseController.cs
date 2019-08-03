using SIS.HTTP.Responses.Contracts;
using SIS.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using TorshiaApp.Data;
using TorshiaApp.Models.Enumerations;

namespace TorshiaApp.Controllers
{
    public abstract class BaseController : Controller
    {
        public TorshiaContext DB { get; set; }
        private Dictionary<UserRole, string> layouts;
        public BaseController()
        {
            DB = new TorshiaContext();
            layouts = new Dictionary<UserRole, string>
            {
                [UserRole.Guest] = "layoutGuest.html",
                [UserRole.Admin] = "layoutAdmin.html",
                [UserRole.User] = "layoutUser.html",
            };
        }

        protected UserRole GetRole()
        {
            if (CurentUser is null)
            {
                return UserRole.Guest;
            }
            return Enum.Parse<UserRole>(CurentUser.Role);
        }

        private string ChooseLayout()
        {
            var role = GetRole();
            var layoutName = layouts[GetRole()];
            return layoutName;
        }

        protected override IHttpResponse View(string layoutName = "DefaultLayout")
        {
            if (layoutName == "DefaultLayout")
            {
                return base.View(ChooseLayout());
            }
            return base.View(layoutName);
        }

        protected override IHttpResponse ViewFilePath(string subPath, string layoutName = "DefaultLayout")
        {
            if (layoutName == "DefaultLayout")
            {
                return base.ViewFilePath(subPath, ChooseLayout());
            }
            return base.ViewFilePath(subPath, layoutName);
        }

        protected override IHttpResponse MessageError(string message, string redirectAdress = "/", string redirectName = "HomePage", string layoutName = "DefaultLayout")
        {
            return base.MessageError(message, redirectAdress, redirectName, ChooseLayout());
        }

        protected override IHttpResponse MessageSuccess(string message, string redirectAdress = "/", string redirectName = "HomePage", string layoutName = "DefaultLayout")
        {
            return base.MessageSuccess(message, redirectAdress, redirectName, ChooseLayout());
        }

        protected override IHttpResponse MessageWithView(string message, bool isError = true, string viewSubPath = "Default", string layoutName = "DefaultLayout")
        {
            return base.MessageWithView(message, isError, viewSubPath, ChooseLayout());
        }
    }
}