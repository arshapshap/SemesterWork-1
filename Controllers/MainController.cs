using HttpServer.Attributes;
using HttpServer.SessionsService;
using HttpServer.TemplateService;
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

        [HttpGET("^(main|popular)$", needSessionId: true)]
        public static ControllerResponse ShowPopularPublications(Guid sessionId)
            => ShowMainPage(sessionId, showPopular: true);

        [HttpGET("^new$", needSessionId: true)]
        public static ControllerResponse ShowNewPublications(Guid sessionId)
            => ShowMainPage(sessionId, showPopular: false);

        public static ControllerResponse ShowMainPage(Guid sessionId, bool showPopular)
        {
            var publications = PublicationController.GetPublications();

            var ordered = (showPopular)
                ? publications.OrderByDescending(p => p.Rating)
                : publications.OrderByDescending(p => p.Id);

            var currentUser = UserController.GetUserBySessionId(sessionId);

            var view = new View("pages/main", new { ShowPopular = showPopular, CurrentUser = currentUser, Publications = ordered });
            return new ControllerResponse(view);
        }

        [HttpGET("^auth$", needSessionId:true)]
        public static ControllerResponse ShowAuthPage(Guid sessionId)
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

            var view = new View("pages/auth", new { EnteredInfo = new { }});
            return new ControllerResponse(view);
        }

        [HttpGET("^register$", needSessionId:true)]
        public static ControllerResponse ShowRegisterPage(Guid sessionId)
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

            var view = new View("pages/register", new { EnteredInfo = new { } });
            return new ControllerResponse(view);
        }
    }
}
