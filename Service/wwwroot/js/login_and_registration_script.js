document.addEventListener('DOMContentLoaded', function () {

    // === Переменные DOM ===
    const loginBtn = document.getElementById('click-to-show');
    // Добавляем кнопку для мобильной версии, если она есть в Layout
    const mobileLoginBtn = document.getElementById('mobile-login-btn');

    const overlay = document.getElementById('overlay');
    const formContainer = document.getElementById('form-container');

    // Элементы для подтверждения почты (из _ConfirmEmailPartial)
    const confirmContainer = document.querySelector('.container-confirm-email');
    const confirmOverlay = document.getElementById('confirm-overlay');
    const btnSubmitConfirm = document.getElementById('send-confirm-code');

    const showRegister = document.getElementById('show-register');
    const showLogin = document.getElementById('show-login');

    // Временное хранилище данных регистрации (чтобы передать их при подтверждении)
    let tempRegisterData = null;

    // === Функции UI ===

    function openLogin() {
        if (overlay) overlay.classList.add('active');
        if (formContainer) formContainer.classList.add('active');
    }

    function closeForm() {
        if (overlay) overlay.classList.remove('active');
        if (formContainer) {
            formContainer.classList.remove('active');
            formContainer.classList.remove('show-register');
        }
        clearErrors();
    }

    function showConfirm() {
        closeForm(); // Закрываем обычную форму
        if (confirmContainer) confirmContainer.style.display = 'flex';
    }

    function hideConfirm() {
        if (confirmContainer) confirmContainer.style.display = 'none';
    }

    function clearErrors() {
        const errorContainers = document.querySelectorAll('[id^="error-messages-"], #confirm-error');
        errorContainers.forEach(container => container.innerHTML = '');
    }

    function displayErrors(errors, containerId) {
        const container = document.getElementById(containerId);
        if (!container) return;
        container.innerHTML = '';

        const errorList = Array.isArray(errors) ? errors : [errors];
        errorList.forEach(err => {
            const div = document.createElement('div');
            div.classList.add('error-item'); // Можно добавить стили для этого класса
            div.style.color = '#e74c3c';
            div.style.fontSize = '14px';
            div.style.marginTop = '5px';
            div.textContent = err;
            container.appendChild(div);
        });
    }

    // Функция отправки запросов
    async function sendRequest(url, data) {
        try {
            // Пробуем найти токен анти-подделки, если он есть на форме
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            const headers = { 'Content-Type': 'application/json;charset=utf-8' };
            if (tokenInput) {
                headers['RequestVerificationToken'] = tokenInput.value;
            }

            const response = await fetch(url, {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(data)
            });
            return await response.json();
        } catch (error) {
            console.error('Request failed:', error);
            return { success: false, errors: ['Ошибка соединения с сервером'] };
        }
    }

    // === Слушатели событий ===

    // Открытие формы входа
    if (loginBtn) loginBtn.addEventListener('click', (e) => { e.preventDefault(); openLogin(); });
    if (mobileLoginBtn) mobileLoginBtn.addEventListener('click', (e) => { e.preventDefault(); openLogin(); });

    // Закрытие по клику на оверлей
    if (overlay) overlay.addEventListener('click', closeForm);
    if (confirmOverlay) confirmOverlay.addEventListener('click', hideConfirm);

    // Переключение Вход / Регистрация
    if (showRegister) {
        showRegister.addEventListener('click', (e) => {
            e.preventDefault();
            formContainer.classList.add('show-register');
            clearErrors();
        });
    }

    if (showLogin) {
        showLogin.addEventListener('click', (e) => {
            e.preventDefault();
            formContainer.classList.remove('show-register');
            clearErrors();
        });
    }

    // Закрытие по ESC
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            if (formContainer && formContainer.classList.contains('active')) closeForm();
            if (confirmContainer && confirmContainer.style.display === 'flex') hideConfirm();
        }
    });

    // === ЛОГИКА ФОРМ ===

    // 1. Вход
    const loginForm = document.querySelector('.form-box.login form');
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            clearErrors();

            const data = {
                Login: document.getElementById('loginEmail').value,
                Password: document.getElementById('loginPassword').value
            };

            if (!data.Login || !data.Password) {
                displayErrors(['Заполните все поля'], 'error-messages-signin');
                return;
            }

            const result = await sendRequest('/Account/Login', data);

            if (result.success) {
                window.location.reload(); // Перезагрузка для обновления шапки (кнопка Выход)
            } else {
                displayErrors(result.errors, 'error-messages-signin');
            }
        });
    }

    // 2. Регистрация (Шаг 1)
    const registerForm = document.querySelector('.form-box.register form');
    if (registerForm) {
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            clearErrors();

            const data = {
                Username: document.getElementById('registerUsername').value,
                Email: document.getElementById('registerEmail').value,
                Password: document.getElementById('registerPassword').value,
                ConfirmPassword: document.getElementById('registerConfirmPassword').value
            };

            // Простая валидация на клиенте
            const errors = [];
            if (!data.Username) errors.push('Введите имя пользователя');
            if (!data.Email) errors.push('Введите Email');
            if (!data.Password || data.Password.length < 6) errors.push('Пароль должен быть не менее 6 символов');
            if (data.Password !== data.ConfirmPassword) errors.push('Пароли не совпадают');

            if (errors.length > 0) {
                displayErrors(errors, 'error-messages-signup');
                return;
            }

            // Отправка запроса на регистрацию
            const result = await sendRequest('/Account/Register', data);

            if (result.success) {
                // ВАЖНО: Сохраняем данные, вернувшиеся с сервера, для подтверждения
                tempRegisterData = result.data;
                // Показываем окно ввода кода
                showConfirm();
            } else {
                displayErrors(result.errors, 'error-messages-signup');
            }
        });
    }

    // 3. Подтверждение почты (Шаг 2)
    if (btnSubmitConfirm) {
        btnSubmitConfirm.addEventListener('click', async () => {
            const codeInput = document.getElementById('confirmCode');
            const errorContainer = document.getElementById('confirm-error');

            if (errorContainer) errorContainer.innerText = '';

            if (!codeInput || !codeInput.value) {
                if (errorContainer) errorContainer.innerText = "Введите код";
                return;
            }

            if (!tempRegisterData) {
                alert("Ошибка сессии регистрации. Пожалуйста, попробуйте зарегистрироваться заново.");
                hideConfirm();
                openLogin();
                return;
            }

            // Формируем данные для финального подтверждения
            const data = {
                Code: codeInput.value,
                GeneratedCode: tempRegisterData.generatedCode,
                UserName: tempRegisterData.userName,
                UserEmail: tempRegisterData.userEmail,
                UserPassword: tempRegisterData.userPassword
            };

            const result = await sendRequest('/Account/ConfirmEmail', data);

            if (result.success) {
                alert("Регистрация успешно завершена!");
                window.location.reload(); // Входим в систему
            } else {
                if (errorContainer) errorContainer.innerText = result.errors ? result.errors.join(', ') : "Неверный код";
            }
        });
    }
});