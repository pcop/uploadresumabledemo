using System;
using System.Collections.Generic;
using System.Web.Services;

public partial class LogOn : System.Web.UI.Page {
    /// <summary>
    /// 進行驗證登入
    /// </summary>
    /// <param name="_User">The _ user.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public static object DoAuth(LoginDat _User) {

        //隨機緩衝 1500 ~ 25000 ms;
        Random Rnd = new Random();
        System.Threading.Thread.Sleep(Rnd.Next(500, 1000));

        if (Auth.IsCSRF) return null;

        List<String> _Err = new List<string>();

        if (CaptchaUtil.VerifyCode(_User.Captcha) == false) {
            _Err.Add("圖形驗證碼錯誤");
            return new {
                IsSuccess = false,
                Err = _Err
            };
        }

        if (_User.Id != "eri" || _User.Pwd != "12686505") _Err.Add("帳號密碼錯誤");

        //驗證有錯誤強制清除驗證碼重來
        if (_Err.Count > 0) CaptchaUtil.Clear();

        Auth.SetLogin();

        return new {
            IsSuccess = (_Err.Count == 0),
            Err = _Err
        };

    }


}

public class LoginDat {
    public String Id { get; set; }
    public String Pwd { get; set; }
    public String Captcha { get; set; }
}