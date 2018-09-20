<%@ WebHandler Language="C#" Class="get_captcha"%>

using System.Web;
using System.Web.SessionState;

public class get_captcha : IHttpHandler, IRequiresSessionState {

    public void ProcessRequest(HttpContext context) {
        System.Drawing.Bitmap _Img = (new CaptchaUtil()).CaptchaImg;
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
            _Img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            context.Response.ClearContent();
            context.Response.ContentType = "image/Jpeg";
            context.Response.BinaryWrite(ms.ToArray());
        }
        _Img.Dispose();
    }


    public bool IsReusable {
        get {
            return false;
        }
    }
}