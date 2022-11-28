using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    [ApiController("^$")]
    internal class MainController
    {

        [HttpGET("main")]
        public static ControllerResponse GetMainPage()
        {
            var view = new View("main", new { });
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

            var view = new View("auth", new { });
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

            var view = new View("register", new { });
            return new ControllerResponse(view);
        }
    }
}
