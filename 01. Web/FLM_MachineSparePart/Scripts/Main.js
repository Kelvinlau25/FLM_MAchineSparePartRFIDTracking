$(function ($) {
    $('#make-small-nav').click(function (e) {
        if ($('.RightBar').hasClass("HideLeftBar")) {
            $('.RightBar').removeClass("HideLeftBar");
            $('.LeftBar').removeClass("LeftNone");
        } else {
            $('.RightBar').addClass("HideLeftBar");
            $('.LeftBar').addClass("LeftNone");
        }
    });
});