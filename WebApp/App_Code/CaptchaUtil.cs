using System;
using System.Web;
using System.Drawing;

/// <summary>
/// CaptchaUtil 圖形隨機驗證碼處理工具.
/// </summary>
public class CaptchaUtil {

    #region <<-- VerifyCode 驗證輸入的驗證碼是否正確 -->>
    /// <summary>
    /// 驗證輸入的驗證碼是否正確.
    /// </summary>
    /// <param name="_KeyInCode"></param>
    /// <returns></returns>
    public static bool VerifyCode(string _KeyInCode) {

        if (String.IsNullOrWhiteSpace(_KeyInCode)) return false;

        string _SessCode = string.Format("{0}", HttpContext.Current.Session[ConstantConfig.Session.CaptchaId]).Trim();

        if (String.IsNullOrWhiteSpace(_SessCode)) return false;

        return _KeyInCode.ToLower() == _SessCode.ToLower();

    }
    #endregion

    #region <<-- Clear 移除 Session 中的驗證資訊 -->>
    /// <summary>
    /// Clear 移除 Session 中的驗證資訊
    /// </summary>
    public static void Clear() {
        HttpContext.Current.Session.Remove(ConstantConfig.Session.CaptchaId);
    }
    #endregion

    public CaptchaUtil(float _FontSize = 20, Int32 _ImgHigh = 33) {
        this.ImgHigh = _ImgHigh;
        this.FontSize = _FontSize;
    }

    public Int32 ImgHigh { get; set; }

    public float FontSize { get; set; }

    #region <<-- CaptchaImg 產出對應圖形  -->>
    /// <summary>
    /// CaptchaImg 產出對應圖形 
    /// </summary>
    public Bitmap CaptchaImg {
        get {
            String checkCode = _CheckCode;

            if (checkCode == null || checkCode.Trim() == String.Empty) return null;

            Bitmap image = new Bitmap((int)Math.Ceiling((checkCode.Length * (this.FontSize + 0.5))), this.ImgHigh);
            Graphics g = Graphics.FromImage(image);

            try {
                //生成隨機生成器
                Random random = new Random();

                //清空圖片背景色
                g.Clear(Color.White);

                //圖片的背景噪音生成
                for (int i = 0; i < 25; i++) {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);

                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                Font font = new System.Drawing.Font("Arial", this.FontSize, (FontStyle.Bold | FontStyle.Italic));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                //圖片的前景噪音
                for (int i = 0; i < 100; i++) {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //圖片的邊框
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                return image;

            } finally {
                g.Dispose();
            }
        }
    }
    #endregion

    #region <<-- _CheckCode 產生隨機驗證碼併寫入 Session (私有) -->>
    /// <summary>
    /// _CheckCode 產生隨機驗證碼併寫入 Session
    /// </summary>
    private static string _CheckCode {
        get {
            int number;
            char code;
            string checkCode = String.Empty;

            System.Random random = new Random();

            for (int i = 0; i < 5; i++) {
                number = random.Next();

                if (number % 2 == 0) {
                    code = (char)('0' + (char)(number % 10));
                } else {
                    code = (char)('A' + (char)(number % 26));
                }

                checkCode += code.ToString();
            }
            //儲存在session
            HttpContext.Current.Session[ConstantConfig.Session.CaptchaId] = checkCode;
            return checkCode;
        }
    }
    #endregion

}