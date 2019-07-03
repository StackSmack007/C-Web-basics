using Infrastructure.Models.Models;
using Infrastructure.Models.Validators;
using SIS.HTTP.Cookies;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Results;
using System;
using System.Linq;

namespace Application.Controllers
{
    public class AuthenticationController : BaseController
    {
        public IHttpResponse Register()
        {
            return View();
        }

        public IHttpResponse Register(IHttpRequest request)
        {
            string userName = request.FormData["username"].ToString();
            string password = request.FormData["password"].ToString();
            string verificationPassword = request.FormData["verifyPassword"].ToString();
            HttpCookie legitimationCookie = request.Cookies.GetCookie(loginCookieName);
            string userNameLoged = GetUserNameFromCookie(legitimationCookie);
            if (userNameLoged != null && legitimationCookie.Expires > DateTime.UtcNow)
            {
                return this.ControllerError($"User with name {userNameLoged} is already loged in. Please log out first before registering new user!");
            }

            if (password != verificationPassword)
            {
                return this.ControllerError("Passwords missmatch", "Register", "Register");
            }
            if (db.Users.FirstOrDefault(x => x.Username == userName) != null)
            {
                return this.ControllerError($"Username {userName} already used", "Register", "Register");
            }
            var user = new User() { Username = userName, Password = password, RegisteredOn = DateTime.UtcNow };
            var userValidator = new UserValidator();
            var validationResult = userValidator.Validate(user);

            if (!validationResult.IsValid)
            {
                return this.ControllerError(string.Join(Environment.NewLine, validationResult.Errors), "Register", "Register");
            }
            user.Password = hasher.Encrypt(password);
            db.Users.Add(user);
            db.SaveChanges();

            var loginCookie = GeLoginCookie(userName);
            request.Cookies.Add(loginCookie);
            IHttpResponse goToIndex = new HomeController().Index(request);
            goToIndex.AddCookie(loginCookie);
            return goToIndex;
        }

        public IHttpResponse LogOf(IHttpRequest request)
        {
            string cookieValue = request.Cookies.GetCookie(loginCookieName).Value;
            if (cookieValue is null)
            {
                return this.ControllerError($"No user was loged in at the moment", "Home", "Home");
            }
            var cookieDelete = new HttpCookie(loginCookieName, cookieValue,true, -1,true,false);

            
            IHttpResponse redirectToHome = new RedirectResult("/");
            redirectToHome.AddCookie(cookieDelete);
            return redirectToHome;
        }

        public IHttpResponse LogIn(IHttpRequest request)
        {
            string userName = GetUserNameFromCookie(request.Cookies.GetCookie(loginCookieName));
            if (userName != null)
            {
                return this.ControllerError($"User with name {userName} is already loged in. Please log out first");
            }
            return View();
        }

        public IHttpResponse LogInData(IHttpRequest request)
        {
            string userName = request.FormData["username"].ToString();
            string password = request.FormData["password"].ToString();

            string passwordHashed = hasher.Encrypt(password);
            if (!db.Users.Any(x => x.Username == userName && x.Password == passwordHashed))
            {
                return this.ControllerError($"Username or password do not match. Please enter correct Data", "LogIn", "Log In");
            }
            var loginCookie = GeLoginCookie(userName);
            request.Cookies.Add(loginCookie);
            IHttpResponse goToIndex = new HomeController().Index(request);
            goToIndex.AddCookie(loginCookie);
            return goToIndex;
        }
    }
}
