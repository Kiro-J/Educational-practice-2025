const loginBtn = document.getElementById('click-to-show');
const overlay = document.getElementById('overlay');
const formContainer = document.getElementById('form-container');
const showRegister = document.getElementById('show-register');
const showLogin = document.getElementById('show-login');

// Открыть форму
if (loginBtn) {
    loginBtn.addEventListener('click', () => {
        overlay.classList.add('active');
        formContainer.classList.add('active');
    });
}

// Закрыть форму при клике вне
if (overlay) {
    overlay.addEventListener('click', () => {
        closeForm();
    });
}

// Переключение между входом и регистрацией
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

function closeForm() {
    overlay.classList.remove('active');
    formContainer.classList.remove('active');
    formContainer.classList.remove('show-register');
    clearErrors();
}

async function sendRequest(url, data) {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(data)
        });
        return await response.json();
    } catch (error) {
        console.error('Request failed:', error);
        return { success: false, errors: ['Ошибка сети'] };
    }
}

function displayErrors(errors, containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;

    container.innerHTML = '';
    if (Array.isArray(errors)) {
        errors.forEach(err => {
            const div = document.createElement('div');
            div.classList.add('error');
            div.textContent = err;
            container.appendChild(div);
        });
    } else {
        const div = document.createElement('div');
        div.classList.add('error');
        div.textContent = errors;
        container.appendChild(div);
    }
}

function clearErrors() {
    const errorContainers = document.querySelectorAll('[id^="error-messages-"]');
    errorContainers.forEach(container => {
        container.innerHTML = '';
    });
}

function cleaningAndClosingForm() {
    const forms = document.querySelectorAll('#form-container form');
    forms.forEach(form => form.reset());
    closeForm();
}

// Обработчик формы входа
document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.querySelector('.form-box.login form');
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            clearErrors();

            const data = {
                Login: document.getElementById('loginEmail').value,
                Password: document.getElementById('loginPassword').value
            };

            // Валидация
            if (!data.Login || !data.Password) {
                displayErrors(['Заполните все поля'], 'error-messages-signin');
                return;
            }

            const result = await sendRequest('/Account/Login', data);

            if (result.success) {
                if (result.redirectUrl) {
                    window.location.href = result.redirectUrl;
                } else {
                    cleaningAndClosingForm();
                    setTimeout(() => location.reload(), 500);
                }
            } else {
                displayErrors(result.errors, 'error-messages-signin');
            }
        });
    }

    // Обработчик формы регистрации
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

            // Валидация
            const errors = [];
            if (!data.Username) errors.push('Введите имя пользователя');
            if (!data.Email) errors.push('Введите email');
            if (!data.Password) errors.push('Введите пароль');
            if (!data.ConfirmPassword) errors.push('Подтвердите пароль');
            if (data.Password && data.ConfirmPassword && data.Password !== data.ConfirmPassword) {
                errors.push('Пароли не совпадают');
            }
            if (data.Password && data.Password.length < 6) {
                errors.push('Пароль должен быть не менее 6 символов');
            }

            if (errors.length > 0) {
                displayErrors(errors, 'error-messages-signup');
                return;
            }

            const result = await sendRequest('/Account/Register', data);

            if (result.success) {
                cleaningAndClosingForm();
                if (result.message) {
                    alert(result.message);
                }
                formContainer.classList.remove('show-register');
            } else {
                displayErrors(result.errors, 'error-messages-signup');
            }
        });
    }

    // Закрытие по ESC
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && formContainer.classList.contains('active')) {
            closeForm();
        }
    });
});