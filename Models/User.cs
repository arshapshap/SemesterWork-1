using HttpServer.Attributes;
using HttpServer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    class User
    {
        public int Id { get; }

        [FieldDB("login")]
        public string Login { get; }

        [FieldDB("password", isCyrillic: true)]
        public string Password { get; }

        [FieldDB("name", isCyrillic: true)]
        public string Name { get; set; }

        [FieldDB("image")]
        public string Image { get; set; }

        public Publication[] Publications { get => PublicationController.GetUserPublications(Id); }

        public User(int id, string login, string password, string name, string image) : this(login, password, name, image)
        {
            Id = id;
        }

        public User(string login, string password, string name, string image = "~/img/profile.png")
        {
            Login = login;
            Password = password;
            Name = name;
            Image = image;
        }
    }
}
