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

        public Musician[] Select() 
            => orm.SelectAsync<Musician>().Result;

        public Musician? Select(int id) 
            => orm.Select<Musician>(id);

        public Musician? Select(string name) 
            => orm.SelectWhereAsync<Musician>(new Dictionary<string, object> { { "name", name } }).Result.FirstOrDefault();

        public int Insert(Musician musician) 
            => orm.InsertAsync(musician).Result;
    }
}
