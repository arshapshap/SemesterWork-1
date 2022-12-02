using HttpServer.Controllers;
using HttpServer.ORM;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.SessionsService
{
    internal class SessionManager
    {
        private static SessionManager instance;
        public static SessionManager Instance
        {
            get {
                if (instance is null)
                    instance = new SessionManager();
                return instance;
            }
        }

        private readonly MemoryCache cache;
        private readonly SessionDAO sessionDAO;
        private SessionManager()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
            sessionDAO = new SessionDAO(MainController.DatabaseConnectionString);
        }

        public Session CreateSession(int accountId, bool rememberMe)
        {
            var session = new Session(Guid.NewGuid(), accountId, DateTime.Now);
            cache.Set(session.Guid, session);

            if (rememberMe)
                sessionDAO.Insert(session);

            return session;
        }

        public void RemoveSession(Guid guid)
        {
            cache.Remove(guid);
            sessionDAO.Delete(guid);
        }

        public bool CheckSession(Guid guid)
            => (cache.Get(guid) is not null) || (sessionDAO.Select(guid) is not null);

        public Session? GetSession(Guid guid)
        {
            if (cache.Get(guid) is not null)
                return (Session)cache.Get(guid);

            return sessionDAO.Select(guid);
        }
    }
}
