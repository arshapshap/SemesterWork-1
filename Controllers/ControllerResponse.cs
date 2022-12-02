using HttpServer.TemplatesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Controllers
{
    internal struct ControllerResponse
    {
        public readonly View? View;
        public readonly Action<HttpListenerResponse> Action;

        public ControllerResponse(View? response = null, 
            Action<HttpListenerResponse>? action = null)
        {
            this.View = response;
            this.Action = (action is null) ? (HttpListenerResponse) => { } : action;
        }
    }
}
