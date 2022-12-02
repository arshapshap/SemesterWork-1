
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

        [HttpPOST("^$")]
        public static ControllerResponse Create(int publicationId, int points, Guid sessionId)
        {
            var currentUser = UserController.GetUserBySessionId(sessionId)
                ?? throw new ServerException(HttpStatusCode.Unauthorized);

            var publication = PublicationController.GetPublication(publicationId)
                ?? throw new ServerException(HttpStatusCode.NotFound);

            if (currentUser.Id == publication.AuthorId || GetRating(publicationId, currentUser.Id) is not null)
                throw new ServerException(HttpStatusCode.Forbidden);

            var rating = new Rating(publicationId, currentUser.Id, points, DateTime.Now);
            ratingDAO.Insert(rating);

            var redirectAction = (HttpListenerResponse response)
                => response.Redirect($"/publication/{publicationId}");

            return new ControllerResponse(action: redirectAction);
        }

        public static Rating[] GetRatingsOnPublication(int publicationId)
            => ratingDAO.SelectByPublicationId(publicationId);

        public static Rating? GetRating(int publicationId, int authorId) 
            => ratingDAO.Select(publicationId, authorId);
    }
}
