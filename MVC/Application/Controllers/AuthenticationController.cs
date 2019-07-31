namespace Application.Controllers
{
    using Application.ViewModels.Authentication;
    using Infrastructure.Models.Models;
    using Infrastructure.Models.Validators;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using SIS.MVC.Contracts;
    using System;
    using System.Linq;

    public class AuthenticationController : BaseController
    {
        protected ILogger logger;

        public AuthenticationController(ILogger logger)
        {
            this.logger = logger;
        }

        [HttpGet("/Authentication/Register")]
        public IHttpResponse Register()
        {
            if (this.CurentUser != null)
            {
                return this.MessageError($"User with name {CurentUser.UserName} is already loged in. Please log out first");
            }
            return View();
        }

        [HttpPost("/Authentication/RegisterData")]
        public IHttpResponse RegisterData(UserDto newUser)
        {
            HttpCookie legitimationCookie = this.Request.Cookies.GetCookie(cookieService.LoginCookieName);

            if (CurentUser != null)
            {
                return this.MessageError($"User with name {CurentUser.UserName} is already loged in. Please log out first before registering new user!");
            }

            if (newUser.Password != newUser.PasswordVerify)
            {
                return this.MessageError("Passwords missmatch", "Register", "Register");
            }

            if (db.Users.FirstOrDefault(x => x.Username == newUser.UserName) != null)
            {
                return this.MessageError($"Username {newUser.UserName} already used", "Register", "Register");
            }

            var user = new User() { Username = newUser.UserName, Password = newUser.Password, RegisteredOn = DateTime.UtcNow };//password is entered to pass validation only!
            var userValidator = new UserValidator();
            var validationResult = userValidator.Validate(user);

            if (!validationResult.IsValid)
            {
                return this.MessageError(string.Join(Environment.NewLine, validationResult.Errors), "Register", "Register");
            }

            user.Password = hasher.Encrypt(newUser.Password);
            db.Users.Add(user);
            db.SaveChanges();

            int userId = db.Users.FirstOrDefault(x=>x.Username== newUser.UserName).Id;
            this.LogInUser(newUser.UserName, userId);

            this.RedirectResult("/");
            return this.Response;
        }

        [HttpGet("/Authentication/LogOff")]
        public IHttpResponse LogOff()
        {
            if (CurentUser is null)
            {
                return this.MessageError("No user was loged in at the moment");
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
                return this.MessageError($"User with name {CurentUser.UserName} is already loged in. Please log out first");
            }
            return View();
        }

        [HttpPost("/Authentication/LogInData")]
        public IHttpResponse LogInData(UserDto user)
        {
            string passwordHashed = hasher.Encrypt(user.Password);
            if (!db.Users.Any(x => x.Username == user.UserName && x.Password == passwordHashed))
            {
                return this.MessageError($"Username or password do not match. Please enter correct Data", "LogIn", "Log In");
            }
            int userId = db.Users.FirstOrDefault(x => x.Username == user.UserName).Id;
            this.LogInUser(user.UserName, userId);
            this.RedirectResult("/");
            logger.Log($"User {user.UserName} loged in at {DateTime.Now.ToString("R")}");
            return this.Response;
        }
    }
}