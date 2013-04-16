using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HttpHeaderModule
{
    public class PostAuthHttpHeaderModule: IHttpModule
    {
        void IHttpModule.Init(HttpApplication app)
        {
            app.PostAuthenticateRequest += new EventHandler(app_PostAuthenticateRequest);
        }

        void app_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            HttpContext context = app.Context;
            HttpHeaderModule module = new HttpHeaderModule();
            module.OutputHeader(context);
        }

        void IHttpModule.Dispose() { }
    }
}
