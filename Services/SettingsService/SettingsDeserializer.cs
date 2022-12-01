﻿using System;
using System.Text.Json;

namespace HttpServer.SettingsService
{
    static class SettingsDeserializer
    {
        public static Settings GetSettings(string path)
        {
            var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllBytes(path));

            if (settings is null)
                return new Settings();
            return settings;
        }
    }
}
