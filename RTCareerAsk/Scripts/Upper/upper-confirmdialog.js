/*
 * Upper Confirm Dialog plugin for Upper v 1.0.0
 * 
 * Developed by Lai Wei, 2016
 * http://www.uppernews.com
 */

(function ($) {
    $.fn.upperconfirmdialog = function (targetUrl) {
        var config = new Object()
        {
            id = '',
            type = '',
            url = '',
            title = '',
            context = ''
        };
        var modal = $('#divModalSm');

        var data = $(this).data();
        if (data) {
            $.extend(config, data);
        }

        config.url = targetUrl;

        this.each(function (i) {
            $(this).on('click', function () {
                var data = $(this).data();

                if (data) {
                    $.extend(config, data);
                }

                $.ajax(confirmDel, {
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(config),
                    dataType: 'html',
                    success: function (result) {
                        modal.children().html(result.trim())
                    },
                    error: function (xhr) {
                        var json = $.parseJSON(xhr.responseText);
                        DisplayErrorInfo(json.errorMessage);
                    }
                })
            });
        });

        //$(this).on('click', function () {
        //    var data = $(this).data();
        //    if (data) {
        //        $.extend(config, data);
        //    }

        //    $.ajax(confirmDel, {
        //        type: 'POST',
        //        contentType: 'application/json',
        //        data: JSON.stringify(config),
        //        dataType: 'html',
        //        success: function (result) {
        //            modal.children().html(result.trim())
        //        },
        //        error: function (xhr) {
        //            var json = $.parseJSON(xhr.responseText);
        //            DisplayErrorInfo(json.errorMessage);
        //        }
        //    })
        //});

        modal.on('click', '.btn-ok', function (e) {
            var data = $(this).data(),
                modal = $(e.delegateTarget);

            $.ajax(data.href, {
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json',
                beforeSend: function () {
                    modal.addClass('loading');
                },
                success: function () {
                    $(document).trigger('delSuccess', { id: data.id, type: data.type });
                    modal.modal('hide');
                },
                error: function (xhr) {
                    var json = $.parseJSON(xhr.responseText);
                    DisplayErrorInfo(json.errorMessage);
                },
                complete: function () {
                    modal.removeClass('loading');
                }
            })
        })
    }
})(jQuery);