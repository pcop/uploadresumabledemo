<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableSessionState="False" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        /* Uploader: Drag & Drop */
        .resumable-error {
            display: none;
            font-size: 14px;
            font-style: italic;
        }

        .resumable-drop {
            padding: 10px;
            font-size: 16px;
            text-align: left;
            color: #666;
            font-weight: bold;
            background-color: #eee;
            border: 2px dashed #aaa;
            border-radius: 10px;
            margin-top: 20px;
            z-index: 9999;
            height: 5em;
            width: 25em;
            text-align: center;
        }

        .resumable-dragover {
            padding: 30px;
            color: #555;
            background-color: #ddd;
            border: 1px solid #999;
        }

        a {
            color: #45913A;
        }
        /* Uploader: Progress bar */
        .resumable-progress {
            margin: 30px 0 30px 0;
            width: 100%;
            display: none;
        }

        .progress-container {
            height: 7px;
            background: #9CBD94;
            position: relative;
        }

        .progress-bar {
            position: absolute;
            top: 0;
            left: 0;
            bottom: 0;
            background: #45913A;
            width: 0;
        }

        .progress-pause {
            padding: 0 0 0 7px;
        }

        .progress-resume-link {
            display: none;
        }

        /* Uploader: List of items being uploaded */
        .resumable-list {
            overflow: auto;
            margin: 0;
            padding: 0;
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentBody" Runat="Server">
    <div class="row">
        <div class="col-sm-4 col-sm-offset-4">
            <div class="resumable-drop"
                ondragenter="$(this).addClass('resumable-dragover');"
                ondragend="$(this).removeClass('resumable-dragover');"
                ondrop="$(this).removeClass('resumable-dragover');">
                <a class="resumable-browse" href="#" style="text-decoration: none;">選擇上傳檔案<br />
                    Chrome , Edge 可直接拖曳檔案
                </a>
            </div>
        </div>
        <div class="col-sm-4 text-center">
            <a href="FileList.aspx" target="_blank" class="btn btn-info" style="margin-top:1em;">已上傳檔案清單</a>
        </div>
    </div>
    <div class="resumable-progress">
        <div class="progress-container" style="width: 360px;">
            <div class="progress-bar"></div>
        </div>
        <a href="#" onclick="r.upload(); return(false);" class="progress-resume-link">開始上傳</a>
        <a href="#" onclick="r.pause(); return(false);" class="progress-pause-link">暫停上傳</a>
    </div>
    <ul class="resumable-list">
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFoot" Runat="Server">
    <script src="resumable.js"></script>
    <script>
        var r = new Resumable({
            target: 'SaveFile.ashx',
            chunkSize: 2 * 1024 * 1024,
            simultaneousUploads: 3,
            testChunks: true,
            throttleProgressCallbacks: 1,
            method: "octet"
        });

        (function () {
            // Resumable.js isn't supported, fall back on a different method
            if (!r.support) {
                $('.resumable-error').show();
                return;
            }
            $('.resumable-drop').show();
            r.assignDrop($('.resumable-drop')[0]);
            r.assignBrowse($('.resumable-browse')[0]);
            var objName = "";
            // Handle file add event
            r.on('fileAdded', function (file) {
                file.startchunkindex = 0; // 設置當前文件開始上傳的塊數
                // Show progress pabr
                $('.resumable-progress, .resumable-list').show();
                $('.resumable-list').append('<li class="resumable-file-' + file.uniqueIdentifier + '">Uploading <span class="resumable-file-name"></span> <span class="resumable-file-progress"></span>');
                $('.resumable-file-' + file.uniqueIdentifier + ' .resumable-file-name').html(file.fileName);
                objName = file.fileName;

                if (r.isUploading()) {
                    $('.resumable-progress .progress-resume-link').hide();
                    $('.resumable-progress .progress-pause-link').show();
                } else {
                    $('.resumable-progress .progress-resume-link').show();
                    $('.resumable-progress .progress-pause-link').hide();
                }
                //r.upload();
            });

            r.on('uploadStart', function () {
                $('.resumable-progress .progress-resume-link').hide();
                $('.resumable-progress .progress-pause-link').show();
            });
            r.on('pause', function () {
                // Show resume, hide pause
                $('.resumable-progress .progress-resume-link').show();
                $('.resumable-progress .progress-pause-link').hide();
            });
            r.on('complete', function () {
                // Hide pause/resume when the upload has completed
                $('.resumable-progress .progress-resume-link, .resumable-progress .progress-pause-link').hide();
            });
            r.on('fileSuccess', function (file, message) {
                // Reflect that the file upload has completed
                $('.resumable-file-' + file.uniqueIdentifier + ' .resumable-file-progress').html('(上傳完成)');
            });
            r.on('fileError', function (file, message) {
                // Reflect that the file upload has resulted in error
                $('.resumable-file-' + file.uniqueIdentifier + ' .resumable-file-progress').html('(file could not be uploaded: ' + message + ')');
            });
            r.on('fileProgress', function (file) {
                //alert(file.progress());
                //Handle progress for both the file and the overall upload
                //console.log(file);
                $('.resumable-file-' + file.uniqueIdentifier + ' .resumable-file-progress').html(Math.floor(file.progress() * 100) + '%');
                $('.progress-bar').css({ width: Math.floor(r.progress() * 100) + '%' });
            });
        })();
    </script>
</asp:Content>

