using HttpServer.Models;
using HttpServer.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpServer.Controllers
{
    internal class MusicianController
    {
        static MusicianDAO musicianDAO
            = new MusicianDAO(MainController.DatabaseConnectionString);

        public static Musician? GetMusicianByName(string name)
            => musicianDAO.Select(HttpUtility.UrlDecode(name));
        public static Musician? GetMusicianById(int id)
            => musicianDAO.Select(id);

        public static void Create(string name, string biography, string image)
        {
            musicianDAO.Insert(new Musician(HttpUtility.UrlDecode(name), HttpUtility.UrlDecode(biography), HttpUtility.UrlDecode(image)));
        }
    }
}
