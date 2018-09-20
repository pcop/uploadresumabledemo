<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FileList.aspx.cs" Inherits="FileList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentBody" runat="Server">
    <div class="row" style="margin-top:2em;">
        <div class="col-sm-4 col-sm-offset-4">
            <div>
                <input type="button" id="GetListBtn" value="刷新檔案清單" class="btn btn-primary btn-block" />
            </div>
        </div>
    </div>
    <table class="table table-hover" id="FileList">
        <thead>
            <tr>
                <th>檔名</th>
                <th style="width:5em;">大小</th>
                <th style="width:4em;" class="text-center">&nbsp;</th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFoot" runat="Server">
    <script>
        var FileListCtrl = {
            ShowList: function (_FileList) {
                var me = this;
                var _List = $("#FileList tbody").html('');
                _FileList.map(function (item) {
                    var _Tr = $('<tr/>');
                    _Tr.append("<td><a href='UpLoad/Complete/" + item.Name + "' target='_bank'>" + item.Name + "</a></td>");
                    _Tr.append("<td>" + (item.Length / (1024 * 1024)).toFixed(2) + "MB</td>");
                    _Tr.append($('<td/>').append(
                        $("<input type='button' value='刪 除' class='btn btn-xs btn-danger'/>")).data("fs", item.Name).click(function () {
                            var fs = $(this).data("fs");
                            UICtrl.confirm('確定要刪除' + fs, function () {
                                me.RemoveFile(fs);
                            });
                        })
                    );
                    _List.append(_Tr);
                });
            },
            RemoveFile: function ( fs ) {
                var me = this;
                UICtrl.blockUI();
                $.ajax({
                    type: 'POST',
                    url: '<%=Request.FilePath%>/RemoveFile',
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json',
                    data: JSON.stringify({ fs : fs }),
                    success: function (data) {
                        var _Result = data.hasOwnProperty('d') ? data.d : data;
                        me.GetList();
                    },
                    error: function () {
                        UICtrl.ShowErr(["服務呼叫異常.."], "error", 3);
                    },
                    complete: function () {
                        UICtrl.unblockUI();
                    }
                });
            },
            GetList: function () {
                var me = this;
                UICtrl.blockUI();
                $.ajax({
                    type: 'POST',
                    url: '<%=Request.FilePath%>/GetFiles',
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json',
                    data: JSON.stringify({}),
                    success: function (data) {
                        var _Result = data.hasOwnProperty('d') ? data.d : data;
                        me.ShowList(_Result);
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
            $('#GetListBtn').click(function () {
                FileListCtrl.GetList();
            });
            FileListCtrl.GetList();
        })();
    </script>
</asp:Content>

