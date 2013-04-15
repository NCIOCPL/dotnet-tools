<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Http Request Dump</title>
    
    <script language="C#" runat="server" id="csharpScript">
        void Page_Init(object sender, EventArgs e)
        {
            headersViewer.ItemDataBound += new RepeaterItemEventHandler(HeadersViewer_ItemDataBound);
            serverVariablesViewer.ItemDataBound += new RepeaterItemEventHandler(ServerVariablesViewer_ItemDataBound);
            headersViewer.DataSource = Request.Headers;
            headersViewer.DataBind();
            serverVariablesViewer.DataSource = Request.ServerVariables;
            serverVariablesViewer.DataBind();
            BindHttpRequestProperties();
        }

        void HeadersViewer_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string key = e.Item.DataItem as string;
                Label keyLabel = e.Item.FindControl("headersKey") as Label;
                Label valueLabel = e.Item.FindControl("headersValue") as Label;
                keyLabel.Text = key;
                valueLabel.Text = Request.Headers.Get(key);
            } 
        }

        void ServerVariablesViewer_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string key = e.Item.DataItem as string;
                Label keyLabel = e.Item.FindControl("serverVarsKey") as Label;
                Label valueLabel = e.Item.FindControl("serverVarsValue") as Label;
                keyLabel.Text = key;
                valueLabel.Text = Request.ServerVariables.Get(key);
            } 
        }

        private void BindHttpRequestProperties()
        {
            System.Data.DataTable httpRequestPropertiesTable = new System.Data.DataTable("Variables");

            httpRequestPropertiesTable.Columns.Add("Property");
            httpRequestPropertiesTable.Columns.Add("Value");

            int accentTypesLength = Request.AcceptTypes.Length;
            string accentTypes = "";
            for (int i = 0; i < accentTypesLength; i++)
            {
                accentTypes = String.Format("{0}{1} ", accentTypes, Request.AcceptTypes[i]);
            }

            httpRequestPropertiesTable.Rows.Add("AccentTypes", accentTypes);

            string[] headers = Request.Headers.AllKeys;
            string headersOut = "";
            foreach (string header in headers)
            {
                headersOut = String.Format("{0}{1} ", headersOut, header);
            }

            httpRequestPropertiesTable.Rows.Add("Headers", headersOut);
            httpRequestPropertiesTable.Rows.Add("AnonymousID", Request.AnonymousID);
            httpRequestPropertiesTable.Rows.Add("ApplicationPath", Request.ApplicationPath);
            httpRequestPropertiesTable.Rows.Add("AppRelativeCurrentExecutionFilePath", Request.AppRelativeCurrentExecutionFilePath);
            httpRequestPropertiesTable.Rows.Add("ContentEncoding", Request.ContentEncoding.EncodingName);
            httpRequestPropertiesTable.Rows.Add("CurrentExecutionFilePath", Request.CurrentExecutionFilePath);
            httpRequestPropertiesTable.Rows.Add("FilePath", Request.FilePath);

            string[] files = Request.Files.AllKeys;
            string filesOutput = "";
            foreach (string file in files)
            {
                filesOutput = String.Format("{0}{1} ", filesOutput, file);
            }

            httpRequestPropertiesTable.Rows.Add("Files", filesOutput);

            httpRequestPropertiesTable.Rows.Add("Path", Request.Path);
            httpRequestPropertiesTable.Rows.Add("PathInfo", Request.PathInfo);
            httpRequestPropertiesTable.Rows.Add("PhysicalApplicationPath", Request.PhysicalApplicationPath);
            httpRequestPropertiesTable.Rows.Add("RawUrl", Request.RawUrl);
            httpRequestPropertiesTable.Rows.Add("RequestType", Request.RequestType);

            string[] keys = Request.ServerVariables.AllKeys;
            string serverVarOutput = "";
            foreach (string key in keys)
            {
                serverVarOutput = String.Format("Key: {1} ", serverVarOutput, key);
                string[] values = Request.ServerVariables.GetValues(key);
                string valuesOutput = "";
                foreach (string value in values)
                {
                    valuesOutput = String.Format("{0}{1} ", valuesOutput, value);
                }
                serverVarOutput = String.Format("{0}Values: {1} ", serverVarOutput, valuesOutput);
            }

            httpRequestPropertiesTable.Rows.Add("ServerVariables", serverVarOutput);

            httpRequestPropertiesTable.Rows.Add("Url", Request.Url);
            httpRequestPropertiesTable.Rows.Add("UserAgent", Request.UserAgent);
            httpRequestPropertiesTable.Rows.Add("UserHostAddress", Request.UserHostAddress);
            httpRequestPropertiesTable.Rows.Add("UserHostName", Request.UserHostName);

            int langLength = Request.UserLanguages.Length;
            string languages = "";
            for (int i = 0; i < langLength; i++)
            {
                languages = String.Format("{0}{1} ", languages, Request.UserLanguages[i]);
            }
            httpRequestPropertiesTable.Rows.Add("UserLanguages", languages);

            httpRequestPropertiesGridView.DataSource = httpRequestPropertiesTable;
            httpRequestPropertiesGridView.DataBind();

        }
    </script> 
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h3 id="Headers">Headers</h3>
        <asp:Repeater ID="headersViewer" Runat="server"> 
            <ItemTemplate>
                <p><asp:Label ID="headersKey" runat="server" /> 
                - <asp:Label ID="headersValue" runat="server" /></p>
            </ItemTemplate> 
        </asp:Repeater>
        <br />
        <h3 id="ServerVars">Server Variables</h3>
        <asp:Repeater ID="serverVariablesViewer" Runat="server"> 
            <ItemTemplate> 
                <p><asp:Label ID="serverVarsKey" runat="server" />
                - <asp:Label ID="serverVarsValue" runat="server" /></p>
            </ItemTemplate> 
        </asp:Repeater>
        <br />
        <h3 id="HttpRequestProperties">Http Request Properties</h3>
        <asp:GridView ID="httpRequestPropertiesGridView" runat="server" />
    </div>
    </form>
</body>
</html>