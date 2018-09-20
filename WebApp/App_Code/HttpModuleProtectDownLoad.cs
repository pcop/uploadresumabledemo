using System;
using System.Configuration;
using System.Web;

public class HttpModuleProtectDownLoad : IHttpModule {
    public void Dispose() {

    }

    public void Init(HttpApplication application) {
        application.BeginRequest += (new EventHandler(this.Application_BeginRequest));
    }

    private static bool ModuleEnabled() {
        Boolean ProtectDownLoad = false;
        bool.TryParse(ConfigurationManager.AppSettings["ProtectDownLoad"], out ProtectDownLoad);
        return ProtectDownLoad;
    }

    private void Application_BeginRequest(Object source, EventArgs e) {
        HttpApplication application = (HttpApplication)source;
        HttpContext context = application.Context;
        string filePath = context.Request.FilePath;

        if (ModuleEnabled() == false) return;

        if (Auth.IsLogin == false) {
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.StatusCode = 403;
            context.Response.Write("不允許存取");
            context.Response.End();
            return;
        }
        application.Request.InsertEntityBody();
    }
}