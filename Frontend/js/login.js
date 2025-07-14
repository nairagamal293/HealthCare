document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');
    const togglePassword = document.getElementById('togglePassword');
    const loginBtn = document.getElementById('loginBtn');
    const loginText = document.getElementById('loginText');
    const loginSpinner = document.getElementById('loginSpinner');
    const errorAlert = document.getElementById('errorAlert');

    // Toggle password visibility
    togglePassword.addEventListener('click', function() {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);
        this.querySelector('i').classList.toggle('fa-eye-slash');
    });

    // Handle form submission
    loginForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        // Validate inputs
        if (!emailInput.value || !passwordInput.value) {
            showError('Please fill in all fields');
            return;
        }

        // Show loading state
        loginBtn.disabled = true;
        loginText.classList.add('d-none');
        loginSpinner.classList.remove('d-none');

        try {
            const response = await fetch('https://localhost:7230/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: emailInput.value,
                    password: passwordInput.value
                })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Login failed');
            }

            // Save token and redirect to dashboard
            localStorage.setItem('healthcare_token', data.token);
            localStorage.setItem('healthcare_user', JSON.stringify({
                email: data.email,
                roles: data.roles,
                userId: data.userId
            }));

            window.location.href = 'admin-dashboard.html';
        } catch (error) {
            showError(error.message || 'An error occurred during login');
        } finally {
            // Reset loading state
            loginBtn.disabled = false;
            loginText.classList.remove('d-none');
            loginSpinner.classList.add('d-none');
        }
    });

    function showError(message) {
        errorAlert.textContent = message;
        errorAlert.classList.remove('d-none');
        setTimeout(() => {
            errorAlert.classList.add('d-none');
        }, 5000);
    }

    // Check if already logged in
    if (localStorage.getItem('healthcare_token')) {
        window.location.href = 'admin-dashboard.html';
    }
});