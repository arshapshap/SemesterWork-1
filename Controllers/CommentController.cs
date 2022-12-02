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
    [ApiController("^comment$")]
    internal class CommentController
    {
        static CommentDAO commentDAO
            = new CommentDAO(MainController.DatabaseConnectionString);

        [HttpPOST("^$")]
        public static ControllerResponse Create(int publicationId, string text, Guid sessionId)
        {
            var user = UserController.GetUserBySessionId(sessionId)
                ?? throw new ServerException(HttpStatusCode.Unauthorized);

            var publication = PublicationController.GetPublication(publicationId)
                ?? throw new ServerException(HttpStatusCode.NotFound);

            var comment = new Comment(publicationId, user.Id, HttpUtility.UrlDecode(text), DateTime.Now);
            commentDAO.Insert(comment);

            var redirectAction = (HttpListenerResponse response)
                => response.Redirect($"/publication/{publicationId}");

            return new ControllerResponse(action: redirectAction);
        }

        [HttpPOST("^delete$")]
        public static ControllerResponse Delete(int commentId, Guid sessionId)
        {
            var user = UserController.GetUserBySessionId(sessionId)
                ?? throw new ServerException(HttpStatusCode.Unauthorized);

            var comment = commentDAO.Select(commentId)
                ?? throw new ServerException(HttpStatusCode.NotFound);

            if (user.Id == comment.AuthorId)
                commentDAO.Delete(comment.Id);
            else
                throw new ServerException(HttpStatusCode.Forbidden);

            var redirectAction = (HttpListenerResponse response)
                => response.Redirect($"/publication/{comment.PublicationId}");

            return new ControllerResponse(action: redirectAction);
        }

        public static Comment[] GetCommentsOnPublication(int publicationId)
            => commentDAO.SelectByPublicationId(publicationId);
    }
}
