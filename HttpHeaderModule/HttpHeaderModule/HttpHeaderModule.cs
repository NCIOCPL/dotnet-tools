using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.IO;

namespace HttpHeaderModule
{
    public class HttpHeaderModule
    {
        internal void OutputHeader(HttpContext context)
        {
            NameValueCollection collection = context.Request.Headers;
            String[] keys = collection.AllKeys;
            string output = "";

            foreach (string key in keys)
            {
                output = String.Format("{0}{1}: ", output, key);
                String[] values = collection.GetValues(key);
                foreach (string value in values)
                {
                    output = String.Format("{0}{1} <br />", output, value);
                }
            }

            context.Response.Write(output);
            context.Response.End();
        }
    }
}