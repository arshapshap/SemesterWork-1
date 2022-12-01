﻿using Microsoft.Extensions.Caching.Memory;
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
        private SessionManager()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
        }

        public Session CreateSession(int accountId, string login)
        {
            var session = new Session(Guid.NewGuid(), accountId, login, DateTime.Now);
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10*60));
            cache.Set(session.Guid, session, cacheEntryOptions);

            return session;
        }

        public void RemoveSession(Guid guid) => cache.Remove(guid);

        public bool CheckSession(Guid guid) => cache.Get(guid) is not null;

        public Session GetSession(Guid guid) => (Session)cache.Get(guid);
    }
}
