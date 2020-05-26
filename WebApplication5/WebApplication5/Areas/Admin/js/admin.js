$(document).ready(function () {
    CKEDITOR.replace("postContent");
    $("#btnSelectImage").click(function () {
        var finder = new CKFinder();
        finder.selectActionFunction = function (fileUrl) {
            $("#link").val(fileUrl);
        }
        finder.popup();
    })
})