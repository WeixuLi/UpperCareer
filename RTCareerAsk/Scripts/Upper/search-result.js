function OnExtendResultsBegin(element) {
    $(element).find('.page-end').toggle();
    $(element).find('.preload-box').toggle();
}

function OnExtendResultsSuccess(data, status, xhr, element) {
    var sibling = $(element).siblings('div'),
        results = data.trim(),
        list = $(results).find('li');

    $(element).find('ul').append(list);
    AnimatedListDisplay(list);
    $(element).siblings('h4').children('.marked-number:first').text($(element).find('ul').find('li').length)

    sibling.slideUp('slow', function () {
        $(this).remove();
    });

    HighlightKeywords();
    BindScrollPaging(element);
}

function OnExtendResultsFailure(xhr, status, error, element) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
    $(element).find('.page-end').toggle();
}

function OnExtendResultsComplete(xhr, status, element) {
    $(element).find('.preload-box').toggle();
}

function HighlightKeywords() {
    var keywords = $('input[name="Keyword"]').val();

    $('.content-info li a').each(function () {
        var text = $(this).children('p').length > 0 ? $(this).children('p:first') : $(this);

        if (keywords) {
            //keywords = keywords.replace(/\W/g, '');
            var str = keywords.split(" ");
            $(str).each(function (i) {
                var term = this;

                var textNodes = text.contents().filter(function () { return this.nodeType === 3 });
                textNodes.each(function () {
                    var content = $(this).text();
                    var regex = new RegExp(term, "gi");
                    content = content.replace(regex, '<span class="keyword">' + term + '</span>');
                    $(this).replaceWith(content);
                });
            });
        }
    })

}

function BindScrollPaging(element) {
    element = $(element);
    var data = element.data();

    //alert(data.targetid + '\n' + data.contenttype)

    element.find('.content-info').upperscrollpaging('/Home/ExtendSearchResult', {
        targetId: data.targetid,
        contentType: data.contenttype,
        postAction: function () {
            $(element).siblings('h4').children('.marked-number:first').text($(element).find('ul').find('li').length);
            HighlightKeywords();
        }
    })
}

$(document).ready(function () {
    HighlightKeywords();

    $(document).on('click', '.user-list button', function (e) {
        e.preventDefault();

        var $this = $(this),
            isFollow = $this.hasClass('follow'),
            url = isFollow ? '/User/FollowUser' : '/User/UnfollowUser',
            currentClass = isFollow ? 'follow' : 'unfollow',
            currentButton = isFollow ? 'btn-success' : 'btn-default',
            oppositeClass = isFollow ? 'unfollow' : 'follow',
            oppositeButton = isFollow ? 'btn-default' : 'btn-success',
            updatedHtml = isFollow ? '<span class="glyphicon glyphicon-eye-close"></span>&nbsp;取消关注' : '<span class="glyphicon glyphicon-eye-open"></span>&nbsp;关注用户',
            disableClass = 'disabled',
            targetId = $this.data('target')

        $.ajax(url, {
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ id: targetId }),
            beforeSend: function () {
                $this.addClass(disableClass)
            },
            success: function () {
                $this.removeClass(currentClass).removeClass(currentButton).addClass(oppositeClass).addClass(oppositeButton).html(updatedHtml);
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo((isFollow ? '申请关注出现问题: ' : '取消关注出现问题: ') + json.errorMessage);
            },
            complete: function () {
                $this.removeClass(disableClass)
            }
        })
    });

});