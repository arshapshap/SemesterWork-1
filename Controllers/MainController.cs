using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.SessionsService;
using HttpServer.TemplatesService;
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

        [HttpGET("^popular$")]
        public static ControllerResponse ShowPopularPublications(Guid sessionId)
        {
            var publications = PublicationController.GetAllPublications().OrderByDescending(p => p.Rating);

            var currentUser = UserController.GetUserBySessionId(sessionId);

            var view = new View("pages/main", new { AjaxRequest = true, ShowPopular = true, CurrentUser = currentUser, Publications = publications });
            return new ControllerResponse(view);
        }

        [HttpGET("^new$")]
        public static ControllerResponse ShowNewPublications(Guid sessionId)
        {
            var publications = PublicationController.GetAllPublications().OrderByDescending(p => p.Id);

            var currentUser = UserController.GetUserBySessionId(sessionId);

            var view = new View("pages/main", new { AjaxRequest = true, ShowPopular = false, CurrentUser = currentUser, Publications = publications });
            return new ControllerResponse(view);
        }

        [HttpGET("^main$")]
        public static ControllerResponse ShowMainPage(Guid sessionId)
        {
            var publications = PublicationController.GetAllPublications().OrderByDescending(p => p.Rating);

            var currentUser = UserController.GetUserBySessionId(sessionId);

            var view = new View("pages/main", new { ShowPopular = true, CurrentUser = currentUser, Publications = publications });
            return new ControllerResponse(view);
        }

        [HttpGET("^auth$")]
        public static ControllerResponse ShowAuthPage(Guid sessionId)
        {
            if (SessionManager.Instance.CheckSession(sessionId))
            {
                var redirectAction =
                    (HttpListenerResponse response) =>
                    {
                        response.Redirect("/main");
                    };
                return new ControllerResponse(action: redirectAction);
            }

            var view = new View("pages/auth", new { EnteredInfo = new { }});
            return new ControllerResponse(view);
        }

        [HttpGET("^register$")]
        public static ControllerResponse ShowRegisterPage(Guid sessionId)
        {
            if (SessionManager.Instance.CheckSession(sessionId))
            {
                var redirectAction =
                    (HttpListenerResponse response) =>
                    {
                        response.Redirect("/main");
                    };
                return new ControllerResponse(action: redirectAction);
            }

            var view = new View("pages/register", new { EnteredInfo = new { } });
            return new ControllerResponse(view);
        }
    }
}
