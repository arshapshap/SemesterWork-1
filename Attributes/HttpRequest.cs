using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    abstract class HttpRequest : Attribute
    {
        public string MethodURI;

        protected HttpRequest(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}
