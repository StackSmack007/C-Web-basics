namespace Airport.App.Controllers
{
    using Airport.App.ViewModels.Users;
    using Airport.Infrastructure.Models.Models;
    using Airport.Infrastructure.Models.Models.Enumerations;
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
            if (CurentUser != null) LogOffUser();
            return this.View();
        }

        public IHttpResponse Register()
        {
            if (CurentUser != null) LogOffUser();
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Login(UserLoginRegisterDTO user)
        {
            string passwordHashed = hasher.Encrypt(user.Password);
            if (!DB.Users.Any(x => x.Email == user.Email && x.Password == passwordHashed))
            {
                return MessageWithView($"Username or password do not match. Please enter correct Data!");
            }

            var foundUser = DB.Users.FirstOrDefault(x => x.Email==user.Email);

            this.LogInUser(foundUser.Username, foundUser.Id, foundUser.Role);
            logger.Log($"User {foundUser.Username} loged in at {DateTime.Now.ToString("R")}");
            return RedirectResult("/Home/Index");
        }

        [HttpPost]
        public IHttpResponse Register(UserLoginRegisterDTO newUser)
        {
            if (string.IsNullOrEmpty(newUser.Username)) return this.MessageError($"Username cant be empty", "/Users/Register", "Register");
            if (string.IsNullOrEmpty(newUser.Password) || string.IsNullOrEmpty(newUser.RepeatPassword)) return this.MessageError($"Password cant be empty", "/Users/Register", "Register");
            if (string.IsNullOrEmpty(newUser.Email)) return this.MessageError($"Email cant be empty", "/Users/Register", "Register");

            if (newUser.Password != newUser.RepeatPassword)
            {
                return this.MessageError("Passwords missmatch", "/Users/Register", "Register");
            }

            if (DB.Users.Any(x => x.Username == newUser.Username))
            {
                return this.MessageWithView($"Username {newUser.Username} already used!");
            }
            if (DB.Users.Any(x => x.Email == newUser.Email))
            {
                return this.MessageWithView($"Email {newUser.Email} already used!");
            }
            User user = newUser.MapTo<User>();

            if (!DB.Users.Any())
            {
                user.Role = UserRole.Admin;
            }
            user.Password = hasher.Encrypt(user.Password);
            DB.Users.Add(user);
            DB.SaveChanges();
            this.LogInUser(user.Username, user.Id, user.Role);
            return RedirectResult("/Home/Index");
        }

        [HttpGet]
        [HttpGet("/Users/Logout")]
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