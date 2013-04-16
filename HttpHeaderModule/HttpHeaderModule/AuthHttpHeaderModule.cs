using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HttpHeaderModule
{
    public class AuthHttpHeaderModule: IHttpModule
    {
        void IHttpModule.Init(HttpApplication app)
        {
            app.AuthenticateRequest += new EventHandler(app_AuthenticateRequest);
        }

        void app_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            HttpContext context = app.Context;
            HttpHeaderModule module = new HttpHeaderModule();
            module.OutputHeader(context);
        }

        void IHttpModule.Dispose() { }
    }
}
