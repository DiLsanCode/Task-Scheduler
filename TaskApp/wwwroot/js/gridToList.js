$(function () {
    $(window).bind("resize", function () {
        const boxes = document.querySelectorAll('.card');
        if ($(this).width() < 767) {
            boxes.forEach(box => {
                box.classList.remove('col', 'p-0', 'm-1', 'mb-3');
                box.classList.add('p-0', 'm-1', 'mb-3')
            });
        }
        else {
            boxes.forEach(box => {
                box.classList.remove('p-0', 'm-1', 'mb-3');
                box.classList.add('col', 'p-0', 'm-1', 'mb-3')
            });
        }
    }).resize();
});
