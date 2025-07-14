document.addEventListener('DOMContentLoaded', function() {
    // API Base URL - Update this to match your API endpoint
    const API_BASE_URL = 'https://localhost:7230/api';
    
    // DOM Elements
    const form = document.getElementById('appointmentForm');
    const departmentSelect = document.getElementById('department');
    const preferredDateInput = document.getElementById('preferredDate');
    const submitBtn = document.getElementById('submitBtn');
    const submitText = document.getElementById('submitText');
    const spinner = document.getElementById('spinner');
    const confirmationModal = new bootstrap.Modal(document.getElementById('confirmationModal'));
    
    // Set minimum date for the date picker (today)
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    preferredDateInput.min = tomorrow.toISOString().split('T')[0];
    
    // Load departments from API
    async function loadDepartments() {
        try {
            const response = await fetch(`${API_BASE_URL}/departments`);
            if (!response.ok) {
                throw new Error('Failed to load departments');
            }
            const departments = await response.json();
            
            // Clear existing options
            departmentSelect.innerHTML = '<option value="" selected disabled>Select Department</option>';
            
            // Add departments to select
            departments.forEach(dept => {
                const option = document.createElement('option');
                option.value = dept.id;
                option.textContent = dept.name;
                departmentSelect.appendChild(option);
            });
        } catch (error) {
            console.error('Error loading departments:', error);
            showAlert('Failed to load departments. Please try again later.', 'danger');
        }
    }
    
    // Form submission handler
    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        // Validate form
        if (!form.checkValidity()) {
            e.stopPropagation();
            form.classList.add('was-validated');
            return;
        }
        
        // Prepare form data
        const formData = {
            fullName: document.getElementById('fullName').value,
            phoneNumber: document.getElementById('phoneNumber').value,
            email: document.getElementById('email').value,
            departmentId: parseInt(departmentSelect.value),
            preferredDate: document.getElementById('preferredDate').value,
            message: document.getElementById('message').value
        };
        
        // Show loading state
        submitBtn.disabled = true;
        submitText.textContent = 'Processing...';
        spinner.classList.remove('d-none');
        
        try {
            // Send data to API
            const response = await fetch(`${API_BASE_URL}/bookings`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            });
            
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to book appointment');
            }
            
            const bookingData = await response.json();
            
            // Show success message
            showConfirmation(bookingData);
            
            // Reset form
            form.reset();
            form.classList.remove('was-validated');
            
        } catch (error) {
            console.error('Booking error:', error);
            showAlert(error.message || 'An error occurred while booking your appointment. Please try again.', 'danger');
        } finally {
            // Reset button state
            submitBtn.disabled = false;
            submitText.textContent = 'Book Appointment';
            spinner.classList.add('d-none');
        }
    });
    
    // Show confirmation modal with booking details
    function showConfirmation(bookingData) {
        // Format the date for display
        const formattedDate = new Date(bookingData.preferredDate).toLocaleDateString('en-US', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
        
        // Update modal with booking details
        document.getElementById('confirmName').textContent = bookingData.fullName;
        document.getElementById('confirmDepartment').textContent = departmentSelect.options[departmentSelect.selectedIndex].text;
        document.getElementById('confirmDate').textContent = formattedDate;
        
        // Show modal
        confirmationModal.show();
    }
    
    // Show alert message
    function showAlert(message, type) {
        // Remove any existing alerts
        const existingAlert = document.querySelector('.alert');
        if (existingAlert) {
            existingAlert.remove();
        }
        
        // Create alert element
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
        alertDiv.setAttribute('role', 'alert');
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        // Insert alert before form
        form.parentNode.insertBefore(alertDiv, form);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alertDiv);
            bsAlert.close();
        }, 5000);
    }
    
    // Initialize page
    loadDepartments();
    
    // Back to top button
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