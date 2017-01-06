function CoverPicSelected(e) {
    var file = e.target.files[0];

    FillCanvas(file, {
        previewTarget: '#divCoverPreview',
        postAction: function () {
            $('#btnUploadCover').removeClass('disabled');
        }
    })
}

function CoverPicDropped(e) {
    e.stopPropagation();
    e.preventDefault();
    $('#divCoverPreview').removeClass('file-hover').removeClass('file-await');
    var file = e.originalEvent.dataTransfer.files[0];

    FillCanvas(file, {
        previewTarget: '#divCoverPreview',
        postAction: function () {
            $('#btnUploadCover').removeClass('disabled');
        }
    })
}

function DragOver(e) {
    e.stopPropagation();
    e.preventDefault();
    $('#divCoverPreview').addClass('file-hover');
}

function DragLeave(e) {
    e.stopPropagation();
    e.preventDefault();
    $('#divCoverPreview').removeClass('file-hover');
}

$(document).ready(function () {
    var dropTarget = $('#divCoverPreview');

    dropTarget.on('dragover', DragOver);
    dropTarget.on('dragleave', DragLeave);
    dropTarget.on('drop', CoverPicDropped);

    $('#lnkUploadCover').click(function (e) {
        e.preventDefault();
        $('#inpCoverFile').trigger('click');
    })

    $('#inpCoverFile').on('change', CoverPicSelected);

    $('#btnUploadCover').click(function (e) {
        e.preventDefault();
        var btn = $(this),
            inputCover = $('#txtCoverLink'),
            btnText = btn.text();

        $('#cvsPreview').get(0).toBlob(function (blob) {
            var formData = new FormData();

            formData.append('cover', blob);

            $.ajax('/Article/UploadCoverPic', {
                method: 'post',
                data: formData,
                dataType: 'json',
                processData: false,
                contentType: false,
                beforeSend: function () {
                    btn.addClass('disabled').text('正在上传...')
                },
                success: function (result) {
                    inputCover.val(result.url);
                },
                error: function (xhr) {
                    var json = $.parseJSON(xhr.responseText);
                    DisplayErrorInfo(json.errorMessage);
                },
                complete: function () {
                    btn.removeClass('disabled').text(btnText);
                }
            })
        })
    })
})
