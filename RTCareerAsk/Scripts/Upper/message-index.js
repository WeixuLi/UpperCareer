function OnMsgBegin() {
    $('#divMsgBody').empty();
}

function OnMsgSuccess() {
    var msgId = $('#hdnMsgID').val();
    var newBadge = $('#lnkMsg' + msgId).closest('li').find('span');
    if (newBadge.length) {
        newBadge.remove();
        UpdateNewMsgCount();
    }
    $('#btnDeleteMsg').upperconfirmdialog(deleteMessage);
}

function OnMsgComplete() {
}

function OnMsgFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnDeleteSuccess(e) {
    DisplaySuccessInfo('您已成功删除消息！')

    var msgItem = $('#liMsg' + e);
    msgItem.slideUp(500, function () {
        $(this).remove();
    });
    //$('#divMsgBody').html('<div>请您从列表中选择要阅读的消息</div>');
}

function OnDeleteComplete() {
    $('#divLoader').hide();
}

$(document).ready(function () {
    $('a[id^="btnDeleteMsg"]').upperconfirmdialog(deleteMessage);

    $(document).on('click', 'h4.panel-title > a', function () {
        var newBadge = $(this).children('span.badge:contains("new")');
        if (newBadge.length > 0) {
            var id = $(this).data('id');

            $.ajax('/Message/MarkMessageAsRead', {
                method: 'Post',
                contentType: 'application/json',
                data: JSON.stringify({ id: id }),
                success: function () {
                    ModifyNewMsgCount(false);
                    newBadge.fadeOut('slow', function () {
                        $(this).remove();
                    })
                },
                error: function (xhr) {
                    var json = $.parseJSON(xhr.responseText);
                    DisplayErrorInfo(json.errorMessage);
                }
            });
        }
    });

    $(document).on('delSuccess', function (e, data) {
        OnDeleteSuccess(data.id);
    })
});