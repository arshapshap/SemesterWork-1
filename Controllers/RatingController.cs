
using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpServer.Controllers
{
    [ApiController("^rating$")]
    internal class RatingController
    {
        static RatingDAO ratingDAO
            = new RatingDAO(MainController.DatabaseConnectionString);

        [HttpPOST("^$", onlyForAuthorized: true, needSessionId: true)]
        public static ControllerResponse Create(Guid sessionId, int publicationId, int points)
        {
            var user = UserController.GetUserBySessionId(sessionId);
            var publication = PublicationController.GetPublication(publicationId);

            if (publication == null)
                return new ControllerResponse(null, statusCode: HttpStatusCode.NotFound);

            if (user.Id == publication.AuthorId || GetRating(publicationId, user.Id) != null)
                return new ControllerResponse(null, statusCode: HttpStatusCode.Forbidden);

            var rating = new Rating(publicationId, user.Id, points, DateTime.Now);
            ratingDAO.Insert(rating);

            var redirectAction = (HttpListenerResponse response)
                => response.Redirect($"/publication/{publicationId}");

            return new ControllerResponse(null, action: redirectAction);
        }

        public static Rating[] GetRatingsOnPublication(int publicationId)
            => ratingDAO.SelectByPublicationId(publicationId);

        public static Rating? GetRating(int publicationId, int authorId) => ratingDAO.Select(publicationId, authorId);
    }
}
