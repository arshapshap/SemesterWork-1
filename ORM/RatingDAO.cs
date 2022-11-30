using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class RatingDAO
    {
        readonly MyORM orm;

        public RatingDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Ratings");
        }

        public Rating? Select(int publicationId, int userId) => orm.SelectWhere<Rating>(new Dictionary<string, object> 
        { 
            { "publication_id", publicationId },
            { "user_id", userId }
        }).FirstOrDefault();

        public Rating[] SelectByPublicationId(int publicationId) => orm.SelectWhere<Rating>(new Dictionary<string, object> { { "publication_id", publicationId } });

        public void Insert(Rating comment) => orm.Insert(comment, false);
    }
}
