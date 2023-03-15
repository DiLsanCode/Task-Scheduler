$(window).on('resize', function () {
    if ($(window).width() > 500) {
        $('#newRow').removeClass('row');
        $('#newRow').addClass('col');
    } else {
        $('#newRow').removeClass('col');
        $('#newRow').addClass('row');
    }
})