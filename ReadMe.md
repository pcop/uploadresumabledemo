## 搭配 Resumable.js ， 處理用用戶端大檔案上傳問題，
	
1. 可以支援續傳功能
2. 伺服器端上傳時會預開檔案所需大小，減少可能的檔案破碎問題
3. Server 端主要處理邏輯，封裝在 Resumable.cs 對外只提供必要界面
```C#
    /// <summary>
    ///  Save 進行存檔 或 chunk 檢核
    /// </summary>
    /// <param name="StatusCode">The status code.</param>
    /// <returns></returns>
    public object Save(){...}
    
    /// <summary>
    /// MoveFileTo 移動上傳完成檔案到指定位置
    /// </summary>
    /// <param name="TargetPath">預定移動路徑</param>
    /// <param name="IsOverWrite">是否複寫已存在檔案</param>
    /// <returns></returns>
    public Boolean MoveFileTo(string TargetPath , Boolean IsOverWrite = false ) { ... }
    
    /// <summary>
    /// IsComplete 判斷是否已經完成全部的傳輸作業.
    /// </summary>
    public bool IsComplete;
    
    /// <summary>
    /// 使用者上傳原始檔名
    /// </summary>
    public String FileName {...}
    
    /// <summary>
    /// 上傳檔案大小 (單位 byte )
    /// </summary>
    public Int64 FileSize {...}

```
網路參考資料:
	Resumable.js 官方網站:
		http://www.resumablejs.com/
        
	IIS7 如何關閉特定目錄的執行權限（與 IIS6 比較）:
		https://blog.miniasp.com/post/2010/08/04/IIS7-How-to-Turn-off-Execute-Permission.aspx