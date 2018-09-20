using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Services;

public partial class FileList : System.Web.UI.Page {

    protected void Page_Load(object sender, EventArgs e) {
        if (Auth.IsLogin == false) Response.Redirect("~/LogOn.aspx");
    }

    /// <summary>
    /// 取得檔案清單
    /// </summary>
    /// <param name="_User">The _ user.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public static List<Object> GetFiles() {
        if (Auth.IsLogin == false) return new List<Object>();
        DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/UpLoad/Complete"));

        return di.GetFiles("*.*", SearchOption.TopDirectoryOnly).Select(
            x => new {
                Name = x.Name, Length = x.Length
            }
        ).ToList<Object>();
    }

    /// <summary>
    /// 移除檔案
    /// </summary>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public static object RemoveFile( String fs ) {

        if (Auth.IsLogin == false) return false;

        String _File = HttpContext.Current.Server.MapPath("~/UpLoad/Complete/" + fs);

        if (File.Exists(_File)) File.Delete(_File);

        return true;
    }
}

