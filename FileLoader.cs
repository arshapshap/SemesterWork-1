using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    static class FileLoader
    {
        public static (byte[] buffer, string contentType) GetFile(string filePath)
        {
            byte[] buffer;
            string contentType;

            if (!File.Exists(filePath))
                throw new ServerException(HttpStatusCode.NotFound, $"Не найдено: {filePath}.");

            buffer = File.ReadAllBytes(filePath);
            contentType = filePath.Split('.').Last();

            return (buffer, $"text/{contentType}");
        }
    }
}
