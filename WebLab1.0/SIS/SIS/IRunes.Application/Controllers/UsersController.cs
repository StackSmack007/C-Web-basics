namespace IRunes.Application.Controllers
{
    using IRunes.Infrastructure.Models.Models;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using System;
    using System.Linq;
    public class UsersController : BaseController
    {
        public IHttpResponse Register(IHttpRequest request)
        {
            if (IsUserLogedIn(request))
            {
                string logedUser = GetCurrentSessionUserIdandName(request)[1];
                return ResposeErrorMessageAndRedirect($"User {logedUser} is loged in. Please Log-Out first");
            }
            return View();
        }

        public IHttpResponse RegisterData(IHttpRequest request)
        {
            string userName = request.FormData["username"].ToString();
            if (request.FormData.Any(x => string.IsNullOrEmpty(x.ToString())))
            {
                return ResposeErrorMessageAndRedirect("Invalid userName,password or email,please enter corect Data", @"/Users/Register", "Register");
            }
            string password1 = request.FormData["password1"].ToString();
            string password2 = request.FormData["password2"].ToString();
            string email = request.FormData["email"].ToString();
            if (password1 != password2)
            {
                return ResposeErrorMessageAndRedirect("Passwords differ", @"/Users/Register", "Register");
            }
            if (db.Users.Any(x => x.UserName == userName))
            {
                return ResposeErrorMessageAndRedirect("Username is taken choose another...", @"/Users/Register", "Register");
            }
            if (db.Users.Any(x => x.Email == email))
            {
                return ResposeErrorMessageAndRedirect("Email is used by another user already use another...", @"/Users/Register", "Register");
            }
            string hashedPassword = hasher.Encrypt(password1);
            User newUser = new User() { UserName = userName, Password = hashedPassword, Email = email };
            db.Users.Add(newUser);
            db.SaveChanges();
            LogInUser(request, newUser.Id, newUser.UserName);
            return new HomeController().Index(request);
        }


        public IHttpResponse Logout(IHttpRequest request)
        {
            LogOutUser(request);
            return new HomeController().Index(request);
        }

        public IHttpResponse Login(IHttpRequest request)
        {
            if (IsUserLogedIn(request))
            {
                string logedUser = GetCurrentSessionUserIdandName(request)[1];
                return ResposeErrorMessageAndRedirect($"User {logedUser} is loged in. Please Log-Out first");
            }
            return View();
        }

        public IHttpResponse LoginData(IHttpRequest request)
        {
            string nameOrMail = request.FormData["usernameOrMail"].ToString() ;
       
            string password = request.FormData["password"].ToString();
            string hashedPassword = hasher.Encrypt(password);
          
            User foundUser = db.Users.FirstOrDefault(x => (x.UserName == nameOrMail || x.Email == nameOrMail) && x.Password == hashedPassword);
            if (foundUser is null)
            {
                return ResposeErrorMessageAndRedirect("Wrong username/mail or password", "/Users/Login");
            }
            LogInUser(request, foundUser.Id, foundUser.UserName);
            return new HomeController().Index(request);
        }
    }
}