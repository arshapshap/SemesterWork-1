using HttpServer.Attributes;
using HttpServer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    internal class Comment
    {
        public Comment(int publicationId, int userId, string text, DateTime date)
        {
            PublicationId = publicationId;
            AuthorId = userId;
            Text = text;
            DateTime = date;
        }

        public Comment(int id, int publicationId, int userId, string text, DateTime date) : this(publicationId, userId, text, date)
        {
            Id = id;
        }

        public int Id { get; }

        [FieldDB("publication_id")]
        public int PublicationId { get; }

        [FieldDB("user_id")]
        public int AuthorId { get; }

        [FieldDB("text", isCyrillic: true)]
        public string Text { get; set; }

        [FieldDB("date")]
        public string SqlDate { get => DateTime.ToString("yyyy-MM-dd HH:mm:ss"); }

        public string Date { get => DateTime.ToString("HH:mm dd.MM.yyyy"); }

        public DateTime DateTime { get; }

        public User? Author { get => UserController.GetUserById(AuthorId); }

        public bool IsEditing { get; set; }
    }
}
