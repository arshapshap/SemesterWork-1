using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    class HttpPOST : HttpRequest
    {
        public HttpPOST(string methodURI = "", bool onlyForAuthorized = false, bool needSessionId = false) : base(methodURI, onlyForAuthorized, needSessionId) { }
    }
}
