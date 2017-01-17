function AfterAnswerFeedsLoad() {
    Resize();
    $('div[id^="divAnswer"]').uppershorten();
}

$(document).ready(function () {
    AfterAnswerFeedsLoad()
});