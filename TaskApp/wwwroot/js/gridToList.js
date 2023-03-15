$(function () {
    $(window).bind("resize", function () {
        if ($(this).width() < 500) {
            $('#list').removeClass('card col p-0 m-1 mb-3').addClass('card p-0 m-1 mb-3')
        }
        else {
            $('#list').removeClass('card p-0 m-1 mb-3').addClass('card col p-0 m-1 mb-3')
        }
    }).resize();
});
