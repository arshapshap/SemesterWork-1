using System;

namespace HttpServer.SettingsService
{
    class Settings
    {
        public string Path { get; set; } = "./site";
        public int Port { get; set; } = 8080;
    }
}