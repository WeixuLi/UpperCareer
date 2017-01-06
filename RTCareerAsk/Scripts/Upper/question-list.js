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

function OnUpdateContentFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnUpdateListComplete() {
    var qLst = $('#divQuestionList > .content-info > .box').children('div'),
        length = qLst.length,
        i = 0,
        showList = function () {
            qLst.eq(i).fadeIn(200)
        };
    qLst.hide().each(function (i) {
        $(this).delay(i * 50).fadeIn(200);
    });
}

function GetContentTypeId() {
    var indexBase = parseInt($('.active-nav > .tab-content').attr('href').slice(-1)),
        index = parseInt($('.active-nav > .tab-sorting').attr('href').slice(-1));

    return (index + indexBase);
}

function ResizeCarousel() {
    var windowWidth = $(window).width(),
        sectionWidth = $('.featured-section').width(),
        left = $('.featured-section>.left');

    if (windowWidth > 700) {
        left.width(sectionWidth * 0.7);
    }
    else {
        left.width(sectionWidth);
    }

    left.height(left.width() * 0.5625);
}

$(document).ready(function () {
    ResizeCarousel();
    $('.upper-tab').uppertabs();
});

$(window).resize(function () {
    ResizeCarousel();
});