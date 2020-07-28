$(document).ready(function () {
    $('.table.table-responsive-md.table-striped.table-bordered').DataTable();
})

$("#preview").click(function () {
    var title = $("#title").val();
    var content = CKEDITOR.instances.postContent.getData()
    $("#postContent").val(content);
    $.ajax({
        data: { title: title, content: content },
        type: 'post',
        url: '/Admin/Post/SetDataPreview',
        success: function (data) {
            console.log(data);
            if (data) {
                window.location.href = "/Admin/Post/Preview";
            }
        }
    });
});

$(".remove").click(function () {
    var r = confirm("Are you sure!!");
    if (r == true) {
        $.ajax({
            data: { id: $(this).data("id") },
            type: 'post',
            url: '/Admin/Post/Remove',
            success: function (data) {
                console.log(data);
                if (data) {
                    location.reload();
                }
                
            }
        });
    }
    else {

    }
});

$(".removeCat").click(function () {
    var r = confirm("Are you sure!!");
    if (r == true) {
        $.ajax({
            data: { id: $(this).data("id") },
            type: 'post',
            url: '/Admin/User/RemoveCat',
            success: function (data) {
                console.log(data);
                if (data) {
                    location.reload();
                }
            }
        });
    }
    else {

    }
});

$(".removeUser").click(function () {
    var r = confirm("Are you sure!!");
    if (r == true) {
        $.ajax({
            data: { id: $(this).data("id") },
            type: 'post',
            url: '/Admin/User/RemoveUser',
            success: function (data) {
                console.log(data);
                if (data) {
                    location.reload();
                }
                else {
                    alert("Please remove post with current user first");
                }
            }
        });
    }
    else {

    }
});

$(".btn.btn-primary.aprrove").click(function () {
    var r = confirm("Are you sure!!");
    if (r == true) {
        $.ajax({
            data: { id: $(this).data("id") },
            type: 'post',
            url: '/Admin/Post/AprrovePost',
            success: function (data) {
                console.log(data);
                if (data) {
                    location.reload();
                }
            }
        });
    }
    else {

    }
});

$(".btn.btn-danger.reject").click(function () {
    var r = confirm("Are you sure!!");
    if (r == true) {
        $.ajax({
            data: { id: $(this).data("id") },
            type: 'post',
            url: '/Admin/Post/RejectPost',
            success: function (data) {
                console.log(data);
                if (data) {
                    location.reload();
                }
            }
        });
    }
    else {

    }
});


//CKEDITOR.instances.postContent.on('change', function () {
//    console.log("TEST");
//});

$(document).ready(function () {
    $image_crop = $('#image_demo').croppie({
        enableExif: true,
        viewport: {
            width: 200,
            height: 200,
            type: 'circle' //circle
        },
        boundary: {
            width: 400,
            height: 400
        }
    });

    $('#upload_image').on('change', function () {
        var reader = new FileReader();
        reader.onload = function (event) {
            $image_crop.croppie('bind', {
                url: event.target.result
            }).then(function () {
                console.log('jQuery bind complete');
            });
        }
        reader.readAsDataURL(this.files[0]);
        $('#uploadimageModal').modal('show');
    });

    $('.crop_image').click(function (event) {
        $image_crop.croppie('result', {
            type: 'canvas',
            size: 'viewport'
        }).then(function (response) {
            console.log(response);
            $('#uploadimageModal').modal('hide');
            $('#uploaded_image').attr('src', response);
        })
    });
});  