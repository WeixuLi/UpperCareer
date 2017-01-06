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

function OnDeleteBegin() {
    $('#divMsgBody').empty();
    $('#divLoader').show();
}

function OnDeleteSuccess(e) {
    DisplaySuccessInfo('您已成功删除消息！')

    var msgLink = $('#lnkMsg' + e).closest('li');
    msgLink.fadeOut(500, function () {
        $(this).remove();
    });
    $('#divMsgBody').html('<div>请您从列表中选择要阅读的消息</div>');
}

function OnDeleteComplete() {
    $('#divLoader').hide();
}

function OnDeleteFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

$(document).ready(function () {
    $(document).on('delSuccess', function (e, data) {
        OnDeleteSuccess(data.id);
    })
});