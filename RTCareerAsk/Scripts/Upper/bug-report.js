$(document).ready(function () {
    $('#btnBugReport').click(function (e) {
        $.ajax({
            url: '/Test/CreateBugReportForm',
            type: 'POST',
            success: function (result) {
                $('#divModal').children().html(result);
            },
            error: function (e) {
                alert(e.responseText);
            }
        });
    });

    $('td input[type=submit]').click(function (e) {
        var row = $(this).closest('tr');
        var status = $(this).closest('tr').find('select[name=StatusCode]').val();
        AssignColorBasedOnStatus(row, status);
    });
});

function AssignColorBasedOnStatus(element, status) {
    element.removeClass();
    switch (status) {
        case '1':
            element.addClass('danger');
            break;
        case '2':
            element.addClass('success');
            break;
        default:
            break;

    }
}

function OnUpdateSuccess() {
    DisplaySuccessInfo('报告状态已变更！')
}

function OnUpdateFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}