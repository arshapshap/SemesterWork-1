using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using HttpServer.SessionsService;
using HttpServer.TemplatesService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace HttpServer.Controllers
{
    [ApiController("^profile$")]
    class UserController
    {
        static UserDAO userDAO 
            = new UserDAO(MainController.DatabaseConnectionString);

        [HttpPOST("^login$")]
        public static ControllerResponse Login(string login, string password, bool rememberMe, Guid sessionId)
        {
            var user = userDAO.Select(login, HashService.Hash(password));
            if (user is null)
            {
                var entered = new { Login = login };
                return new ControllerResponse(new View("pages/auth", new { IncorrectPassword = true, EnteredInfo = entered }));
            }

            var session = SessionManager.Instance.CreateSession(user.Id, rememberMe);

            var action =
                (HttpListenerResponse response) =>
                {
                    var cookie = new Cookie("SessionId", session.Guid.ToString(), "/");
                    response.Cookies.Add(cookie);
                    response.Redirect(@"/main");
                };

            return new ControllerResponse(action: action);
        }

        [HttpPOST("^register$")]
        public static ControllerResponse Register(string login, string name, string password, bool rememberMe, Guid sessionId)
        {
            if (userDAO.Select(login) is not null)
            {
                var entered = new { Login = login, Name = HttpUtility.UrlDecode(name) };
                return new ControllerResponse(new View("pages/register", new { IncorrectLogin = true, EnteredInfo = entered }));
            }

            userDAO.Insert(new User(login, HashService.Hash(password), HttpUtility.UrlDecode(name)));
            return Login(login, password, rememberMe, sessionId);
        }

        [HttpPOST("^logout$")]
        public static ControllerResponse LogOut(Guid sessionId)
        {
            SessionManager.Instance.RemoveSession(sessionId);

            var redirectAction = (HttpListenerResponse response) => response.Redirect(@"/auth");
            return new ControllerResponse(action: redirectAction);
        }

        [HttpGET("^edit$")]
        public static ControllerResponse ShowEditingPage(string _, int userId, Guid sessionId)
        {
            var currentUser = GetUserBySessionId(sessionId)
               ?? throw new ServerException(HttpStatusCode.Unauthorized);

            if (currentUser.Id != userId)
                throw new ServerException(HttpStatusCode.Forbidden);

            var view = new View("pages/profile", new { IsEditing = true, CurrentUser = currentUser, User = currentUser });
            return new ControllerResponse(view);
        }

        [HttpPOST("^edit$")]
        public static ControllerResponse Edit(int userId, string name, string imageURL, string password, Guid sessionId)
        {
            var currentUser = GetUserBySessionId(sessionId)
               ?? throw new ServerException(HttpStatusCode.Unauthorized);

            if (currentUser.Id != userId)
               throw new ServerException(HttpStatusCode.Forbidden);

            if (name != "")
                currentUser.Name = HttpUtility.UrlDecode(name);
            if (imageURL != "")
                currentUser.Image = HttpUtility.UrlDecode(imageURL);
            if (password != "")
                currentUser.HashedPassword = HashService.Hash(password);

            userDAO.Update(userId, currentUser);

            var redirectAction = (HttpListenerResponse response)
                => response.Redirect($"/profile");

            return new ControllerResponse(action: redirectAction);
        }

        [HttpGET(@"^\d+$")]
        public static ControllerResponse ShowUserProfile(int id, Guid sessionId)
        {
            var currentUser = GetUserBySessionId(sessionId);

            var user = userDAO.Select(id);
            if (user == null)
                throw new ServerException(HttpStatusCode.NotFound);

            var view = new View("pages/profile", new { CurrentUser = currentUser, User = user });
            return new ControllerResponse(view);
        }

        [HttpGET("^[a-zA-Z0-9_]+$")]
        public static ControllerResponse ShowUserProfile(string login, Guid sessionId)
        {
            var user = userDAO.Select(login)
                ?? throw new ServerException(HttpStatusCode.NotFound);

            return ShowUserProfile(user.Id, sessionId);
        }

        [HttpGET("^$")]
        public static ControllerResponse ShowCurrentUserProfile(Guid sessionId)
        {
            var session = SessionManager.Instance.GetSession(sessionId) 
                ?? throw new ServerException(HttpStatusCode.Unauthorized);

            return ShowUserProfile(session.AccountId, sessionId);
        }

        public static User? GetUserBySessionId(Guid sessionId)
        {
            var session = SessionManager.Instance.GetSession(sessionId);
            if (session == null)
                return null;

            return userDAO.Select(session.AccountId);
        }

        public static User? GetUserById(int id)
        {
            var user = userDAO.Select(id);
            return user;
        }
    }
}