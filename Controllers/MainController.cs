using HttpServer.Attributes;
using HttpServer.SessionsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Controllers
{
    [ApiController("^$")]
    internal class MainController
    {
        public static readonly string DatabaseConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SiteDB;Integrated Security=True";

        [HttpGET("main", needSessionId: true)]
        public static ControllerResponse GetMainPage(Guid sessionId)
        {
            var view = new View("main", new { User = SessionManager.Instance.CheckSession(sessionId) });
            return new ControllerResponse(view);
        }

        [HttpGET("auth", needSessionId:true)]
        public static ControllerResponse GetAuthPage(Guid sessionId)
        {
            if (SessionManager.Instance.CheckSession(sessionId))
            {
                var redirectAction =
                    (HttpListenerResponse response) =>
                    {
                        response.Redirect("/main");
                    };
                return new ControllerResponse(null, action: redirectAction);
            }

            var view = new View("auth");
            return new ControllerResponse(view);
        }

        [HttpGET("register", needSessionId:true)]
        public static ControllerResponse GetRegisterPage(Guid sessionId)
        {
            if (SessionManager.Instance.CheckSession(sessionId))
            {
                var redirectAction =
                    (HttpListenerResponse response) =>
                    {
                        response.Redirect("/main");
                    };
                return new ControllerResponse(null, action: redirectAction);
            }

            var view = new View("register");
            return new ControllerResponse(view);
        }
    }
}
