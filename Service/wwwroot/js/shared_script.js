document.addEventListener('DOMContentLoaded', function () {
    const headerTop = document.querySelector('.header-top');

    // Функция для скролла шапки
    function onScroll() {
        if (window.scrollY > 50) {
            headerTop.classList.add('scrolled');
        } else {
            headerTop.classList.remove('scrolled');
        }
    }

    // Функции для мобильного меню
    function initMobileMenu() {
        const hamburger = document.getElementById('hamburger');
        const sideMenu = document.getElementById('sideMenu');
        const closeBtn = document.getElementById('closeMenu');
        const mobileLoginBtn = document.getElementById('mobile-login-btn');

        if (!hamburger || !sideMenu) return;

        function toggleMenu() {
            const isActive = sideMenu.classList.contains('active');
            hamburger.classList.toggle('active');
            sideMenu.classList.toggle('active');
            document.body.style.overflow = isActive ? '' : 'hidden';
        }

        function closeMenu() {
            hamburger.classList.remove('active');
            sideMenu.classList.remove('active');
            document.body.style.overflow = '';
        }

        // Обработчики событий
        hamburger.addEventListener('click', toggleMenu);
        closeBtn.addEventListener('click', closeMenu);

        // Закрываем меню при клике на ссылку
        const menuLinks = document.querySelectorAll('.mobile-nav a');
        menuLinks.forEach(link => {
            link.addEventListener('click', closeMenu);
        });

        // Обработчик для мобильной кнопки входа
        if (mobileLoginBtn) {
            mobileLoginBtn.addEventListener('click', function (e) {
                e.preventDefault();
                closeMenu();
                // Открываем окно входа
                const loginBtn = document.getElementById('click-to-show');
                if (loginBtn) {
                    loginBtn.click();
                }
            });
        }

        // Закрываем меню при клике вне его
        document.addEventListener('click', function (event) {
            if (sideMenu.classList.contains('active') &&
                !sideMenu.contains(event.target) &&
                event.target !== hamburger &&
                !hamburger.contains(event.target)) {
                closeMenu();
            }
        });

        // Закрываем меню при нажатии Escape
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape' && sideMenu.classList.contains('active')) {
                closeMenu();
            }
        });
    }

    // Функция для снежинок (если нужна)
    function createSnowflakes() {
        const textCenter = document.querySelector('.text-center');
        if (!textCenter) return;

        const snowflakesContainer = document.createElement('div');
        snowflakesContainer.className = 'snowflakes';
        textCenter.appendChild(snowflakesContainer);

        const snowflakes = ['❅', '❆', '•', '·'];

        for (let i = 0; i < 20; i++) {
            const snowflake = document.createElement('div');
            snowflake.className = 'snowflake';
            snowflake.innerHTML = snowflakes[Math.floor(Math.random() * snowflakes.length)];
            snowflake.style.left = Math.random() * 100 + 'vw';
            snowflake.style.animationDuration = (Math.random() * 5 + 5) + 's';
            snowflake.style.animationDelay = Math.random() * 5 + 's';
            snowflakesContainer.appendChild(snowflake);
        }
    }

    // Инициализация
    if (headerTop) {
        window.addEventListener('scroll', onScroll);
        onScroll();
    }

    initMobileMenu();
    createSnowflakes();





    // Функция для стрелочек карусели
    function initCarouselArrows() {
        const leftArrow = document.querySelector('.arrow.left');
        const rightArrow = document.querySelector('.arrow.right');
        const cardsWrapper = document.querySelector('.cards-wrapper');

        if (!leftArrow || !rightArrow || !cardsWrapper) return;

        function updateArrows() {
            const scrollLeft = cardsWrapper.scrollLeft;
            const maxScroll = cardsWrapper.scrollWidth - cardsWrapper.clientWidth;

            // Показываем/скрываем стрелки в зависимости от позиции скролла
            leftArrow.classList.toggle('disabled', scrollLeft <= 0);
            rightArrow.classList.toggle('disabled', scrollLeft >= maxScroll - 5); // Небольшой запас
        }

        // Обработчики для стрелочек
        leftArrow.addEventListener('click', function () {
            cardsWrapper.scrollBy({
                left: -350, // Шаг прокрутки
                behavior: 'smooth'
            });
        });

        rightArrow.addEventListener('click', function () {
            cardsWrapper.scrollBy({
                left: 350, // Шаг прокрутки
                behavior: 'smooth'
            });
        });

        // Обновляем состояние стрелок при скролле
        cardsWrapper.addEventListener('scroll', updateArrows);

        // Обновляем при загрузке и изменении размера
        window.addEventListener('resize', updateArrows);
        updateArrows();
    }

    // Добавьте вызов функции в DOMContentLoaded
    document.addEventListener('DOMContentLoaded', function () {
        // ... существующий код ...
        initCarouselArrows();
    });
});