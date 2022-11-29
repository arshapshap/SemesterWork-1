using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using HttpServer.SessionsService;
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

        [HttpGET(@"^\d*$")]
        public static ControllerResponse ShowMusician(int id)
        {
            var musician = musicianDAO.Select(id);
            if (musician == null)
                return new ControllerResponse(null, statusCode: HttpStatusCode.NotFound);

            var view = new View("musician", new { Musician = musician });
            return new ControllerResponse(view);
        }

        public static Musician? GetMusicianByName(string name)
            => musicianDAO.Select(HttpUtility.UrlDecode(name));

        public static Musician? GetMusicianById(int id)
            => musicianDAO.Select(id);

        public static void Create(string name, string biography, string image)
        {
            musicianDAO.Insert(new Musician(HttpUtility.UrlDecode(name), HttpUtility.UrlDecode(biography), HttpUtility.UrlDecode(image)));
        }
    }
}
