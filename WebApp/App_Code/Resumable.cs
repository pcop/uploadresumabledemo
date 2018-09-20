using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// Resumable 的摘要描述
/// </summary>
public class ResumableJs {

    /// <summary>
    /// 上傳紀錄的檢核位元
    /// </summary>
    private const byte CheckByte = 32;

    #region <<-- 列舉資訊 -->>
    /// <summary>
    /// 處理方法列舉
    /// </summary>
    private enum MethodList {
        NONE ,
        GET ,
        POST 
    };
    #endregion

    #region <<-- 建構子 -->>
    /// <summary>
    /// Initializes a new instance of the <see cref="ResumableJs"/> class.
    /// </summary>
    /// <param name="context">HttpContext 請求資訊</param>
    /// <param name="_BasePath">預定存檔的基礎路徑</param>
    public ResumableJs(HttpContext _context, String _BasePath) {

        var _Request = _context.Request;

        this.context = _context;

        if (_Request.HttpMethod.ToUpper() == "GET") this.Method = MethodList.GET;

        if (_Request.HttpMethod.ToUpper() == "POST") this.Method = MethodList.POST;

        this.resumableChunkNumber = GetNum(_Request["resumableChunkNumber"]);
        this.resumableChunkSize = GetNum(_Request["resumableChunkSize"]);
        this.resumableCurrentChunkSize = GetNum(_Request["resumableCurrentChunkSize"]);
        this.resumableTotalSize = GetNum(_Request["resumableTotalSize"]);
        this.resumableTotalChunks = GetNum(_Request["resumableTotalChunks"]);

        this.resumableType = (_Request["resumableType"] ?? String.Empty).Trim();
        this.resumableIdentifier = (_Request["resumableIdentifier"] ?? String.Empty).Trim();
        this.resumableFilename = (_Request["resumableFilename"] ?? String.Empty).Trim();
        this.resumableRelativePath = (_Request["resumableRelativePath"] ?? String.Empty).Trim();
        this.BasePath = _Request.MapPath(_BasePath);

    }
    #endregion

