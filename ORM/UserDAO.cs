using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class UserDAO
    {
        readonly MyORM orm;

        public UserDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Users");
        }

        public User[] Select() => orm.Select<User>();

        public User[] SelectWhere(Dictionary<string, object> conditions) => orm.SelectWhere<User>(conditions);

        public User? Select(string login, string password) 
            => orm.SelectWhere<User>(new Dictionary<string, object>() { { "login", login }, { "password", password } }).FirstOrDefault();

        public User? Select(int id) => orm.Select<User>(id);

        public void Insert(User account) => orm.Insert(account);

        public void Update(int id, User account) => orm.Update(id, account);

        public void Delete(int id) => orm.Delete(id);
    }
}
