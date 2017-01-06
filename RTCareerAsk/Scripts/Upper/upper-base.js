//Fix compatibility issue with CKEditor in a Bootstrap Modal.
$.fn.modal.Constructor.prototype.enforceFocus = function () {
    $(document)
        .off('focusin.bs.modal') // guard against infinite focus loop
        .on('focusin.bs.modal', $.proxy(function (e) {
            if (
                this.$element[0] !== e.target && !this.$element.has(e.target).length
                // CKEditor compatibility fix start.
                && !$(e.target).closest('.cke_dialog, .cke').length
                // CKEditor compatibility fix end.
            ) {
                this.$element.trigger('focus');
            }
        }, this));
};
//Trigger Bootstrap tooltip when page load.
$(function () {
    TriggerTooltip();
    setInterval(KeepSessionAlive, 300000);
})
//Function to trigger Bootstrap tooltip.
function TriggerTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
}

function KeepSessionAlive() {
    $.post("/App_DLL/SessionHeartBeatHandler.ashx", null,
    function (data, textStatus, jqXHR) {
        $('.navbar-brand > strong').animate({ opacity: 0 }, 500, null, function () {
            $(this).animate({ opacity: 1 }, 500);
        });
    }
);
}

function UpdateNewMsgCount() {
    $.ajax({
        url: updateMsgCount,
        type: 'POST',
        success: function (result) {
            $('#divNavBar').html(result);
        },
        error: function (xhr) {
            var json = $.parseJSON(xhr.responseText);
            DisplayErrorInfo(json.errorMessage);
        }
    });
}

function UpdateCmtCount() {
    //For QuestionDetail page.
    if ($('.CmtCount').length) {
        $('.CmtCount').each(function () {
            var cmtCount = $(this).closest('.body').find('.comment-list').find('.box').length;
            if (cmtCount > 0) {
                $(this).find('span').html('(' + cmtCount + ')');
            }
        });
    }
        //For AnswerDetail page.
    else if ($('span.cmt-count')) {
        var count = $('span.cmt-count').parent().siblings('div[id^="divCmtList"]').first().children('div[id^="blkCmt"]:visible').length;
        $('span.cmt-count').text(count);
    }
}

function PreviewPic(file, settings) {
    var config = {
        previewTarget: '#divDropTarget',
        imgId: 'imgPreview',
        postAction: function () { }
    }

    if (settings) {
        $.extend(config, settings)
    }

    if (window.File && window.FileReader && window.FileList && window.Blob) {
        if (!file.type.match('image.*')) {
            return;
        }

        var reader = new FileReader();
        reader.onload = (function (tFile) {
            return function (e) {
                $(config.previewTarget).html('<img id="' + config.imgId + '" style="width: 100%" src="' + e.target.result + '" />');
                config.postAction();
            };
        }(file));
        reader.readAsDataURL(file);
    }
    else {
        alert('此浏览器不支持文件预览');
    }
}

function FillCanvas(file, settings) {
    var config = {
        previewTarget: '#divPreview',
        canvasId: 'cvsPreview',
        postAction: function () { }
    }

    if (settings) {
        $.extend(config, settings)
    }

    if (window.File && window.FileReader && window.FileList && window.Blob) {
        if (!file.type.match('image.*')) {
            return;
        }

        var reader = new FileReader();
        reader.onload = function (e) {
            var img = $('<img>', { src: e.target.result });
            img.load(function () {
                var canvas = document.createElement('canvas');
                var context = canvas.getContext('2d');

                canvas.setAttribute('id', config.canvasId);
                canvas.width = $(config.previewTarget).width();
                canvas.height = $(config.previewTarget).height();
                context.drawImage(this, 0, 0, canvas.width, canvas.height);

                $(config.previewTarget).html(canvas);
                config.postAction();
            })
        };
        reader.readAsDataURL(file);
    }
    else {
        alert('此浏览器不支持文件预览');
    }
}

