using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using HttpServer.SessionsService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpServer.Controllers
{
    [ApiController("^profile$")]
    class UserController
    {
        static UserDAO userDAO 
            = new UserDAO(MainController.DatabaseConnectionString);

        [HttpPOST("^login$")]
        public static ControllerResponse Login(string login, string password)
        {
            var user = userDAO.Select(login, HttpUtility.UrlDecode(password));
            if (user == null)
                return new ControllerResponse(new View("auth", new { IncorrectPassword = true, Login = login }));

            var session = SessionManager.Instance.CreateSession(user.Id, user.Login);

            var action =
                (HttpListenerResponse response) =>
                {
                    var cookie = new Cookie("SessionId", session.Guid.ToString(), "/");
                    response.Cookies.Add(cookie);
                    response.Redirect(@"/main");
                };

            return new ControllerResponse(true, action: action);
        }

        [HttpPOST("^register$")]
        public static ControllerResponse Register(string login, string name, string password)
        {
            if (userDAO.Select(login) != null)
                return new ControllerResponse(new View("register", new { IncorrectLogin = true, Login = login, Name = HttpUtility.UrlDecode(name) }));

            userDAO.Insert(new User(login, HttpUtility.UrlDecode(password), HttpUtility.UrlDecode(name)));
            return Login(login, password);
        }

        [HttpGET("^logout$", needSessionId: true)]
        public static ControllerResponse LogOut(Guid sessionId)
        {
            SessionManager.Instance.RemoveSession(sessionId);

            var redirectAction = (HttpListenerResponse response) => response.Redirect(@"/auth");

            return new ControllerResponse(null, redirectAction);
        }

        [HttpGET(@"^\d$")]
        public static ControllerResponse ShowUserProfile(int id)
        {
            var user = userDAO.Select(id);
            if (user == null)
                return new ControllerResponse(null, statusCode: HttpStatusCode.NotFound);

            var view = new View("profile", user);
            return new ControllerResponse(view);
        }

        [HttpGET("^$", onlyForAuthorized: true, needSessionId: true)]
        public static ControllerResponse ShowCurrentUserProfile(Guid sessionId)
        {
            var session = SessionManager.Instance.GetSession(sessionId);
            return ShowUserProfile(session.AccountId);
        }


        [HttpGET("^[a-zA-Z0-9_]+$")]
        public static ControllerResponse ShowUserProfile(string login)
        {
            var user = userDAO.Select(login);
            if (user == null)
                return new ControllerResponse(null, statusCode: HttpStatusCode.NotFound);

            return ShowUserProfile(user.Id);
        }
        public static User GetUserBySessionId(Guid sessionId)
        {
            var session = SessionManager.Instance.GetSession(sessionId);
            return userDAO.Select(session.AccountId);
        }

        public static User GetUserById(int id)
        {
            var user = userDAO.Select(id);
            return user;
        }
    }
}