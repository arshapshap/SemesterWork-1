using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class ServerException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public ServerException(HttpStatusCode statusCode, string message = "") : base((message == "") ? statusCode.ToString() : message)
        {
            StatusCode = statusCode;
        }
    }
}
