$(document).ready(function () {
    var status = $('script[data-m][data-m!=null]').attr('data-m');
    switch (status) {
            case 'ToDo':
    $("#todo").addClass("active");
    $("#inprogress").removeClass("active");
    $("#inreview").removeClass("active");
    $("#done").removeClass("active");
    break;
            case 'InProgress':
    $("#todo").removeClass("active");
    $("#inprogress").addClass("active");
    $("#inreview").removeClass("active");
    $("#done").removeClass("active");
    break;
            case 'InReview':
    $("#todo").removeClass("active");
    $("#inprogress").removeClass("active");
    $("#inreview").addClass("active");
    $("#done").removeClass("active");
    break;
            case 'Done':
    $("#todo").removeClass("active");
    $("#inprogress").removeClass("active");
    $("#inreview").removeClass("active");
    $("#done").addClass("active");
    break;
            default:
    $("#todo").addClass("active");
    $("#inprogress").removeClass("active");
    $("#inreview").removeClass("active");
    $("#done").removeClass("active");
    break;
}
});