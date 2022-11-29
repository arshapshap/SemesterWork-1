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
        public readonly bool OnlyForAuthorized;
        public readonly bool NeedSessionId;
        protected HttpRequest(string methodURI, bool onlyForAuthorized, bool needSessionId)
        {
            MethodURI = methodURI;
            OnlyForAuthorized = onlyForAuthorized;
            NeedSessionId = needSessionId;
        }
    }
}
