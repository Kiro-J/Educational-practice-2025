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
        let response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(data)
        });
        return await response.json();
    } catch (error) {
        return { success: false, errors: ['Ошибка сети'] };
    }
}

function displayErrors(errors, containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;

    container.innerHTML = '';
    errors.forEach(err => {
        const div = document.createElement('div');
        div.classList.add('error');
        div.textContent = err;
        container.appendChild(div);
    });
}

function clearErrors() {
    const errorContainers = document.querySelectorAll('[id^="error-messages-"]');
    errorContainers.forEach(container => {
        container.innerHTML = '';
    });
}

function cleaningAndClosingForm() {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => form.reset());
    closeForm();
}

// Обработчик формы входа
document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.querySelector('.form-box.login form');
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const data = {
                Email: document.querySelector('#form-container .login input[type="email"]')?.value ||
                    document.querySelector('#form-container .login input[type="text"]')?.value,
                Password: document.querySelector('#form-container .login input[type="password"]')?.value
            };

            const result = await sendRequest('/Account/Login', data);

            if (!result.success) {
                displayErrors(result.errors, 'error-messages-signin');
            } else {
                if (result.redirectUrl) {
                    window.location.href = result.redirectUrl;
                } else {
                    cleaningAndClosingForm();
                    location.reload();
                }
            }
        });
    }

    // Обработчик формы регистрации
    const registerForm = document.querySelector('.form-box.register form');
    if (registerForm) {
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const data = {
                Username: document.querySelector('#form-container .register input[type="text"]')?.value,
                Email: document.querySelector('#form-container .register input[type="email"]')?.value,
                Password: document.querySelectorAll('#form-container .register input[type="password"]')[0]?.value,
                ConfirmPassword: document.querySelectorAll('#form-container .register input[type="password"]')[1]?.value
            };

            const result = await sendRequest('/Account/Register', data);

            if (!result.success) {
                displayErrors(result.errors, 'error-messages-signup');
            } else {
                cleaningAndClosingForm();
                if (result.message) {
                    alert(result.message);
                }
                // Переключаем на форму входа после успешной регистрации
                formContainer.classList.remove('show-register');
            }
        });
    }
});