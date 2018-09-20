<%@ WebHandler Language="C#" Class="SaveFile" %>
using System;
using System.Web;
using Newtonsoft.Json;
using System.IO;

public class SaveFile : IHttpHandler {

    public void ProcessRequest(HttpContext context) {
        context.Response.ContentType = "application/json";
        context.Server.ScriptTimeout = 600;
        var R = new ResumableJs(context, "~/UpLoad/Temp/");
        if (Auth.IsLogin == false) {
            context.Response.StatusCode = 500;
            context.Response.Write(JsonConvert.SerializeObject(new { Err = "認證失敗請重新登入" }));
            context.Response.End();
            return;
        }
        var _Result = R.Save();
        int i = 0;
        if (R.IsComplete) {
            String FileName = context.Request.MapPath("~/UpLoad/Complete/" + R.FileName);
            while (File.Exists(FileName)) {
                FileName = context.Request.MapPath("~/UpLoad/Complete/" + (++i).ToString() +  R.FileName);
            }
            R.MoveFileTo(FileName);
        }
        context.Response.Write(JsonConvert.SerializeObject(_Result));
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}