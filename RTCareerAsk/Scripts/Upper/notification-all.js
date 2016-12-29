﻿function SetScrollPaging() {
    $('.notification-list').upperscrollpaging('/Message/LoadAllNotificationsByType', {
        contentType: $('.tab-li.active-nav > a').attr('href').slice(-1),
        itemSelector: 'li.item'
    })
}

$(document).ready(function () {
    $('.upper-tab').uppertabs();

    SetScrollPaging();
});