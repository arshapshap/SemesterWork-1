using HttpServer.Attributes;
using HttpServer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HttpServer.Models
{
    internal class Musician
    {
        public int Id { get; }

        [FieldDB("name", isCyrillic: true)]
        public string Name { get; }

        [FieldDB("biography", isCyrillic: true)]
        public string Biography { get; set; }

        [FieldDB("image")]
        public string Image { get; set; }

        public Publication[] Publications { get => PublicationController.GetMusicianPublications(Id).OrderByDescending(p => p.Rating).ToArray(); }

        public Musician(int id, string name, string biography, string image) : this(name, biography, image)
        {
            Id = id;
        }

        public Musician(string name, string biography, string image = "")
        {
            Name = name;
            Biography = biography;
            Image = image;
        }
    }
}
