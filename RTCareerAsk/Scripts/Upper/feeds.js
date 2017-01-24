function AfterAnswerFeedsLoad() {
    Resize();
    $('div[id^="divAnswer"]').uppershorten();
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
});