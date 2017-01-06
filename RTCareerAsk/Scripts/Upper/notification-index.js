function OnNotificationFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function SetScrollPaging() {
    $('.notification-list').upperscrollpaging('/Message/LoadNotificationsByType', {
        contentType: $('.tab-li.active-nav > a').attr('href').slice(-1),
        itemSelector: 'li.item'
    })
}

function MarkRead(item) {
    item.removeClass('new');
    item.find('.cancel-new').fadeOut('fast', function () {
        $(this).remove();
        UpdateNewMsgCount();
    })
}

$(document).ready(function () {
    $(document).on('click', '.notification-list > .new a', function (e) {
        if ($(this).hasClass('btn')) {
            e.preventDefault();
        }

        var item = $(this).closest('.new'),
            id = item.data('id');

        $.ajax('/Message/MarkNotificationAsRead', {
            type: "POST",
            data: JSON.stringify({ id: id }),
            contentType: 'application/json',
            success: function (response) {
                MarkRead(item);
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            }
        });
    });

    $('.upper-tab').uppertabs();

    SetScrollPaging();
});