// Скрипт прокрутки карточек "Наши услуги"
document.addEventListener('DOMContentLoaded', () => {
    const wrapper = document.getElementById('cardsWrapper');
    const leftArrow = document.getElementById('arrowLeft');
    const rightArrow = document.getElementById('arrowRight');

    if (!wrapper || !leftArrow || !rightArrow) return;

    const scrollAmount = 350; // ширина одной карточки

    rightArrow.addEventListener('click', () => {
        wrapper.scrollBy({ left: scrollAmount, behavior: 'smooth' });
    });

    leftArrow.addEventListener('click', () => {
        wrapper.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
    });
});
