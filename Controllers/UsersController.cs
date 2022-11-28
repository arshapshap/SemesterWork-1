using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpServer
{
    [ApiController("profile")]
    class UsersController
    {
        static UserDAO userDAO 
            = new UserDAO(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SiteDB;Integrated Security=True");

        [HttpPOST("login")]
        public static ControllerResponse Login(string login, string password)
        {
            var user = userDAO.Select(login, password);
            if (user == null)
                return new ControllerResponse(false);

            var session = SessionManager.Instance.CreateSession(user.Id, user.Login);

            var addCookieAction =
                (HttpListenerResponse response) => {
                    var cookie = new Cookie("SessionId", session.Guid.ToString());
                    response.Cookies.Add(cookie);

                    response.Redirect(@"/");
                };

            return new ControllerResponse(true, action: addCookieAction);
        }

        [HttpPOST("register")]
        public static ControllerResponse Register(string login, string name, string password)
        {
            userDAO.Insert(new User(login, password, HttpUtility.UrlDecode(name)));

            var redirectAction = (HttpListenerResponse response) => {
                    response.Redirect(@"/");
                };

            return new ControllerResponse(null, redirectAction);
        }

        [HttpGET(@"\d")]
        public static ControllerResponse GetUserById(int id)
        {
            var user = userDAO.Select(id);
            if (user == null)
                return new ControllerResponse(null, statusCode: HttpStatusCode.NotFound);

            var view = new View("profile", user);
            return new ControllerResponse(view);
        }

        [HttpGET("^$", onlyForAuthorized: true, needSessionId: true)]
        public static ControllerResponse GetCurrentUser(Guid sessionId)
        {
            var session = SessionManager.Instance.GetSession(sessionId);
            return GetUserById(session.AccountId);
        }

        public static User GetUserBySessionId(Guid sessionId)
        {
            var session = SessionManager.Instance.GetSession(sessionId);
            var user = userDAO.Select(session.AccountId);
            return user;
        }

        /*
        [HttpGET("all", onlyForAuthorized: true)]
        public ControllerResponse GetUsers()
        {
            return new ControllerResponse(userDAO.Select());
        }*/
    }
}