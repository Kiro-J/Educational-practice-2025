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




async function sendRequest(url, data) {
    let response = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        body: JSON.stringify(data)
    });
    return await response.json();
}

function displayErrors(errors, containerId) {
    const container = document.getElementById(containerId);
    container.innerHTML = '';
    errors.forEach(err => {
        const div = document.createElement('div');
        div.classList.add('error');
        div.textContent = err;
        container.appendChild(div);
    });
}

function cleaningAndClosingForm(formId) {
    const form = document.getElementById(formId);
    form.reset();
    location.reload();
}

// Обработчик кнопки Login
document.getElementById('loginButton').addEventListener('click', async () => {
    const data = {
        Email: document.getElementById('loginEmail').value,
        Password: document.getElementById('loginPassword').value
    };

    const result = await sendRequest('/Account/Login', data);

    if (!result.success) {
        displayErrors(result.errors, 'error-messages-signin');
    } else {
        cleaningAndClosingForm('form_signin');
    }
});

// Обработчик кнопки Register
document.getElementById('registerButton').addEventListener('click', async () => {
    const data = {
        FirstName: document.getElementById('registerFirstName').value,
        LastName: document.getElementById('registerLastName').value,
        Email: document.getElementById('registerEmail').value,
        Password: document.getElementById('registerPassword').value,
        ConfirmPassword: document.getElementById('registerConfirmPassword').value
    };

    const result = await sendRequest('/Account/Register', data);

    if (!result.success) {
        displayErrors(result.errors, 'error-messages-signup');
    } else {
        cleaningAndClosingForm('form_signup');
    }
});



document.getElementById('btn-register').addEventListener('click', async () => {
    const data = {
        Name: document.getElementById('name').value,
        Email: document.getElementById('reg-email').value,
        Password: document.getElementById('reg-password').value,
        ConfirmPassword: document.getElementById('confirm-password').value
    };

    let response = await fetch('/Account/Register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        body: JSON.stringify(data)
    });

    let result = await response.json();

    if (!result.success) {
        displayErrors('error-messages-signup', result.errors);
    } else {
        alert(result.message);
        location.reload();
    }
});
