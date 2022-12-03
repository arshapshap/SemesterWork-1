using HttpServer.SessionsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class SessionDAO
    {
        readonly MyORM orm;

        public SessionDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Sessions");
        }

        public Session? Select(Guid guid) 
            => orm.Select<Session>("guid", guid.ToString());

        public void Insert(Session session) 
            => orm.InsertAsync(session, idExists: false);

        public void Delete(Guid guid) => orm.Delete("guid", guid.ToString());
    }
}
