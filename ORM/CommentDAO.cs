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

        public Comment[] SelectByPublicationId(int publicationId) 
            => orm.SelectWhereAsync<Comment>(new Dictionary<string, object> { { "publication_id", publicationId } }).Result;

        public int Insert(Comment comment) 
            => orm.InsertAsync(comment).Result;

        public void Delete(int id) => orm.Delete(id);
    }
}
