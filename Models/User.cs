using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class User
    {
        public int Id { get; set; }
        [FieldDB("login")]
        public string Login { get; set; }
        [FieldDB("password", isCyrillic: true)]
        public string Password { get; set; }
        [FieldDB("name", isCyrillic: true)]
        public string Name { get; set; }
        [FieldDB("image")]
        public string Image { get; set; }

        public User(int id, string login, string password, string name, string image) : this(login, password, name, image)
        {
            Id = id;
        }

        public User(string login, string password, string name, string image = "")
        {
            Login = login;
            Password = password;
            Name = name;
            Image = image;
        }
    }
}
