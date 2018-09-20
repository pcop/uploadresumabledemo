using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// 字串加解秘處理
/// </summary>
public class StringCrypt {

    #region <<-- 屬性 -->>
    /// <summary>
    /// 系統預設編碼用 key
    /// </summary>
    private const string _default_IV_64 = "acnj82d6";

    #region <-- TimeKey 產生時間因子key值. -->
    /// <summary>
    /// TimeKey 產生時間因子key值
    /// </summary>
    public static String TimeKey {
        get {
            DateTime objDate = DateTime.Now;
            int objYearTemp = objDate.Year - 64;
            String objYear = objYearTemp.ToString();
            String objMonth = objDate.Month.ToString();
            String objDay = objDate.Day.ToString();

            if (objMonth.Length < 2)
                objMonth = "0" + objMonth;

            if (objDay.Length < 2)
                objDay = "0" + objDay;

            return objYear + objMonth + objDay;
        }
    }
    #endregion

    #endregion

    #region <<-- 方法 -->>

    #region <-- Encrypt 字串加密 -->
    /// <summary>
    /// Encrypt 字串加密 ( 預設 key 跟日期相依 )
    /// </summary>
    /// <param name="data">須加密字串</param>
    public static string Encrypt(string data) {
        return Encrypt(data, StringCrypt.TimeKey);
    }

    /// <summary>
    /// Encrypt 字串加密
    /// </summary>
    /// <param name="data">須加密字串</param>
    /// <param name="key">自訂KEY(需為8位數字或英文)</param>
    public static string Encrypt(string data, string key) {
        if (data == null || data == string.Empty)
            return "";

        string KEY_64 = key;
        string IV_64 = StringCrypt._default_IV_64;

        byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
        byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
        try {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();

            StringBuilder oSB = new StringBuilder();

            foreach (byte b in ms.ToArray()) {
                oSB.AppendFormat("{0:X2}", b);
            }
            return oSB.ToString();
        }
        catch {
            return "";
        }

    }
    #endregion

    #region <-- Decrypt 字串解密-->
    /// <summary>
    /// Decrypt 字串解密 ( 預設 key 跟日期相依 )
    /// </summary>
    /// <param name="data">須加密字串</param>
    public static string Decrypt(string data) {
        return Decrypt(data, StringCrypt.TimeKey);
    }
    /// <summary>
    /// Decrypt 字串解密
    /// </summary>
    /// <param name="data">須加解字串</param>
    /// <param name="key">自訂KEY(需為8位數字或英文)</param>
    public static string Decrypt(string data, string key) {
        if (data == null || data == string.Empty)
            return "";

        string KEY_64 = key;
        string IV_64 = StringCrypt._default_IV_64;

        byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
        byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

        byte[] byEnc = new byte[data.Length / 2];
        try {
            for (int i = 0; i < data.Length / 2; i++) {
                int j = Convert.ToInt32(data.Substring(i * 2, 2), 16);
                byEnc[i] = (byte)j;
            }

            //byEnc = Convert.FromBase64String(data);
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        catch {
            return "";
        }
    }
    #endregion

    #endregion

    #region <-- SHA256處理. -->
    /// <summary>
    /// 取得 Share256
    /// </summary>
    /// <param name="Txt">The text.</param>
    /// <returns></returns>
    public static byte[] GetSHA1(String _Txt) {
        byte[] crypto;
        using (SHA1 sha1 = new SHA1CryptoServiceProvider()) {
            crypto = sha1.ComputeHash(Encoding.UTF8.GetBytes(_Txt));//進行SHA256加密
        }
        return crypto;
    }
    #endregion
}