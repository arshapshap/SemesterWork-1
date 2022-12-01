using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using HttpServer.SessionsService;
using HttpServer.TemplatesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpServer.Controllers
{
    [ApiController("^musician$")]
    internal class MusicianController
    {
        static MusicianDAO musicianDAO
            = new MusicianDAO(MainController.DatabaseConnectionString);

        [HttpGET(@"^\d+$")]
        public static ControllerResponse ShowMusician(int id, Guid sessionId)
        {
            var currentUser = UserController.GetUserBySessionId(sessionId);

            var musician = musicianDAO.Select(id);
            if (musician is null)
                throw new ServerException(HttpStatusCode.NotFound);

            var view = new View("pages/musician", new { Musician = musician, CurrentUser = currentUser });
            return new ControllerResponse(view);
        }

        public static Musician? GetMusicianByName(string name)
            => musicianDAO.Select(HttpUtility.UrlDecode(name));

        public static Musician? GetMusicianById(int id)
            => musicianDAO.Select(id);

        public static void Create(string name, string biography, string image)
            => musicianDAO.Insert(new Musician(HttpUtility.UrlDecode(name), HttpUtility.UrlDecode(biography), HttpUtility.UrlDecode(image)));
    }
}
