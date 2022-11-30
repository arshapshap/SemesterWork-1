using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net.Http;
using HttpServer.SettingsService;

namespace HttpServer
{
    public class HttpServer
    {
        private readonly HttpListener listener;
        private bool isWorking;
        private string settingsJsonPath;
        private Settings settings;

        public HttpServer(string settingsPath)
        {
            settingsJsonPath = settingsPath;
            listener = new HttpListener();
            UpdateSettings();
        }

        public void Start()
        {
            if (isWorking)
            {
                Program.PrintMessage("Сервер уже запущен.");
                return;
            }

            UpdateSettings();
            
            listener.Start();
            isWorking = true;

            Program.PrintMessage("Сервер запущен.");

            ProcessingAsync();
        }

        public void Stop()
        {
            if (!isWorking)
            {
                Program.PrintMessage("Сервер уже остановлен.");
                return;
            }
            listener.Stop();
            isWorking = false;

            Program.PrintMessage("Сервер остановлен.");
        }

        private async Task ProcessingAsync()
        {
            try
            {
                var context = await listener.GetContextAsync();

                byte[] buffer;
                var response = ResponseProvider.GetResponse(context, settings.Path, out buffer);

                Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();

                await ProcessingAsync();
            }
            catch (Exception ex)
            {
                if (isWorking)
                {
                    Program.PrintMessage("Произошла ошибка: " + ex.Message);
                    Stop();
                    Start();
                }
                return;
            }
        }

        private void UpdateSettings()
        {
            settings = new Settings();
            if (File.Exists(settingsJsonPath))
                settings = SettingsDeserializer.GetSettings(settingsJsonPath);
            else
                Program.PrintMessage($"Файл настроек не найден по следующему пути: {settingsJsonPath}.");

            listener.Prefixes.Clear();
            listener.Prefixes.Add($"http://localhost:{settings.Port}/");
        }
    }
}