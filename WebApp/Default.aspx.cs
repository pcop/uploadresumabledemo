using System;

public partial class _Default : System.Web.UI.Page {
    /*
     網路參考資料
     http://www.tqcto.com/article/web/32373.html
     */

    protected void Page_Load(object sender, EventArgs e) {
        if (Auth.IsLogin == false) Response.Redirect("~/LogOn.aspx");
    }
}