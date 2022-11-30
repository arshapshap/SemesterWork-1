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

        [HttpPOST("^$", onlyForAuthorized: true, needSessionId: true)]
        public static ControllerResponse CreateComment(Guid sessionId, int publicationId, string text)
        {
            var user = UserController.GetUserBySessionId(sessionId);

            var comment = new Comment(publicationId, user.Id, HttpUtility.UrlDecode(text), DateTime.Now);
            commentDAO.Insert(comment);

            var redirectAction = (HttpListenerResponse response)
                => response.Redirect($"/publication/{publicationId}");

            return new ControllerResponse(null, action: redirectAction);
        }

        [HttpPOST("^delete$", onlyForAuthorized: true, needSessionId: true)]
        public static ControllerResponse Delete(Guid sessionId, int commentId)
        {
            var user = UserController.GetUserBySessionId(sessionId);
            var comment = commentDAO.Select(commentId);

            if (comment == null)
                return new ControllerResponse(null, statusCode: HttpStatusCode.NotFound);

            if (user.Id == comment.AuthorId)
                commentDAO.Delete(comment.Id);
            else
                return new ControllerResponse(null, statusCode: HttpStatusCode.Forbidden);

            var redirectAction = (HttpListenerResponse response)
                => response.Redirect($"/publication/{comment.PublicationId}");

            return new ControllerResponse(null, action: redirectAction);
        }

        public static Comment[] GetCommentsOnPublication(int publicationId)
            => commentDAO.SelectByPublicationId(publicationId);
    }
}
