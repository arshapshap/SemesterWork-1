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
    [ApiController("publication")]
    internal class PublicationController
    {
        static PublicationDAO publicationDAO
            = new PublicationDAO(MainController.DatabaseConnectionString);

        [HttpPOST("new-musician", onlyForAuthorized: true, needSessionId: true)]
        public static ControllerResponse CreateWithMusician(Guid sessionId, string musicianName, string musicianBiography, string musicianImage, string title, string text)
        {
            MusicianController.Create(musicianName, musicianBiography, musicianImage);
            return Create(sessionId, musicianName, title, text);
        }

        [HttpPOST("new", onlyForAuthorized: true, needSessionId: true)]
        public static ControllerResponse Create(Guid sessionId, string musicianName, string title, string text)
        {
            var user = UserController.GetUserBySessionId(sessionId);

            var musician = MusicianController.GetMusicianByName(musicianName);
            if (musician == null)
                return new ControllerResponse(new View("new-publication", new { NewMusician = true, MusicianName = HttpUtility.UrlDecode(musicianName), Title = HttpUtility.UrlDecode(title), Text = HttpUtility.UrlDecode(text) }));

            var publication = new Publication(user.Id, musician.Id, HttpUtility.UrlDecode(title), HttpUtility.UrlDecode(text), DateTime.Now);
            var publicationId = publicationDAO.Insert(publication);

            var redirectAction = (HttpListenerResponse response) =>
                response.Redirect($"/publication/{publicationId}");

            return new ControllerResponse(null, action: redirectAction);
        }

        [HttpGET(@"\d", needSessionId: true)]
        public static ControllerResponse ShowPublicationById(Guid sessionId, int id)
        {
            var isAuthorized = SessionManager.Instance.CheckSession(sessionId);

            User? currentUser = null;
            if (isAuthorized)
                currentUser = UserController.GetUserBySessionId(sessionId);

            var publication = publicationDAO.Select(id);
            if (publication == null)
                return new ControllerResponse(null, statusCode: HttpStatusCode.NotFound);

            var view = new View("publication", new { CurrentUser = currentUser, Publication = publication });
            return new ControllerResponse(view);
        }

        [HttpGET("^$", onlyForAuthorized: true)]
        public static ControllerResponse ShowCreatePublicationPage()
        {
            var view = new View("new-publication");
            return new ControllerResponse(view);
        }

        public static Publication[] GetUserPublications(int userId) => publicationDAO.SelectByUserId(userId);

        public static Publication[] GetPublications() => publicationDAO.Select();
    }
}
