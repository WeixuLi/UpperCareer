function OnUpdateSortingBegin(xhr, settings) {
    //settings.url = settings.url.slice(0, -1) + '3';
    var urlBase = settings.url.slice(0, -1);
    index = parseInt(settings.url.slice(-1)),
    indexBase = parseInt($('.active-nav > .tab-content').attr('href').slice(-1));
    settings.url = urlBase + (index + indexBase);
    return true;
}

function OnUpdateContentBegin(xhr, settings) {
    var urlBase = settings.url.slice(0, -1);
    index = parseInt($('.active-nav > .tab-sorting').attr('href').slice(-1)),
    indexBase = parseInt(settings.url.slice(-1));
    settings.url = urlBase + (index + indexBase);
    return true;
}

function OnUpdateSortingFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnUpdateContentFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnUpdateListComplete() {
    AnimatedListDisplay($('#divQuestionList li.list-group-item'));

    BindScrollPaging();
}

function GetContentTypeId() {
    var indexBase = parseInt($('.active-nav > .tab-content').attr('href').slice(-1)),
        index = parseInt($('.active-nav > .tab-sorting').attr('href').slice(-1));

    return (index + indexBase);
}

function AfterAnswerListLoad() {
    Resize();
    $('.list-group-item > .section-wrap > .text > div').uppershorten();
    $('.brief').css('margin', 0);
}

function BindScrollPaging() {
    var contentTypeId = GetContentTypeId(),
        isOnQuestions = contentTypeId == 1 || contentTypeId == 2;

    if (isOnQuestions) {
        $('.content-info').upperscrollpaging(loadListUpdate, {
            contentType: contentTypeId
        });
    }
    else {
        AfterAnswerListLoad();
        $('.content-info').upperscrollpaging(loadListUpdate, {
            contentType: contentTypeId,
            postAction: AfterAnswerListLoad
        });
    }
}

//function ResizeCarousel() {
//    var windowWidth = $(window).width(),
//        sectionWidth = $('.featured-section').width(),
//        left = $('.featured-section>.left');

//    if (windowWidth > 700) {
//        left.width(sectionWidth * 0.7);
//    }
//    else {
//        left.width(sectionWidth);
//    }

//    left.height(left.width() * 0.5625);
//}

$(document).ready(function () {
    //ResizeCarousel();
    $('.upper-tab').uppertabs();

    AnimatedListDisplay($('#divQuestionList li.list-group-item'));
    BindScrollPaging();
});

$(window).resize(function () {
    //ResizeCarousel();
});