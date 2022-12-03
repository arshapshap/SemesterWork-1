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

        public Rating? Select(int publicationId, int userId) 
            => orm.SelectWhereAsync<Rating>(new Dictionary<string, object> 
        { 
            { "publication_id", publicationId },
            { "user_id", userId }
        }).Result.FirstOrDefault();

        public Rating[] SelectByPublicationId(int publicationId) 
            => orm.SelectWhereAsync<Rating>(new Dictionary<string, object> { { "publication_id", publicationId } }).Result;

        public void Insert(Rating comment) 
            => orm.InsertAsync(comment, false);
    }
}
