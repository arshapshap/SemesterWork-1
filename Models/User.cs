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

        [FieldDB("password")]
        public string HashedPassword { get; }

        [FieldDB("name", isCyrillic: true)]
        public string Name { get; set; }

        [FieldDB("image")]
        public string Image { get; set; }

        public Publication[] Publications { get => PublicationController.GetUserPublications(Id).OrderByDescending(p => p.Rating).ToArray(); }

        public User(int id, string login, string hashedPassword, string name, string image) : this(login, hashedPassword, name, image)
        {
            Id = id;
        }

        public User(string login, string hashedPassword, string name, string image = "~/img/profile.png")
        {
            Login = login;
            HashedPassword = hashedPassword;
            Name = name;
            Image = image;
        }
    }
}
