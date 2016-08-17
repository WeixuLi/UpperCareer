﻿function DragOver(e) {
    e.stopPropagation();
    e.preventDefault();
    e.originalEvent.dataTransfer.dropEffect = 'copy';
}

function DragEnter(e) {
    e.stopPropagation();
    e.preventDefault();
    $(e.target).addClass('file-hover');
}

function DragLeave(e) {
    e.stopPropagation();
    e.preventDefault();
    $(e.target).removeClass('file-hover');
}

function FileDropped(e) {
    e.stopPropagation();
    e.preventDefault();
    $(e.target).removeClass('file-hover');

    if (window.File && window.FileReader && window.FileList && window.Blob) {
        var files = e.originalEvent.dataTransfer.files;

        for (var i = 0; i < files.length; i++) {
            if (!files[i].type.match('image.*')) {
                continue;
            }

            reader = new FileReader();
            reader.onload = (function (tFile) {
                return function (e) {
                    $('#divDropTarget').html('<img id="imgPortraitPreview" style="width: 100% ;" src="' + e.target.result + '" />');
                    ResizePortrait();
                    CropPortrait();
                };
            }(files[i]));
            reader.readAsDataURL(files[i]);
        }
    }
    else {
        alert('此浏览器不支持文件预览');
    }
}

function ResizePortrait() {
    var portrait = $('#imgPortraitPreview');
    var container = portrait.parent();

    var imgRatio = portrait.width() / portrait.height();

    var max = portrait.parent().parent().width();

    if (imgRatio > 1) {
        container.width(max);
        portrait.width(container.width());
        portrait.height(portrait.width() / imgRatio);
        container.height(portrait.height());
    }
    else {
        container.height(max);
        portrait.height(container.height());
        portrait.width(portrait.height() * imgRatio);
        container.width(portrait.width());
    }
}

function CropPortrait() {
    var portrait = $('#imgPortraitPreview');

    $('#divActions').show();
    portrait.cropper({ aspectRatio: 1 / 1, preview: $('div[class^="preview"]') });
}

$(document).ready(function () {
    var dropTarget = $('#divDropTarget');

    dropTarget.on('dragover', DragEnter);
    dropTarget.on('dragleave', DragLeave);
    dropTarget.on('drop', FileDropped);

    $('#btnUpload').click(function () {
        $('#imgPortraitPreview').cropper('getCroppedCanvas', { width: 128, height: 128 }).toBlob(function (blob) {
            var formData = new FormData();

            formData.append('portrait', blob);

            $.ajax(changePortrait, {
                method: "POST",
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    var loadingTab = $('#divInfoLoading').find('strong');
                    loadingTab.text('正在提交问题，请您耐心等待');
                    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
                    $('div[id^="divInfo"]').hide();
                    loadingTab.closest('.alert-tag').show('slow');
                },
                success: function (result) {
                    $('#divNavBar').html(result);
                    DisplaySuccessInfo('更换头像成功！');
                },
                error: function (e) {
                    DisplayErrorInfo('更换头像出现问题，请您查看……');
                }
            });
        },
        'image/jpeg');
    });
});

$(window).resize(function () {
    ResizePortrait();
});