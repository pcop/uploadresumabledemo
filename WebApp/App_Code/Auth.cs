using System;
using System.Web;

/// <summary>
/// Auth 的摘要描述
/// </summary>
public class Auth {

    public static Boolean IsCSRF {
        get {
            HttpRequest _Request = HttpContext.Current.Request;
            if (_Request.HttpMethod != "POST") return true;
            if (_Request.UrlReferrer == null) return true;
            return _Request.Url.Host != _Request.UrlReferrer.Host;
        }
    }

    public static Boolean IsLogin {
        get {
            var Cookie = HttpContext.Current.Request.Cookies["U"];
            if (Cookie == null) return false;
            String Token = StringCrypt.Decrypt(Cookie.Value, "eri01268");
            if (String.IsNullOrWhiteSpace(Token)) return false;
            return Token == Convert.ToBase64String(StringCrypt.GetSHA1(HttpContext.Current.Request.UserAgent + HttpContext.Current.Request.UserHostAddress));
        }
    }

    public static void SetLogin() {
        var Cookie = new HttpCookie(
            "U",
            StringCrypt.Encrypt(Convert.ToBase64String(StringCrypt.GetSHA1(HttpContext.Current.Request.UserAgent + HttpContext.Current.Request.UserHostAddress)), "eri01268")
        );
        Cookie.HttpOnly = true;
        HttpContext.Current.Response.Cookies.Add(Cookie);
    }
}