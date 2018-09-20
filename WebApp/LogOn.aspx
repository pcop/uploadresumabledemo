<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="LogOn.aspx.cs" Inherits="LogOn" EnableSessionState="ReadOnly" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentBody" runat="Server">
    <div class="row" style="margin-top:2em;">
        <div class="col-sm-4 col-sm-offset-4">
            <div class="input-group">
                <span class="input-group-addon"><i class="glyphicon glyphicon-user"></i></span>
                <input id="UserId" type="text" class="form-control" name="email" placeholder="帳號" />
            </div>
            <div class="input-group">
                <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span>
                <input id="UserPwd" type="password" class="form-control" name="password" placeholder="密碼" />
            </div>
            <div class="form-group form-inline">
                <img id="CaptchaImg" src="~/Captcha.ashx" runat="server" enableviewstate="false" alt="" title="點擊可重新產生驗證碼" />
                <input class="form-control" placeholder="圖形驗證碼" id="Captcha" type="text" autocomplete="off" />
            </div>
            <div>
                <input type="button" id="LoginBtn" value="登入" class="btn btn-primary btn-block" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFoot" runat="Server">
    <script>
    var LogonCtrl = {
        Id: null, Pwd: null, 
        UpLoadDiv: null, LoginDiv : null ,
        Init: function (_Cfg) {
            this.LoginDiv = _Cfg.LoginDiv;
            this.UpLoadDiv = _Cfg.UpLoadDiv;
            var me = this;
            _Cfg.LoginBtn.click(function () {
                me.DoAuth({
                    Id: _Cfg.Id.val(),
                    Pwd: _Cfg.Pwd.val() ,
                    Captcha: _Cfg.Captcha.val()
                });
            });
        },
        DoAuth: function (User) {
            var me = this;
            var _Err = [];
            if (!User.Id) _Err.push("帳號尚未輸入");
            if (!User.Pwd) _Err.push("密碼尚未輸入");
            if (!User.Captcha) _Err.push("圖形驗證尚未輸入");
            if (_Err.length > 0) {
                UICtrl.ShowErr(_Err, "error", 3);
                return;
            }
            UICtrl.blockUI();
            $.ajax({
                type: 'POST',
                url: '<%=Request.FilePath%>/DoAuth',
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json',
                data: JSON.stringify({ _User: User }),
                success: function (data) {
                    var _Result = data.hasOwnProperty('d') ? data.d : data;
                    if (_Result.IsSuccess !== true) {
                        UICtrl.ShowErr(_Result.Err, "error", 3);
                        $("#CaptchaImg").trigger("click");
                        return;
                    }
                    location.href = '<%=ResolveUrl("~/")%>';
                },
                error: function () {
                    UICtrl.ShowErr(["服務呼叫異常.."], "error", 3);
                },
                complete: function () {
                    UICtrl.unblockUI();
                }
            });
        }
    };
    (function () {
        $("#CaptchaImg").click(function () {
            this.src = this.src.replace(/\?.+$/gi, '') + '?' + Math.random();
        });
        LogonCtrl.Init({
            Id: $('#UserId'),
            Pwd: $('#UserPwd'),
            Captcha: $('#Captcha'),
            LoginBtn : $('#LoginBtn')
        });
    })();
    </script>
</asp:Content>