    #region <<-- 公開屬性-->>
    /// <summary>
    /// IsComplete 判斷是否已經完成全部的傳輸作業.
    /// </summary>
    public bool IsComplete {
        get {
            if (!File.Exists(this.TempFile) || !File.Exists(this.TempLog)) return false;
            Boolean _IsComplete = true;
            using (FileStream fs = new FileStream(this.TempLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                for (int i = 0; i < this.resumableTotalChunks; i++) {
                    if (fs.ReadByte() != CheckByte) {
                        _IsComplete = false;
                        break;
                    }
                }
            }
            return _IsComplete;
        }
    }

    /// <summary>
    /// 使用者上傳原始檔名
    /// </summary>
    public String FileName { get { return this.resumableFilename; } }

    /// <summary>
    /// 上傳檔案大小 (單位 byte )
    /// </summary>
    public Int64 FileSize { get { return this.resumableTotalSize; } }
    #endregion

    #region <-- MoveFileTo 移動上傳完成檔案到指定位置 ->
    /// <summary>
    /// MoveFileTo 移動上傳完成檔案到指定位置
    /// </summary>
    /// <param name="TargetPath">預定移動路徑</param>
    /// <param name="IsOverWrite">是否複寫已存在檔案</param>
    /// <returns></returns>
    public Boolean MoveFileTo(string TargetPath , Boolean IsOverWrite = false ) {
        if (this.IsComplete == false) return false;
        if (IsOverWrite && File.Exists(TargetPath) ) return false;
        if (!Directory.Exists(Path.GetDirectoryName(TargetPath))) Directory.CreateDirectory(Path.GetDirectoryName(TargetPath));
        if (File.Exists(TargetPath)) File.Delete(TargetPath);
        FileInfo _File = new FileInfo(this.TempFile);
        _File.MoveTo(TargetPath);
        File.Delete(this.TempLog);
        return true;
    }
    #endregion

    #region <-- Save 進行存檔 或 chunk 檢核 -->
    /// <summary>
    ///  Save 進行存檔 或 chunk 檢核
    /// </summary>
    /// <param name="StatusCode">The status code.</param>
    /// <returns></returns>
    public object Save() {
        this.context.Response.StatusCode = 200;
        if (this.context == null) return null;
        if (this.Method == MethodList.GET) return testChunks();
        if (this.Method == MethodList.POST) return saveFile();
        return null;
    }
    #endregion

    #region <-- testChunks 檢核 chunk -->
    /// <summary>
    /// testChunks 檢核 chunk
    /// </summary>
    /// <param name="StatusCode">The status code.</param>
    /// <returns></returns>
    private object testChunks() {
        if (!File.Exists(this.TempFile)|| !File.Exists(this.TempLog)) {
            this.context.Response.StatusCode = 404;
            return new { };
        }
        int flg = 0;
        using (FileStream fs = new FileStream(this.TempLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
            fs.Seek((this.resumableChunkNumber - 1), SeekOrigin.Begin);
            flg = fs.ReadByte();
        }
        this.context.Response.StatusCode = ( flg == CheckByte) ? 200 : 404;
        return new {
            IsComplete = IsComplete,
            resumableChunkNumber = this.resumableChunkNumber
        };
    }
    #endregion

    #region <-- saveFile 進行實際檔案存檔動作 -->
    /// <summary>
    /// saveFile 進行實際檔案存檔動作
    /// </summary>
    /// <param name="StatusCode">The status code.</param>
    /// <returns></returns>
    private object saveFile() {
        this.context.Response.StatusCode = 200;
        lock (FileLock) {
            if (!File.Exists(this.TempFile)) {
                if ( !Directory.Exists(Path.GetDirectoryName(this.TempFile)) ) Directory.CreateDirectory(Path.GetDirectoryName(this.TempFile));
                using (FileStream fs = new FileStream(this.TempFile, FileMode.CreateNew)) {
                    fs.Seek(this.resumableTotalSize, SeekOrigin.Begin);
                    fs.WriteByte(0);
                }
                using (FileStream fs = new FileStream(this.TempLog, FileMode.CreateNew)) {
                    fs.Seek(this.resumableTotalChunks, SeekOrigin.Begin);
                    fs.WriteByte(0);
                }
            }
        }
        var _Request = this.context.Request;
        using (FileStream fs = new FileStream(this.TempFile, FileMode.Open , FileAccess.Write , FileShare.ReadWrite )) {
            fs.Seek((this.resumableChunkNumber - 1) * this.resumableChunkSize, SeekOrigin.Begin);
            byte[] data = _Request.BinaryRead(_Request.ContentLength);
            fs.Write(data, 0, data.Length);
        }
        using (FileStream fs = new FileStream(this.TempLog, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)) {
            fs.Seek((this.resumableChunkNumber - 1), SeekOrigin.Begin);
            fs.WriteByte(CheckByte);
        }
        return new {
            IsComplete = IsComplete,
            resumableChunkNumber = this.resumableChunkNumber
        };
    }
    private static object FileLock = new object();
    #endregion

    #region <<-- 基礎屬性 -->>
    /// <summary>
    /// 用戶端請求資料.
    /// </summary>
    private HttpContext context { get; set; }

    /// <summary>
    /// 目前處理的碎片編號
    /// </summary>
    private Int64 resumableChunkNumber { get; set; }

    /// <summary>
    /// 每一個 Chunk 的大小.
    /// </summary>
    private Int64 resumableChunkSize { get; set; }

    /// <summary>
    /// 目前處理 Chunk 的大小
    /// </summary>
    private Int64 resumableCurrentChunkSize { get; set; }

    /// <summary>
    /// 整個檔案的大小
    /// </summary>
    private Int64 resumableTotalSize { get; set; }
    private string resumableType { get; set; }

    private string resumableIdentifier { get; set; }
    private string resumableFilename { get; set; }
    private string resumableRelativePath { get; set; }

    /// <summary>
    /// 檔案被切成幾個碎片.
    /// </summary>
    private Int64 resumableTotalChunks { get; set; }

    /// <summary>
    /// 基礎 Temp 存檔路徑
    /// </summary>
    private string BasePath { get; set; }

    /// <summary>
    /// HttpMethod 資訊
    /// </summary>
    private MethodList Method { get;set; }

    #endregion

    #region <-- 相關暫存檔名取得 -->

    private String TempFile {
        get {
            return this.BasePath + GetMd5Hash( resumableFilename + resumableIdentifier + context.Request.UserHostAddress) + ".tmp";
        }
    }
    private String TempLog {
        get {
            return this.BasePath + GetMd5Hash( resumableFilename + resumableIdentifier + context.Request.UserHostAddress) + ".log";
        }
    }
    #endregion

    #region <<-- 靜態輔助函數 -->>

    #region <-- GetNum 取得 Request 數值. -->
    /// <summary>
    /// 取得 Request 數值.
    /// </summary>
    /// <param name="_s">The _s.</param>
    /// <param name="_def">The _def.</param>
    /// <returns></returns>
    private static Int64 GetNum(String _s, Int64 _def = 0) {
        if (String.IsNullOrWhiteSpace(_s)) return _def;
        Int64.TryParse(_s, out _def);
        return _def;
    }
    #endregion

    #region <-- GetMd5Hash 計算 MD5 雜湊 -->
    /// <summary>
    /// GetMd5Hash 計算 MD5 雜湊
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    private static string GetMd5Hash(string input) {
        byte[] data;
        using (MD5 md5Hash = MD5.Create()) { 
            // Convert the input string to a byte array and compute the hash.
            data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++) {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
    #endregion

    #endregion
}