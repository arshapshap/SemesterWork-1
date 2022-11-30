using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class UserDAO
    {
        readonly MyORM orm;

        public UserDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Users");
        }

        public User? Select(string login, string password) 
            => orm.SelectWhere<User>(new Dictionary<string, object>() { { "login", login }, { "password", password } }).FirstOrDefault();

        public User? Select(int id) => orm.Select<User>(id);

        public User? Select(string login) => orm.SelectWhere<User>(new Dictionary<string, object> { { "login", login } }).FirstOrDefault();

        public int Insert(User account) => orm.Insert(account);

        public void Update(int id, User account) => orm.Update(id, account);
    }
}
