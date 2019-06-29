namespace Application.Controllers
{
    using Application.Services;
    using Infrastructure.Models.Models;
    using Infrastructure.Models.Validators;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using System;

    public class HomeController : BaseController
    {
        private readonly EncriptionManager encrypter;
        public HomeController()
        {
            encrypter = new EncriptionManager();
        }
        public IHttpResponse Index()
        {
            return View();
        }

        public IHttpResponse Register()
        {
            return View();
        }

        public IHttpResponse Register(IHttpRequest response)
        {
            string userName = response.FormData["username"].ToString();
            string password = response.FormData["password"].ToString();
            string verificationPassword = response.FormData["verifyPassword"].ToString();

            if (password != verificationPassword)
            {
                return this.ControllerError("Passwords missmatch", "Register", "Register");
            }
            var user = new User() { Username = userName, Password = password, RegisteredOn = DateTime.UtcNow };
            var userValidator = new UserValidator();
            var validationResult = userValidator.Validate(user);

            if (!validationResult.IsValid)
            {
                return this.ControllerError(string.Join(Environment.NewLine, validationResult.Errors), "Register", "Register");
            }
            user.Password= encrypter.Encrypt(password);

            db.Users.Add(user);
            db.SaveChanges();

            return Index();
        }

    }
}