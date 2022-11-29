using HttpServer.Attributes;
using HttpServer.Controllers;
using HttpServer.SessionsService;
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
            (byte[] buffer, string contentType) serverResponse;

            try
            {
                if (!TryHandleRequest(request, response, sitePath, out serverResponse))
                {
                    string filePath = sitePath + request.RawUrl.Replace("%20", " ").Split("~").Last();
                    if (!FileLoader.TryGetResponse(filePath, out serverResponse))
                    {
                        serverResponse = GetErrorServerResponse(HttpStatusCode.NotFound, sitePath);
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        Program.PrintMessage($"Ресурс не найден по следующему пути: {filePath}.");
                    }
                }
            }
            catch (Exception ex)
            {
                serverResponse = GetErrorServerResponse(HttpStatusCode.InternalServerError, sitePath);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Program.PrintMessage("Произошла ошибка: " + ex.Message);
            }

            response.Headers.Set("Content-Type", serverResponse.contentType);
            response.ContentLength64 = serverResponse.buffer.Length;

            buffer = serverResponse.buffer;

            return response;
        }

        private static bool TryHandleRequest(HttpListenerRequest request, HttpListenerResponse response, string sitePath, out (byte[] buffer, string contentType) serverResponse)
        {
            serverResponse = (new byte[0], "");
            if (request.Url.AbsolutePath.Contains('~'))
                return false;
            if (request.Url.Segments.Length < 2)
            {
                response.Redirect(@"/main");
                return true;
            }

            string[] strParams = request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            var controllerURI = request.Url.Segments[1].Replace("/", "");
            var methodURI = (strParams.Length > 0) ? strParams[0] : "";

            if (!TryFindMethod(controllerURI, methodURI, request.HttpMethod, out MethodInfo method))
                return false;

            if (request.HttpMethod == "POST")
            {
                var postData = GetRequestPostData(request);
                strParams = postData.Split('&').Select(p => p.Split('=')[1]).ToArray();
            }

            object?[] queryParams = null;
            Guid? sessionId = GetSessionId(request);

            var httpRequestAttribute = (HttpRequest)method.GetCustomAttribute(typeof(HttpRequest));
            if (httpRequestAttribute?.OnlyForAuthorized == true && sessionId == null)
            {
                serverResponse = GetErrorServerResponse(HttpStatusCode.Unauthorized, sitePath);
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return true;
            }

            if (httpRequestAttribute?.NeedSessionId == true)
                queryParams = method.GetParameters()
                        .Skip(1)
                        .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                        .Prepend(sessionId)
                        .ToArray();
            else
                queryParams = method.GetParameters()
                               .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                               .ToArray();

            var methodResponse = (ControllerResponse)method.Invoke(null, queryParams);

            if (methodResponse.statusCode != HttpStatusCode.OK)
            {
                serverResponse = GetErrorServerResponse(methodResponse.statusCode, sitePath);
                response.StatusCode = (int)methodResponse.statusCode;
                return true;
            }

            methodResponse.action.Invoke(response);

            if (methodResponse.response is View)
            {
                var view = (View)methodResponse.response;
                if (sessionId != null)
                    view.CurrentUser = UserController.GetUserBySessionId((Guid)sessionId);
                serverResponse = (Encoding.UTF8.GetBytes(view.GetHTML(sitePath)), "text/html");
            }
            else
                serverResponse = (Encoding.ASCII.GetBytes(JsonSerializer.Serialize(methodResponse.response)), "application/json");
            return true;
        }

        private static bool TryFindMethod(string controllerURI, string methodURI, string httpMethod, out MethodInfo method)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var controller = FindControllerByURI(assembly, "");
            method = FindMethodByURI(controller, httpMethod, controllerURI);

            if (method != null)
                return true;

            controller = FindControllerByURI(assembly, controllerURI);

            if (controller == null)
                return false;

            method = FindMethodByURI(controller, httpMethod, methodURI);

            if (method == null)
                return false;

            return true;
        }

        private static Guid? GetSessionId(HttpListenerRequest request)
        {
            var sessionCookie = request.Cookies.Where(cookie => cookie.Name == "SessionId").FirstOrDefault();
            Guid? sessionId = null;
            if (sessionCookie != null &&
                SessionManager.Instance.CheckSession(Guid.Parse(sessionCookie.Value)))
                sessionId = Guid.Parse(sessionCookie.Value);
            return sessionId;
        }

        private static Type? FindControllerByURI(Assembly assembly, string uri)
        {
            return assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(ApiController)) 
                    && Regex.IsMatch(uri, ((ApiController?)t.GetCustomAttribute(typeof(ApiController)))?.ClassURI))
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

            var view = new View("error", new { ErrorCode = (int)statusCode, ErrorDescription = errorDescription });
            return (Encoding.UTF8.GetBytes(view.GetHTML(path)), "text/html");
        }
    }

}