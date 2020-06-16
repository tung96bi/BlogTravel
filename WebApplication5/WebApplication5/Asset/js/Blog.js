$("#password1").change(function () {
    var pass = $("#password").val();
    var pass1 = $("#password1").val();
    if (pass != pass1) {
        alert("Mật khẩu không trùng khớp")
    }
})

//$("#signup").click(function () {
//    var username = $("#email").val();
//    var pass1 = $("#password1").val();
//    $.ajax({
//        type: "post",
//        data: { name: username, pass: pass1 },
//        url: "/Blog/BuildEmailTemplate",
//        success: function (result) {
//            console.log(result)
//            alert("pls check your email");
//        }
//    });
//})

$("a").click(function () {
    $.ajax({
        type: "post",
        data: { id: $(this).data("id") },
        url: "/Blog/CountView",
        success: function (result) {
            console.log(result)
        }
    });
});

