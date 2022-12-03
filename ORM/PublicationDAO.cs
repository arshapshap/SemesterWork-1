using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class PublicationDAO
    {
        readonly MyORM orm;

        public PublicationDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Publications");
        }

        public Publication[] Select() 
            => orm.SelectAsync<Publication>().Result;

        public Publication[] SelectWhere(Dictionary<string, object> conditions) 
            => orm.SelectWhereAsync<Publication>(conditions).Result;

        public Publication? Select(int id) 
            => orm.Select<Publication>(id);

        public int Insert(Publication publication) 
            => orm.InsertAsync(publication).Result;

        public void Update(int id, Publication publication) 
            => orm.UpdateAsync(id, publication);

        public void Delete(int id) => orm.Delete(id);
    }
}
