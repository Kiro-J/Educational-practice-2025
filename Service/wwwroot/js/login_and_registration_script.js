const loginBtn = document.getElementById('click-to-show');
const overlay = document.getElementById('overlay');
const formContainer = document.getElementById('form-container');
const showRegister = document.getElementById('show-register');
const showLogin = document.getElementById('show-login');

// Открыть форму
loginBtn.addEventListener('click', () => {
    overlay.classList.add('active');
    formContainer.classList.add('active');
});

// Закрыть форму при клике вне
overlay.addEventListener('click', () => {
    overlay.classList.remove('active');
    formContainer.classList.remove('active');
    formContainer.classList.remove('show-register');
});

// Переключение между входом и регистрацией
showRegister.addEventListener('click', (e) => {
    e.preventDefault();
    formContainer.classList.add('show-register');
});

showLogin.addEventListener('click', (e) => {
    e.preventDefault();
    formContainer.classList.remove('show-register');
});
