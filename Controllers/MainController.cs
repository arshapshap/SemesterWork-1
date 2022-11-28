using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGET("auth")]
        public static ControllerResponse GetAuthPage()
        {
            var view = new View("auth", new { });
            return new ControllerResponse(view);
        }

        [HttpGET("register")]
        public static ControllerResponse GetRegisterPage()
        {
            var view = new View("register", new { });
            return new ControllerResponse(view);
        }
    }
}
