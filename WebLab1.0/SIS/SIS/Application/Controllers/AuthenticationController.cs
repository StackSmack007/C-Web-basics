namespace Application.Controllers
{
    using Infrastructure.Models.Models;
    using Infrastructure.Models.Validators;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System;
    using System.Linq;

    public class AuthenticationController : BaseController
    {

        [HttpGet("/Authentication/Register")]
        public IHttpResponse Register()
        {
            if (this.CurentUser != null)
            {
                return this.ControllerError($"User with name {CurentUser.UserName} is already loged in. Please log out first");
            }
            return View();
        }

        [HttpPost("/Authentication/RegisterData")]
        public IHttpResponse RegisterData()
        {
            string userName = this.Request.FormData["username"].ToString();
            string password = this.Request.FormData["password"].ToString();
            string verificationPassword = this.Request.FormData["verifyPassword"].ToString();
            HttpCookie legitimationCookie = this.Request.Cookies.GetCookie(loginCookieName);

            if (CurentUser != null)
            {
                return this.ControllerError($"User with name {CurentUser.UserName} is already loged in. Please log out first before registering new user!");
            }

            if (password != verificationPassword)
            {
                return this.ControllerError("Passwords missmatch", "Register", "Register");
            }

            if (db.Users.FirstOrDefault(x => x.Username == userName) != null)
            {
                return this.ControllerError($"Username {userName} already used", "Register", "Register");
            }

            var user = new User() { Username = userName, Password = password, RegisteredOn = DateTime.UtcNow };//password is entered to pass validation only!
            var userValidator = new UserValidator();
            var validationResult = userValidator.Validate(user);

            if (!validationResult.IsValid)
            {
                return this.ControllerError(string.Join(Environment.NewLine, validationResult.Errors), "Register", "Register");
            }

            user.Password = hasher.Encrypt(password);
            db.Users.Add(user);
            db.SaveChanges();

            int userId = GetIdOfUserName(userName);
            this.LogInUser(userName, userId);

            this.RedirectResult("/");
            return this.Response;
        }

        [HttpGet("/Authentication/LogOf")]
        public IHttpResponse LogOf()
        {
            if (CurentUser is null)
            {
                return this.ControllerError("No user was loged in at the moment");
            }
            LogOffUser();
            this.RedirectResult("/");
            return this.Response;
        }
        [HttpGet("/Authentication/LogIn")]
        public IHttpResponse LogIn()
        {
            if (this.CurentUser != null)
            {
                return this.ControllerError($"User with name {CurentUser.UserName} is already loged in. Please log out first");
            }
            return View();
        }
        [HttpPost("/Authentication/LogInData")]
        public IHttpResponse LogInData()
        {
            string userName = this.Request.FormData["username"].ToString();
            string password = this.Request.FormData["password"].ToString();


            string passwordHashed = hasher.Encrypt(password);
            if (!db.Users.Any(x => x.Username == userName && x.Password == passwordHashed))
            {
                return this.ControllerError($"Username or password do not match. Please enter correct Data", "LogIn", "Log In");
            }
            int userId = GetIdOfUserName(userName);
            this.LogInUser(userName, userId);
            this.RedirectResult("/");
            return this.Response;
        }
    }
}