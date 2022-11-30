using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class CommentDAO
    {
        readonly MyORM orm;

        public CommentDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Comments");
        }

        public Comment? Select(int id) => orm.Select<Comment>(id);

        public Comment[] SelectByPublicationId(int publicationId) => orm.SelectWhere<Comment>(new Dictionary<string, object> { { "publication_id", publicationId } });

        public int Insert(Comment comment) => orm.Insert(comment);

        public void Delete(int id) => orm.Delete(id);
    }
}
