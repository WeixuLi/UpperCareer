function OnPostBegin() {
    var loadingTab = $('#divInfoLoading').find('p');
    loadingTab.text('正在提交问题，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
    $('#divModal').modal('hide');
}

function OnAnsBegin() {
    var loadingTab = $('#divInfoLoading').find('p');
    loadingTab.text('正在提交答案，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
}

function OnCmtBegin() {
    var loadingTab = $('#divInfoLoading').find('p');
    loadingTab.text('正在提交评论，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
    $('form').closest('.collapse').collapse('hide');
}

function OnPostSuccess() {
    DisplaySuccessInfo('恭喜，您已成功发布问题！')

    if ($('.tab-content').length > 0 && $('.tab-sorting:contains("最新")').length > 0) {
        $('.tab-content:contains("问题")').parent().trigger('click');
        $('.tab-sorting:contains("最新")').parent().trigger('click');
    }
}

function OnAnsSuccess() {
    DisplaySuccessInfo('恭喜，您已成功发布答案！')

    $('#btnSubmitAns').addClass('disabled');
    $('#btnWriteAnswer').addClass('not-active');
    $('#divAnsForm').collapse('toggle');
}

function OnCmtSuccess(ansId) {
    DisplaySuccessInfo('恭喜，您已成功发布评论！')

    var element = $('#divCmtList' + ansId + '> .box').first(),
        elemHeight = element.height(),
        winHeight = $(window).height(),
        offset = $('#divCmtList' + ansId).offset().top + elemHeight / 2 - winHeight / 2;

    element.hide().fadeIn('slow');

    $('html,body').animate({ scrollTop: offset }, 600);
}

function OnPostComplete() { }

function OnAnsComplete() {
    $('#divAnsForm').find('button[type="submit"]').removeClass('disabled');
}

function OnCmtComplete() {
    UpdateCmtCount();
}

function OnPostFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnAnsFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnCmtFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnLetterBegin() {
    var loadingTab = $('#divInfoLoading').find('p');
    loadingTab.text('正在发送信件，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
    $('#divModal').modal('hide');
}

function OnLetterSuccess() {
    DisplaySuccessInfo('私信发送成功！')
}

function OnLetterComplete() {

}

function OnLetterFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnQuickPostBegin() {
    var submitBtn = $('#formQuickPost').find('input[type="submit"]');

    submitBtn.addClass('disabled').val('正在提交...');
}

function OnQuickPostSuccess() {
    DisplaySuccessInfo('恭喜，您已成功发布问题！')

    if ($('.tab-content').length > 0 && $('.tab-sorting:contains("最新")').length > 0) {
        $('.tab-content:contains("问题")').parent().trigger('click');
        $('.tab-sorting:contains("最新")').parent().trigger('click');
    }

    $('#formQuickPost').find('input[type="text"]').val('');
    $('#formQuickPost').find('input[type="submit"]').removeClass('disabled').val('提问');
}

function OnQuickPostComplete() { }

function OnQuickPostFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
    $('#formQuickPost').find('input[type="submit"]').removeClass('disabled').val('提问');
}