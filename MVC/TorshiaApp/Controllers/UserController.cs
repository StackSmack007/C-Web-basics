namespace TorshiaApp.Controllers
{
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using SIS.MVC.Contracts;
    using System;
    using System.Linq;
    using TorshiaApp.DTO.User;
    using TorshiaApp.Models;
    using TorshiaApp.Models.Enumerations;

    public class UserController : BaseController
    {

        ILogger logger;
        public UserController(ILogger logger)
        {
            this.logger = logger;
        }

        public IHttpResponse Login()
        {
            if (CurentUser != null)
            {
                return this.MessageError($"User with name {CurentUser.UserName} is already loged in. Please log out first before registering new user!");
            }
            return this.View();
        }

        public IHttpResponse Register()
        {
            if (CurentUser != null)
            {
                return this.MessageError($"User with name {CurentUser.UserName} is already loged in. Please log out first before registering new user!");
            }
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Login(UserLoginDTO user)
        {
            var foundUser = DB.Users.FirstOrDefault(x => x.Username == user.Username);

            string passwordHashed = hasher.Encrypt(user.Password);
            if (!DB.Users.Any(x => x.Username == user.Username && x.Password == passwordHashed))
            {
                return MessageWithView($"Username or password do not match. Please enter correct Data!");
            }

            this.LogInUser(foundUser.Username, foundUser.Id);
            logger.Log($"User {foundUser.Username} loged in at {DateTime.Now.ToString("R")}");
            return RedirectResult("/");
            //return this.MessageSuccess($"User {foundUser.Username} was successfully logged in!", "/Home/Index", "HomePage");
        }


        [HttpPost]
        public IHttpResponse Register(UserRegisterDTO newUser)
        {
            if (string.IsNullOrEmpty(newUser.Username)) return this.MessageError($"Username cant be empty", "/Users/Register", "Register");
            if (string.IsNullOrEmpty(newUser.Password1)|| string.IsNullOrEmpty(newUser.Password2)) return this.MessageError($"Password cant be empty", "/Users/Register", "Register");
               if (string.IsNullOrEmpty(newUser.Email)) return this.MessageError($"Email cant be empty", "/Users/Register", "Register");
        
               HttpCookie legitimationCookie = this.Request.Cookies.GetCookie(cookieService.LoginCookieName);
        
               if (CurentUser != null)
               {
                   return this.MessageError($"User with name {CurentUser.UserName} is already loged in. Please log out first before registering new user!");
               }
        
               if (newUser.Password1 != newUser.Password2)
               {
                   return this.MessageError("Passwords missmatch", "/Users/Register", "Register");
               }
               string hashedPassword = hasher.Encrypt(newUser.Password1);
        
               if (DB.Users.Any(x => x.Username == newUser.Username))
               {
                   return this.MessageWithView($"Username {newUser.Username} already used!");
               }
               User user = new User
               {
                   Username=newUser.Username,
                   Email=newUser.Email,
                   Password=hashedPassword
               } ;
               if (!DB.Users.Any())
               {
                   user.Role = UserRole.Admin;
               }
               DB.Users.Add(user);
               DB.SaveChanges();
               this.LogInUser(user.Username, user.Id);
               return RedirectResult("/");
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