function AfterAnswerFeedsLoad() {
    Resize();
    $('div[id^="divAnswer"]').uppershorten();
}

function UpdateAnswerCmtCount(ansId, isIncrement) {
    var badges = $('span[id^="spnCmtCnt' + ansId + '"]')

    badges.each(function () {
        var originCount = parseInt($(this).text()),
            change = isIncrement == true ? 1 : -1;

        $(this).text(originCount + change);
    })
}

function DisplayCommentError(infoText) {
    var errorTab = $('#divInfoFeedComment').find('p');
    $('div[id^="divInfo"]').stop(true, true).hide();
    errorTab.html(infoText);
    errorTab.closest('.alert-tag').slideDown('slow').delay(2000).slideUp('slow');
}

$(document).ready(function () {
    AfterAnswerFeedsLoad();

    $(document).on('click', '.question-collapse', function () {
        var $this = $(this),
            classOpen = 'opened';

        if ($this.hasClass(classOpen)) {
            $this.removeClass(classOpen).text('查看内容');
        }
        else {
            $this.addClass(classOpen).text('收起');
        }
    });

    $('.feed-list').upperscrollpaging('/Home/LoadFeedsByPage', {
        postAction: AfterAnswerFeedsLoad
    })

    $(document).on('click', '.feed-comments', function (e) {
        e.preventDefault();

        var $this = $(this),
            modal = $('#divModal'),
            ansId = $this.data('id')

        $.ajax('/Home/LoadFeedAnswerComments', {
            type: "POST",
            data: JSON.stringify({ ansId: ansId }),
            contentType: 'application/json',
            dataType: "html",
            success: function (result) {
                modal.children().html(result);
                modal.modal('show');
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            }
        });
    });

    $(document).on('keyup', '.feed-comment .textbox', function (e) {
        var $this = $(this),
            text = $this.text(),
            placeholder = $this.parent().siblings('.placeholder');

        if (text) {
            placeholder.hide();
        }
        else {
            placeholder.show();
        }
    });

    $(document).on('click', '.feed-comment .placeholder', function (e) {
        var $this = $(this),
            textbox = $this.siblings('.text').children('.textbox');

        textbox.focus();
    });

    $(document).on('click', '.feed-comment .btn', function (e) {
        e.preventDefault();

        var $this = $(this),
            textbox = $this.parent().siblings('.textbox'),
            data = $this.data();

        if (!textbox.html()) {
            DisplayCommentError("评论内容不能为空！")
            return;
        }

        data.postcontent = textbox.html();

        $.ajax('/Home/SaveFeedComment', {
            type: "POST",
            data: JSON.stringify(data),
            contentType: 'application/json',
            dataType: "html",
            success: function (result) {
                $this.closest('.feed-comment').find('ul').prepend(result);
                textbox.html('');
                UpdateAnswerCmtCount(data.answerid, true);
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayCommentError(json.errorMessage);
            }
        });
    });
});