using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HttpHeaderModule
{
    public class PreReqHttpHeaderModule: IHttpModule
    {
        void IHttpModule.Init(HttpApplication app)
        {
            app.PreRequestHandlerExecute += new EventHandler(app_PreRequestHandlerExecute);
        }

        void app_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            HttpContext context = app.Context;
            HttpHeaderModule module = new HttpHeaderModule();
            module.OutputHeader(context);
        }

        void IHttpModule.Dispose() { }
    }
}
