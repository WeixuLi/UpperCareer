function OnAtclCmtBegin(event) {
    var form = $(event.target);

    form.find('input[type="submit"]').attr('disabled', '').val('正在提交...');
}

function OnAtclCmtSuccess(data, cmtCountDisplayId, cmtListClass) {
    var data = $.trim(data),
        cmtCountDisplay = $('#' + cmtCountDisplayId),
        cmtList = $('.' + cmtListClass),
        cmtCount = cmtList.children('li').filter(':visible').length;

    if (cmtCount == 0) {
        cmtCountDisplay.children().html('<span class="marked-number">1</span>条评论');
        cmtCountDisplay.addClass('underlined');
        var atclId = $(data).find('li:first').find('button').data('target')
        $('ul[data-targetarticle=""]').attr('data-targetarticle', atclId)
    }
    else if (Math.floor(cmtCount / 20) > 0 && cmtCount % 20 == 0) {
        cmtList.children('li').filter(':visible').filter(':last').hide()
    }
    else {
        cmtCountDisplay.children().children('span').text(cmtCount + 1)
    }

    cmtList.prepend($(data).find('li'));
}

function OnAtclCmtComplete(formClass, xhr, status) {
    var form = $('.' + formClass),
        textarea = form.find('textarea'),
        submitBtn = form.find('input[type="submit"]');

    if (status == 'success') {
        textarea.val('');
    }

    form.find('input[type="submit"]').val('提交').removeAttr('disabled');
}

function OnAtclCmtFailure(xhr, status, error) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

$(document).ready(function () {
    $(document).on('click', '#btnReplyCmt', function () {
        var $this = $(this),
            cmtBoxRply = $('#divCmtFormFloat');

        if ($this.closest('li').next().is('#divCmtFormFloat') && !cmtBoxRply.is(':hidden')) {
            cmtBoxRply.slideUp('fast');
        }
        else if ($this.closest('li').next().is('#divCmtFormFloat') && cmtBoxRply.is(':hidden')) {
            cmtBoxRply.slideDown('fast');
        }
        else {
            cmtBoxRply.hide();
            cmtBoxRply.children('form').find('input[name="ArticleID"]').val($this.data('target'))
            cmtBoxRply.children('form').find('input[name="NotifyUserID"]').val($this.data('authorid'))
            cmtBoxRply.find('textarea').val('回复 ' + $this.data('name') + ': ')
            $this.closest('li').after(cmtBoxRply);
            cmtBoxRply.slideDown('fast');
        }
    });

    $(document).on('submit', '#divCmtFormFloat > form', function () {
        $(this).parent().slideUp();
    });

    $(document).on('click', '.cmt-delete', function () {
        var $this = $(this),
            url = '/Article/DeleteComment',
            acmtId = $this.data('target'),
            atclId = $this.data('article'),
            cmtList = $('.article-comment-list').children('li'),
            cmtCount = cmtList.filter(':visible').length,
            onPageBreak = Math.floor(cmtCount / 20) > 0 && cmtCount % 20 == 0,
            replaceIndex = onPageBreak && cmtList.filter(':last').is(':visible') ? cmtCount : 0,
            data = new Object()

        //alert('Target ID: ' + targetId + '\nReplace with Index: ' + replaceIndex)

        data.acmtId = acmtId;
        data.atclId = atclId;
        data.replaceIndex = replaceIndex;

        $.ajax(url, {
            method: 'post',
            data: JSON.stringify(data),
            contentType: 'application/json',
            dataType: 'html',
            beforeSend: function () {
                $('.cmt-delete').attr('disabled', '')
                $this.text('删除中...')
            },
            success: function (result) {
                result = $.trim(result)

                if (replaceIndex != 0) {
                    cmtList.parent('ul').append($(result).find('li'))
                }
                else if (onPageBreak) {
                    var showCmt = cmtList.filter(':last');

                    while (showCmt.prev('li').is(':hidden'))
                        showCmt = showCmt.prev('li');

                    showCmt.show();
                }

                $this.closest('li').fadeOut('fast', function () {
                    $(this).remove();
                    $('span.marked-number').text($('.article-comment-list').find('li:visible').length);
                })
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            },
            complete: function () {
                $('.cmt-delete').removeAttr('disabled')
                $this.text('删除')
            }
        })
    });
})