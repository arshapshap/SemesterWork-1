using HttpServer.Attributes;
using HttpServer.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    internal class Publication
    {
        public Publication(int userId, int musicianId, string title, string text, DateTime date)
        {
            AuthorId = userId;
            MusicianId = musicianId;
            Title = title;
            Text = Format(text);
            DateTime = date;
        }

        public Publication(int id, int userId, int musicianId, string title, string text, DateTime date) : this(userId, musicianId, title, text, date)
        {
            Id = id;
        }

        public int Id { get; }

        [FieldDB("user_id", isCyrillic: true)]
        public int AuthorId { get; }

        [FieldDB("musician_id", isCyrillic: true)]
        public int MusicianId { get; }

        [FieldDB("title", isCyrillic: true)]
        public string Title { get;}

        [FieldDB("text", isCyrillic: true)]
        public string Text { get; }

        [FieldDB("date")]
        public string SqlDate { get => DateTime.ToString("yyyy-MM-dd"); }

        public string Date { get => DateTime.ToString("dd.MM.yyyy"); }

        public DateTime DateTime { get; }

        public double Rating { get { 
                var ratings = RatingController.GetRatingsOnPublication(Id);
                return (ratings.Length > 0) ? ratings.Average(r => r.Points) : 0;
            } }
        public string RatingString { get => Rating.ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("en-US")); }

        public Comment[] Comments { get => CommentController.GetCommentsOnPublication(Id).OrderByDescending(c => c.Id).ToArray() ; }

        public User? Author { get => UserController.GetUserById(AuthorId);}

        public Musician Musician { get => MusicianController.GetMusicianById(MusicianId); }

        private static string Format(string text)
        {
            var chordRegex = @"\b[ABCDEFGH][b#]?[m]?(2|5|6|7|9|11|13|6/9|7-5|7-9|7#5|7#9|7+5|7+9|7b5|7b9|7sus2|7sus4|add2|add4|add9|aug|dim|dim7|maj7|m6|m7|m7b5|m9|m11|m13|maj|maj7|maj9|maj11|maj13|mb5|sus|sus2|sus4){0,2}\b";
            var matches = Regex.Matches(text, chordRegex).DistinctBy(m => m.Value).OrderByDescending(m => m.Value.Length).ToArray();
            string result = text;
            foreach (Match match in matches)
                result = Regex.Replace(result, $@"\b{match.Value}\b", $"<b>{match.Value}</b>");

            return result;
        }
    }
}
