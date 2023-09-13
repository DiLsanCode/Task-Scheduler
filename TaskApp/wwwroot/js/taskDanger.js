$(function () {
    $(document).ready(function () {
        var m = JSON.parse($('script[data-m][data-m!=null]').attr('data-m'));
        /*$("#taskListDate" + m).toggleClass("bg-primary");
        $("#taskListDate" + m).toggleClass("bg-danger");*/
        for (i in m) {
            //console.log(m[i]);
            $("#taskListDate" + m[i]).toggleClass("bg-primary")
            $("#taskListDate" + m[i]).toggleClass("bg-danger")
        }
    });
});