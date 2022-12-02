using HttpServer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.SessionsService
{
    internal class Session
    {
        [FieldDB("guid")]
        public string GuidString { get => Guid.ToString(); }

        [FieldDB("account_id")]
        public int AccountId { get; }

        [FieldDB("datetime")]
        public string SqlDateTime { get => Created.ToString("yyyy-MM-dd HH:mm:ss"); }

        public Guid Guid { get; }

        public DateTime Created { get; }

        public Session(string guidString, int accountId, DateTime created) : this(Guid.Parse(guidString), accountId, created) { }

        public Session(Guid guid, int accountId, DateTime created)
        {
            Guid = guid;
            AccountId = accountId;
            Created = created;
        }
    }
}
