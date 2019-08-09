namespace MishMashApplication.Controllers
{
    using MishMashApplication.DTO.Users;
    using MishMashApplication.Models;
    using MishMashApplication.Models.Enumerations;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using SIS.MVC.Contracts;
    using SIS.MVC.Extensions;
    using System;
    using System.Linq;

    public class UsersController : BaseController
    {
        ILogger logger;
        public UsersController(ILogger logger)
        {
            this.logger = logger;
        }

        public IHttpResponse Login()
        {
            if (CurentUser != null)
            { 
                return this.MessageError($"User with name {CurentUser.Username} is already loged in. Please log out first before registering new user!");
            }
            return this.View();
        }

        public IHttpResponse Register()
        {
            if (CurentUser != null)
            {
                return this.MessageError($"User with name {CurentUser.Username} is already loged in. Please log out first before registering new user!");
            }
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Login(UserDTO user)
        {
            var FoundUser = Db.Users.FirstOrDefault(x => x.Username == user.Username);


            string passwordHashed = hasher.Encrypt(user.Password);
            if (!Db.Users.Any(x => x.Username == user.Username && x.Password == passwordHashed))
            {
                return MessageWithView($"Username or password do not match. Please enter correct Data!");
            }

            int userId = Db.Users.FirstOrDefault(x => x.Username == user.Username).Id;
            this.LogInUser(user.Username, userId);

            logger.Log($"User {user.Username} loged in at {DateTime.Now.ToString("R")}");
            return this.MessageSuccess($"User {user.Username} was successfully logged in!", "/Home/Index", "HomePage");
        }

        [HttpPost]
        public IHttpResponse Register(UserDTO newUser)
        {
            if (string.IsNullOrEmpty(newUser.Username)) return this.MessageError($"Username cant be empty", "/Users/Register", "Register");
            if (string.IsNullOrEmpty(newUser.Password)) return this.MessageError($"Password cant be empty", "/Users/Register", "Register");
            if (string.IsNullOrEmpty(newUser.Email)) return this.MessageError($"Email cant be empty", "/Users/Register", "Register");

            HttpCookie legitimationCookie = this.Request.Cookies.GetCookie(cookieService.LoginCookieName);

            if (CurentUser != null)
            {
                return this.MessageError($"User with name {CurentUser.Username} is already loged in. Please log out first before registering new user!");
            }

            if (newUser.Password != newUser.VerifyPassword)
            {
                return this.MessageError("Passwords missmatch", "/Users/Register", "Register");
            }
            newUser.Password = hasher.Encrypt(newUser.Password);

            if (Db.Users.FirstOrDefault(x => x.Username == newUser.Username) != null)
            {
                return this.MessageError($"Username {newUser.Username} already used", "/Users/Register", "Register");
            }
            User user = newUser.MapTo<User>();
            if (!Db.Users.Any())
            {
                user.Role = UserRole.Admin;
            }
            Db.Users.Add(user);
            Db.SaveChanges();
            this.LogInUser(user.Username, user.Id);
            return MessageSuccess($"User {user.Username} was successfully registered and logged in!", "/Home/Index", "HomePage");
        }

        public IHttpResponse Logoff()
        {
            if (CurentUser is null)
            {
                return this.MessageError($"No User is loged in, unloging not possible!");
            }
            this.LogOffUser();
            return RedirectResult("/Home/Index");
        }
    }
}