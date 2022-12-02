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
    [ApiController("^publication$")]
    internal class PublicationController
    {
        static PublicationDAO publicationDAO
            = new PublicationDAO(MainController.DatabaseConnectionString);

        [HttpPOST("^new-musician$")]
        public static ControllerResponse CreateWithMusician(string musicianName, string musicianBiography, string musicianImage, string title, string text, Guid sessionId)
        {
            MusicianController.Create(musicianName, musicianBiography, musicianImage);
            return Create(musicianName, title, text, sessionId);
        }

        [HttpPOST("^new$")]
        public static ControllerResponse Create(string musicianName, string title, string text, Guid sessionId)
        {
            var currentUser = UserController.GetUserBySessionId(sessionId)
                ?? throw new ServerException(HttpStatusCode.Unauthorized);

            var musician = MusicianController.GetMusicianByName(musicianName);
            if (musician is null)
            {
                var entered = new {
                    MusicianName = HttpUtility.UrlDecode(musicianName),
                    PublicationTitle = HttpUtility.UrlDecode(title),
                    PublicationText = HttpUtility.UrlDecode(text)
                };
                return new ControllerResponse(new View("pages/new-publication", new { NewMusician = true, EnteredInfo = entered, CurrentUser = currentUser }));
            }

            var publication = new Publication(currentUser.Id, musician.Id, HttpUtility.UrlDecode(title), HttpUtility.UrlDecode(text), DateTime.Now);
            var publicationId = publicationDAO.Insert(publication);

            var redirectAction = (HttpListenerResponse response) 
                => response.Redirect($"/publication/{publicationId}");

            return new ControllerResponse(action: redirectAction);
        }

        [HttpPOST("^delete$")]
        public static ControllerResponse Delete(int publicationId, Guid sessionId)
        {
            var currentUser = UserController.GetUserBySessionId(sessionId)
                ?? throw new ServerException(HttpStatusCode.Unauthorized);

            var publication = publicationDAO.Select(publicationId)
                ?? throw new ServerException(HttpStatusCode.NotFound);

            if (currentUser.Id == publication.AuthorId)
                publicationDAO.Delete(publicationId);
            else
                throw new ServerException(HttpStatusCode.Forbidden);

            var redirectAction = (HttpListenerResponse response) 
                => response.Redirect($"/main");

            return new ControllerResponse(action: redirectAction);
        }

        [HttpGET(@"^\d+$")]
        public static ControllerResponse ShowPublication(int id, int focusedCommentId, Guid sessionId)
        {
            var currentUser = UserController.GetUserBySessionId(sessionId);

            var publication = publicationDAO.Select(id)
                ?? throw new ServerException(HttpStatusCode.NotFound);

            var isRatingAvailable = !(currentUser is not null && (publication.AuthorId == currentUser.Id || RatingController.GetRating(id, currentUser.Id) is not null));

            if (publication.Comments.Where(c => c.Id == focusedCommentId).FirstOrDefault() == null)
                focusedCommentId = publication.Comments.Where(c => c.Id < focusedCommentId).FirstOrDefault()?.Id ?? 0;

            var view = new View("pages/publication", new { FocusedCommentId = focusedCommentId, Publication = publication, CurrentUser = currentUser, IsRatingAvailable = isRatingAvailable });
            return new ControllerResponse(view);
        }

        [HttpGET("^$")]
        public static ControllerResponse ShowCreationPublicationPage(Guid sessionId)
        {
            var currentUser = UserController.GetUserBySessionId(sessionId)
                ?? throw new ServerException(HttpStatusCode.Unauthorized);

            var view = new View("pages/new-publication", new { CurrentUser = currentUser, EnteredInfo = new { } });
            return new ControllerResponse(view);
        }

        public static Publication[] GetUserPublications(int userId) => publicationDAO.SelectWhere(new Dictionary<string, object> { { "user_id", userId } });

        public static Publication[] GetMusicianPublications(int musicianId) => publicationDAO.SelectWhere(new Dictionary<string, object> { { "musician_id", musicianId } });

        public static Publication? GetPublication(int id) => publicationDAO.Select(id);
        public static Publication[] GetPublications() => publicationDAO.Select();
    }
}
