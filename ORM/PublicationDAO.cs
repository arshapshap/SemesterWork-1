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

        public Publication[] Select() => orm.Select<Publication>();

        public Publication[] SelectWhere(Dictionary<string, object> conditions) => orm.SelectWhere<Publication>(conditions);

        public Publication? Select(int id) => orm.Select<Publication>(id);

        public int Insert(Publication publication) => orm.Insert(publication);
        public void Delete(int id) => orm.Delete(id);
    }
}