function DisplaySuccessInfo(infoText) {
    var successTab = $('#divInfoSuccess').find('p');
    $('div[id^="divInfo"]').stop(true, true).hide();
    successTab.html(infoText);
    successTab.closest('.alert-tag').fadeIn('slow').delay(2000).fadeOut('slow');
}

function DisplayErrorInfo(infoText) {
    var errorTab = $('#divInfoError').find('p');
    $('div[id^="divInfo"]').stop(true, true).hide();
    errorTab.html(infoText);
    errorTab.closest('.alert-tag').fadeIn('slow').delay(2000).fadeOut('slow');
}

function RemoveHtml(originString) {
    var container = document.createElement('div');
    var text = document.createTextNode(originString);
    container.appendChild(text);
    return container.innerHTML.replace(/&nbsp;/g, '').trim(); // innerHTML will be a xss safe string
}

function TriggerLoginModal() {
    var data = new Object();

    data.returnUrl = window.location.pathname;

    $.ajax({
        type: "POST",
        url: "/Account/QuickLoginForm",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "html",
        success: function (response) {
            $('#divModal').children().html(response);
            $('#divModal').modal('show');
        },
        error: function (xhr) {
            var json = $.parseJSON(xhr.responseText);
            DisplayErrorInfo(json.errorMessage);
        }
    });
}

//function RequestForDelete(data) {
//    var config = new Object()
//    {
//        id = '',
//        type = '',
//        url = '',
//        title = '',
//        context = ''
//    };

//    var modal = $('#divModalSm');

//    if (data) {
//        $.extend(config, data);
//    }

//    $.ajax(confirmDel, {
//        type: 'POST',
//        contentType: 'application/json',
//        data: JSON.stringify(config),
//        dataType: 'html',
//        success: function (result) {
//            modal.children().html(result.trim())
//        },
//        error: function (xhr) {
//            var json = $.parseJSON(xhr.responseText);
//            DisplayErrorInfo(json.errorMessage);
//        }
//    })
//}

$(document).ready(function () {
    $(document).on('click', 'button.close', function () {
        $(this).closest('.alert-tag').stop(true, true).fadeOut('fast');
    });

    $(document).on('click', '.redirect-login', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        TriggerLoginModal();
    });

    $('#lnkFollowUs').popover({
        html: true,
        trigger: 'click focus',
        //placement: function (context, source) {
        //    var get_position = $(source).position();
        //    if (get_position.left > 515) {
        //        return "left";
        //    }
        //    if (get_position.left < 515) {
        //        return "right";
        //    }
        //    if (get_position.top < 110) {
        //        return "bottom";
        //    }
        //    return "top";
        //},
        content: function () {
            return $('#divWechatQR').html();
        }
    }).on('click', function (e) {
        e.preventDefault();
    }).on("mouseleave", function () {
        var _this = this;
        setTimeout(function () {
            if (!$(".popover:hover").length) {
                $(_this).popover("hide")
            }
        }, 100);
    });

    $('#btnPostQuestion').click(function (e) {
        $.ajax({
            url: loadQuestionForm,
            type: 'POST',
            success: function (result) {
                $('#divModal').children().html(result);
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            }
        });
    });

    $(document.body).on('click', '#btnWriteLetter', function () {
        var targetId = $(this).data('id');

        $.ajax({
            url: '/Message/CreateLetterForm',
            data: { targetId: targetId },
            dataType: 'html',
            type: 'POST',
            success: function (result) {
                $('#divModal').children().html(result);
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            }
        });
    })

    $('#formSearch').submit(function (e) {

        var form = $(e.delegateTarget),
            keyword = form.find('input[type="text"]').val();

        if (keyword.length == 0) {
            e.preventDefault();
            DisplaySuccessInfo('<strong style="font-size: 15px">请输入搜索关键词！</strong>');
        }
    })

    //Enable nav bar dropdown list with hover event.
    //$('ul.nav li.dropdown').hover(function () {
    //    $(this).find('.dropdown-menu').stop(true, true).delay(200).slideDown(200);
    //}, function () {
    //    $(this).find('.dropdown-menu').stop(true, true).delay(200).slideUp(200);
    //});
});
