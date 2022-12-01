using HttpServer.Attributes;
using HttpServer.Controllers;
using HttpServer.SessionsService;
using HttpServer.TemplatesService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class ResponseProvider
    {
        public static HttpListenerResponse GetResponse(HttpListenerContext context, string sitePath, out byte[] buffer)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            (byte[] buffer, string contentType)? serverResponse;

            try
            {
                serverResponse = HandleRequest(request, response, sitePath);
            }
            catch (Exception exception)
            {
                var statusCode = (exception is ServerException) ? ((ServerException)exception).StatusCode : HttpStatusCode.InternalServerError;

                serverResponse = GetErrorServerResponse(statusCode, sitePath);
                response.StatusCode = (int)statusCode;

                if (exception is not ServerException)
                    Program.PrintMessage("Произошла ошибка: " + exception.Message);
            }

            response.Headers.Set("Content-Type", serverResponse?.contentType);
            response.ContentLength64 = serverResponse?.buffer?.Length ?? 0;
            buffer = serverResponse?.buffer ?? Array.Empty<byte>();

            return response;
        }

        private static (byte[] buffer, string contentType)? HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string sitePath)
        {
            if (request.RawUrl.Contains('~'))
            {
                var filePath = sitePath + request.RawUrl.Replace("%20", " ").Split("~").Last();
                return FileLoader.GetFile(filePath);
            }

            if (request.Url.Segments.Length < 2)
            {
                response.Redirect(@"/main");
                return null;
            }

            string[] strParams = request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            var controllerURI = request.Url.Segments[1].Replace("/", "");
            var methodURI = (strParams.Length > 0) ? strParams[0] : "";

            if (!TryFindMethod(controllerURI, methodURI, request.HttpMethod, out MethodInfo method))
                throw new ServerException(HttpStatusCode.NotFound);

            if (request.HttpMethod == "POST")
            {
                var postData = GetRequestPostData(request);
                strParams = (postData is not null) ? postData.Split('&').Select(p => p.Split('=')[1]).ToArray() : new string[0];
            }

            Guid? sessionId = GetSessionId(request);

            var httpRequestAttribute = (HttpRequest)method.GetCustomAttribute(typeof(HttpRequest));

            var queryParams = method.GetParameters()
                .SkipLast(1)
                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                .Append(sessionId)
                .ToArray();

            var methodResponse = (ControllerResponse)method.Invoke(null, queryParams);

            methodResponse.Action.Invoke(response);

            var view = methodResponse.View;

            if (view is not null)
            {
                view.CurrentUser = UserController.GetUserBySessionId(sessionId ?? Guid.Empty);
                return (Encoding.UTF8.GetBytes(view.GetHTML(sitePath)), "text/html");
            }

            return null;
        }

        private static bool TryFindMethod(string controllerURI, string methodURI, string httpMethod, out MethodInfo? method)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var controller = FindControllerByURI(assembly, "");
            method = FindMethodByURI(controller, httpMethod, controllerURI);

            if (method is not null)
                return true;

            controller = FindControllerByURI(assembly, controllerURI);

            if (controller is null)
                return false;

            method = FindMethodByURI(controller, httpMethod, methodURI);

            if (method is null)
                return false;

            return true;
        }

        private static Guid? GetSessionId(HttpListenerRequest request)
        {
            var sessionCookie = request.Cookies.Where(cookie => cookie.Name == "SessionId").FirstOrDefault();
            Guid? sessionId = null;
            if (sessionCookie is not null &&
                SessionManager.Instance.CheckSession(Guid.Parse(sessionCookie.Value)))
                sessionId = Guid.Parse(sessionCookie.Value);
            return sessionId;
        }

        private static Type? FindControllerByURI(Assembly assembly, string uri)
        {
            return assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ApiController))
                    && Regex.IsMatch(uri, ((ApiController?)t.GetCustomAttribute(typeof(ApiController)))?.ClassURI ?? ""))
                .FirstOrDefault();
        }

        private static MethodInfo? FindMethodByURI(Type controller, string httpMethod, string methodURI)
        {
            return controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == $"Http{httpMethod}"
                    && Regex.IsMatch(methodURI, (((HttpRequest)attr).MethodURI == "") ? t.Name.ToLower() : ((HttpRequest)attr).MethodURI)))
                .FirstOrDefault();
        }

        private static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
                return null;
            using (Stream body = request.InputStream)
            {
                using (var reader = new StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static (byte[] buffer, string contentType) GetErrorServerResponse(HttpStatusCode statusCode, string path)
        {
            string errorDescription;

            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    errorDescription = "Пользователь не авторизован";
                    break;
                case HttpStatusCode.Forbidden:
                    errorDescription = "Нет доступа";
                    break;
                case HttpStatusCode.NotFound:
                    errorDescription = "Страница не найдена";
                    break;
                case HttpStatusCode.InternalServerError:
                    errorDescription = "Ошибка сервера";
                    break;
                default:
                    errorDescription = statusCode.ToString();
                    break;
            }

            var view = new View("pages/error", new { ErrorCode = (int)statusCode, ErrorDescription = errorDescription });
            return (Encoding.UTF8.GetBytes(view.GetHTML(path)), "text/html");
        }
    }

}