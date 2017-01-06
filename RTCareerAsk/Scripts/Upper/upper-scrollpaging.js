/*
 * Upper scroll paging plugin for Upper v 1.0.0
 * 
 * Developed by Lai Wei, 2016
 * http://www.uppernews.com
 */

(function ($) {
    $.fn.upperscrollpaging = function (url, settings) {
        var config = {
            itemSelector: '.list-group-item',
            contentType: 1,
            targetId: 'none',
            postAction: function () { }
        }

        if (settings) {
            $.extend(config, settings);
        }

        var $this = $(this),
            pageIndex = 1,
            onLoading = false,
            loader = $(document.createElement('div')).addClass('preload-box').html($(document.createElement('div')).addClass('preload-3')).hide(),
            endTag = $(document.createElement('div')).addClass('page-end').text('没有更多新内容了');

        if ($this.length > 1) {
            alert('Cannot bind scroll paging to multiple targets!');
            return false;
        }

        $this.after(loader)

        $(window).off('scroll');

        $(window).scroll(function () {
            if ($this.find(config.itemSelector).length == 0) {
                return;
            }

            if ($(window).scrollTop() + $(window).height() >= $this.height() + $this.offset().top && !onLoading) {
                var data = new Object();
                data.pageIndex = pageIndex;
                data.contentType = config.contentType;
                data.targetId = config.targetId;

                $.ajax(url, {
                    method: 'POST',
                    data: JSON.stringify(data),
                    contentType: 'application/json',
                    dataType: 'html',
                    beforeSend: function () {
                        loader.fadeIn(1000);
                        onLoading = true;
                    },
                    success: function (result) {
                        result = $.trim(result);
                        var resultCount = $(result).find(config.itemSelector).length;

                        if (resultCount > 0) {
                            pageIndex++;
                            $this.find(config.itemSelector).parent().append($(result).find(config.itemSelector));
                            onLoading = false;
                            config.postAction();
                        }
                        else {
                            var count = $this.find(config.itemSelector).length;
                            endTag.text('已显示全部 ' + count + ' 条内容')
                            $this.after(endTag);
                        }
                    },
                    error: function (xhr) {
                        var json = $.parseJSON(xhr.responseText);
                        DisplayErrorInfo(json.errorMessage);
                        onLoading = false;
                    },
                    complete: function () {
                        loader.stop(true, true).hide();
                    }
                })
            }
        });
    }
})(jQuery);