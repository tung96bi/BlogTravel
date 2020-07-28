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

//$("#search").click(function () {
//    var searchVal = $("#searchVal").val();
//    $.ajax({
//        type: "post",
//        data: { postSearch: searchVal },
//        url: "/Blog/Search"
//    });
//});

$("#resend").click(function () {
    var id = $("#activeID").val();
    $.ajax({
        type: "post",
        data: { id: id },
        url: "/Blog/ResendEmail",
        success: function (result) {
            console.log(result)
        }
    });
});

$("#emailSub").keypress(function (e) {
    var email = $("#emailSub").val();
    if (e.which == 13) {
        $.ajax({
            type: "post",
            data: { emailSub: email },
            url: "/Blog/Subcribe",
            success: function (result) {
                if (result) {
                    $('#exampleModalCenter').modal('show');
                }
                else {
                    $('#exampleModalCenter1').modal('show');
                }
            }
        });
    }
});

$("#subscribe").click(function () {
    var email = $("#ememailSub1").val();
    $.ajax({
        type: "post",
        data: { emailSub: email },
        url: "/Blog/Subcribe",
        success: function (result) {
            if (result) {
                $('#exampleModalCenter').modal('show');
            }
            else {
                $('#exampleModalCenter1').modal('show');
            }
        }
    });
});

