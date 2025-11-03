document.addEventListener('DOMContentLoaded', function () {
    window.addEventListener('scroll', function () {
        var header= document.getElementById('header-top');
        var scrollTop = window.scrolly;
        var maxScroll = 250;
        var opacity=Math.min(scrollTop / max5croll, 1);
        header.style.backgroundColor = `rgba(255, 165, 8, $(opacity))`;
    });
});
