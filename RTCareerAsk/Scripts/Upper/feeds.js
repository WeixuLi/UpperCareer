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
});