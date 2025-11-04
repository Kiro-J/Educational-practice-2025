document.addEventListener('DOMContentLoaded', function () {
    const headerTop = document.querySelector('.header-top');
    if (!headerTop) return; // если нет элемента — безопасно выйти

    // При скролле добавляем/убираем класс
    function onScroll() {
        if (window.scrollY > 50) {
            headerTop.classList.add('scrolled');
        } else {
            headerTop.classList.remove('scrolled');
        }
    }

    window.addEventListener('scroll', onScroll);
    onScroll(); // вызвать сразу, чтобы в нужном состоянии при загрузке
});
