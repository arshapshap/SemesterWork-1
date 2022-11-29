using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class MusicianDAO
    {
        readonly MyORM orm;

        public MusicianDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Musicians");
        }

        public Musician[] Select() => orm.Select<Musician>();

        public Musician[] SelectWhere(Dictionary<string, object> conditions) => orm.SelectWhere<Musician>(conditions);

        public Musician? Select(int id) => orm.Select<Musician>(id);

        public Musician? Select(string name) => orm.SelectWhere<Musician>(new Dictionary<string, object> { { "name", name } }).FirstOrDefault();

        public int Insert(Musician musician) => orm.Insert(musician);
    }
}
