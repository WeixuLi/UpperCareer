function ResizeImage(img) {
    var parent = img.parent();

    if (img.is(':hidden')) {
        return false
    }
    //if (!parent.hasClass('img-wrap')) {
    //    alert('Image src: ' + img.attr('src').split('/')[3] + '\nWidth ratio: ' + img.width() / parent.width());
    //}

    if (!img.hasClass('resized') && img.width() > parent.width()) {
        var maxWidth = img.width(),
            ratio = img.width() / img.height();

        img.css('width', '');
        img.css('height', '');
        img.addClass('resized');
        img.attr('width', '100%');
        img.attr('height', 'auto');
        img.attr('data-maxwidth', maxWidth);
        img.attr('data-ratio', ratio);
        img.attr('data-toggle', 'tooltip');
        img.attr('data-placement', 'bottom');
        img.attr('title', '点击查看大图');
        TriggerTooltip();
    }
    else if (img.hasClass('resized') && img.data('maxwidth') < parent.width()) {
        var maxWidth = img.data('maxwidth'),
            maxHeight = img.data('maxwidth') / img.data('ratio');

        img.attr('width', maxWidth);
        img.attr('height', maxHeight);
        img.removeAttr('data-maxwidth');
        img.removeAttr('data-ratio');
        img.removeAttr('data-toggle');
        img.removeAttr('data-placement');
        img.removeAttr('data-original-title');
        img.removeAttr('title');
        img.removeClass('resized');
    }
    //if (img.width() > parent.width() && !parent.is('a')) {
    //    var parentWidth = parent.width(),
    //        imgRatio = img.height() / img.width(),
    //        adjustedImgHight = imgRatio * parentWidth;

    //    img.height(adjustedImgHight);
    //    img.width(parentWidth);

    //    var newLink = $("<a href='" + img.attr('src') + "' target='_blank' data-toggle='tooltip' data-placement='bottom' title='点击查看大图'>" + img.prop('outerHTML') + "</a>");
    //    img.replaceWith(newLink);
    //    TriggerTooltip();
    //}
}

function Resize() {
    $('img').not('img[class^="portrait"]')
        .not('.photo')
        .not('.nav-img')
        .each(function () {
            ResizeImage($(this));
        });

    $(document).on('click', '.resized', function (e) {
        e.preventDefault();
        var $this = $(this),
            src = $this.attr('src'),
            modal = $('#divModalLg');

        $.ajax('/Home/CreateImgDisplayModal', {
            method: 'post',
            data: JSON.stringify({ src: src }),
            contentType: 'application/json',
            dataType: 'html',
            success: function (result) {
                modal.children().html(result);
                modal.modal('show');
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            }
        })
    });
}

$(document).ready(function () {
    Resize();
});

$(window).resize(function () {
    Resize();
});