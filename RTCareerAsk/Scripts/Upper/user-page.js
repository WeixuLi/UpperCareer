function OnUpdateContentFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnUpdateContentComplete() { }

$(document).ready(function () {
    $('.upper-tab').uppertabs();

    $(document).on('click', '.user-detail .action > button', function (e) {
        e.preventDefault();

        var $this = $(this),
            isFollow = $this.hasClass('follow'),
            url = isFollow ? '/User/FollowUser' : '/User/UnfollowUser',
            currentClass = isFollow ? 'follow' : 'unfollow',
            currentButton = isFollow ? 'btn-success' : 'btn-default',
            oppositeClass = isFollow ? 'unfollow' : 'follow',
            oppositeButton = isFollow ? 'btn-default' : 'btn-success',
            updatedHtml = isFollow ? '<span class="glyphicon glyphicon-eye-close"></span>&nbsp;取消关注' : '<span class="glyphicon glyphicon-eye-open"></span>&nbsp;关注用户',
            newFollowerCount = isFollow ? parseInt($('#spnFollowerCount').html()) + 1 : parseInt($('#spnFollowerCount').html()) - 1,
            newLetterBtnText = isFollow ? '<span class="glyphicon glyphicon-send"></span>&nbsp;发消息' : '<span class="glyphicon glyphicon-send"></span>&nbsp;关注后发消息',
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
                $('#spnFollowerCount').html(newFollowerCount);
                $('#btnWriteLetter').toggleClass(disableClass).html(newLetterBtnText)
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            },
            complete: function () {
                $this.removeClass(disableClass)
            }
        })
    });

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

//function OnFollowBegin() {
//    $('#btnFollow').addClass('disabled').text('关注中...');
//}

//function OnFollowSuccess() {
//    $('#btnFollow').toggle();
//    $('#btnUnfollow').toggle();
//    $('#btnWriteLetter').removeClass('disabled').html('<span class="glyphicon glyphicon-send"></span>&nbsp;发消息');

//    //DisplaySuccessInfo('恭喜，您已成功关注用户！')

//    var newFollowerCount = parseInt($('#spnFollowerCount').html());
//    $('#spnFollowerCount').html(newFollowerCount + 1);
//}

//function OnFollowComplete() {
//    $('#btnFollow').removeClass('disabled').text('关注TA');
//}

//function OnFollowFailure() {
//    DisplayErrorInfo('申请关注出现问题，请您查看……');
//}

//function OnUnfollowBegin() {
//    $('#btnUnfollow').addClass('disabled').text('取关中...');
//}

//function OnUnfollowSuccess() {
//    $('#btnFollow').toggle();
//    $('#btnUnfollow').toggle();
//    $('#btnWriteLetter').addClass('disabled').html('<span class="glyphicon glyphicon-send"></span>&nbsp;关注后发消息');

//    //DisplaySuccessInfo('恭喜，您已取消关注用户！')

//    var newFollowerCount = parseInt($('#spnFollowerCount').html());
//    $('#spnFollowerCount').html(newFollowerCount - 1);
//}

//function OnUnfollowComplete() {
//    $('#btnUnfollow').removeClass('disabled').text('取消关注');
//}

//function OnUnfollowFailure(xhr) {
//    DisplayErrorInfo('取消关注出现问题，请您查看……');
//}
