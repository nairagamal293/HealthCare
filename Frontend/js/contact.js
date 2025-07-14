document.addEventListener('DOMContentLoaded', function() {
    // API Configuration
    const API_BASE_URL = 'https://localhost:7230';
    const CONTACT_API_ENDPOINT = `${API_BASE_URL}/api/Contact`;
    
    // DOM Elements
    const contactForm = document.getElementById('contactForm');
    const submitBtn = document.getElementById('submitBtn');
    const submitText = document.getElementById('submitText');
    const spinner = document.getElementById('spinner');
    
    // Form Submission Handler
    contactForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        // Validate form
        if (!contactForm.checkValidity()) {
            e.stopPropagation();
            contactForm.classList.add('was-validated');
            return;
        }
        
        // Get only the fields the API accepts
        const formData = {
            name: document.getElementById('name').value,
            email: document.getElementById('email').value,
            message: document.getElementById('message').value
            // Note: We're excluding 'subject' since the API doesn't accept it
        };
        
        // Show loading state
        submitBtn.disabled = true;
        submitText.textContent = 'Sending...';
        spinner.classList.remove('d-none');
        
        try {
            // Send data to API
            const response = await fetch(CONTACT_API_ENDPOINT, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(formData)
            });
            
            const responseData = await response.json();
            
            if (!response.ok) {
                throw new Error(responseData.message || 'Failed to send message');
            }
            
            // Show success message
            showAlert('Your message has been sent successfully! We will contact you soon.', 'success');
            
            // Reset form
            contactForm.reset();
            contactForm.classList.remove('was-validated');
            
        } catch (error) {
            console.error('Error submitting contact form:', error);
            showAlert(error.message || 'An error occurred while sending your message. Please try again.', 'danger');
        } finally {
            // Reset button state
            submitBtn.disabled = false;
            submitText.textContent = 'Send Message';
            spinner.classList.add('d-none');
        }
    });
    
    // Show alert message
    function showAlert(message, type) {
        // Remove any existing alerts
        const existingAlert = document.querySelector('.alert');
        if (existingAlert) {
            existingAlert.remove();
        }
        
        // Create alert element
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show mt-3`;
        alertDiv.setAttribute('role', 'alert');
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        // Insert alert after the form
        contactForm.parentNode.insertBefore(alertDiv, contactForm.nextSibling);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alertDiv);
            bsAlert.close();
        }, 5000);
    }
    
    // Back to top button functionality
    const backToTopButton = document.querySelector('.back-to-top');
    window.addEventListener('scroll', () => {
        if (window.pageYOffset > 300) {
            backToTopButton.classList.add('active');
        } else {
            backToTopButton.classList.remove('active');
        }
    });
    
    backToTopButton.addEventListener('click', (e) => {
        e.preventDefault();
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });
});