using HttpServer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    internal class Rating
    {
        public Rating(int publicationId, int userId, int rating, DateTime date)
        {
            PublicationId = publicationId;
            AuthorId = userId;
            DateTime = date;
        }


        [FieldDB("publication_id")]
        public int PublicationId { get; }

        [FieldDB("author_id")]
        public int AuthorId { get; }

        [FieldDB("rating")]
        public int RatingInPoints { get; }

        [FieldDB("date")]
        public string SqlDate { get => DateTime.ToString("yyyy-MM-dd"); }

        public string Date { get => DateTime.ToString("dd.MM.yyyy"); }

        public DateTime DateTime { get; }
    }
}
