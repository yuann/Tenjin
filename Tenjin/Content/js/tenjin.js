$(function () {
    var pageTop1 = $("#page-top");
    pageTop1.click(function () {
        $('body, html').animate({ scrollTop: 0 }, 500);
        return false;
    });
    $(window).scroll(function () {
        if ($(this).scrollTop() >= 200) {
            pageTop1.css("bottom", "10px");
        } else {
            pageTop1.css("bottom", "-60px");
        }
    });
});