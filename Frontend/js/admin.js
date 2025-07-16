document.addEventListener('DOMContentLoaded', function() {
    // Check authentication
    const token = localStorage.getItem('healthcare_token');
    const user = JSON.parse(localStorage.getItem('healthcare_user'));
    
    if (!token || !user || !user.roles.includes('Admin')) {
        window.location.href = 'login.html';
        return;
    }

    // Set admin name in sidebar
    document.getElementById('adminName').textContent = user.email.split('@')[0];

    // Initialize DataTables
    const initDataTable = (tableId, options = {}) => {
        return $(`#${tableId}`).DataTable({
            responsive: true,
            language: {
                search: "_INPUT_",
                searchPlaceholder: "Search...",
            },
            ...options
        });
    };

    // Initialize all tables
    const doctorsTable = initDataTable('doctorsTable');
    const departmentsTable = initDataTable('departmentsTable');
    const blogsTable = initDataTable('blogsTable');
    const appointmentsTable = initDataTable('appointmentsTable');
    const contactsTable = initDataTable('contactsTable');

    // Tab switching
    const tabs = ['dashboard', 'doctors', 'departments', 'blogs', 'bookings', 'contacts'];
    tabs.forEach(tab => {
        document.getElementById(`${tab}Tab`).addEventListener('click', function(e) {
            e.preventDefault();
            
            // Update active tab
            document.querySelectorAll('.nav-link').forEach(link => link.classList.remove('active'));
            this.classList.add('active');
            
            // Update page title
            document.getElementById('pageTitle').textContent = this.textContent.trim();
            
            // Show correct content
            document.querySelectorAll('[id$="Content"]').forEach(content => content.classList.add('d-none'));
            document.getElementById(`${tab}Content`).classList.remove('d-none');
            
            // Refresh data if needed
            if (tab === 'dashboard') {
                loadDashboardData();
            }
        });
    });

    // Load dashboard data
    async function loadDashboardData() {
        try {
            const [doctorsRes, departmentsRes, bookingsRes, contactsRes] = await Promise.all([
                fetchData('/api/doctors'),
                fetchData('/api/departments'),
                fetchData('/api/bookings'),
                fetchData('/api/contact')
            ]);

            document.getElementById('doctorsCount').textContent = doctorsRes.length;
            document.getElementById('departmentsCount').textContent = departmentsRes.length;
            document.getElementById('appointmentsCount').textContent = bookingsRes.length;
            document.getElementById('messagesCount').textContent = contactsRes.length;

            // Populate recent appointments
            const recentAppointments = bookingsRes.slice(0, 5);
            const appointmentsTable = document.querySelector('#recentAppointmentsTable tbody');
            appointmentsTable.innerHTML = '';
            
            recentAppointments.forEach(appointment => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${appointment.fullName}</td>
                    <td>${appointment.departmentName}</td>
                    <td>${new Date(appointment.preferredDate).toLocaleDateString()}</td>
                    <td>
                        <span class="badge ${appointment.isConfirmed ? 'bg-success' : 'bg-warning'}">
                            ${appointment.isConfirmed ? 'Confirmed' : 'Pending'}
                        </span>
                    </td>
                `;
                appointmentsTable.appendChild(row);
            });

            // Populate recent messages
            const recentMessages = contactsRes.slice(0, 5);
            const messagesList = document.getElementById('recentMessagesList');
            messagesList.innerHTML = '';
            
            recentMessages.forEach(message => {
                const item = document.createElement('a');
                item.className = 'list-group-item list-group-item-action';
                item.innerHTML = `
                    <div class="d-flex w-100 justify-content-between">
                        <h6 class="mb-1">${message.name}</h6>
                        <small>${new Date(message.createdAt).toLocaleDateString()}</small>
                    </div>
                    <p class="mb-1 text-truncate">${message.message}</p>
                    <small>${message.email}</small>
                `;
                messagesList.appendChild(item);
            });

        } catch (error) {
            console.error('Error loading dashboard data:', error);
            showAlert('error', 'Failed to load dashboard data');
        }
    }

    // Load all doctors
    async function loadDoctors() {
        try {
            const doctors = await fetchData('/api/doctors');
            doctorsTable.clear().draw();
            
            doctors.forEach(doctor => {
                doctorsTable.row.add([
                   `<img src="https://localhost:7230${doctor.imagePath}" alt="${doctor.name}" width="50" class="rounded-circle">`,
                    doctor.name,
                    doctor.specialty,
                    doctor.departmentName,
                    `
                    <div class="btn-group">
                        <button class="btn btn-sm btn-outline-primary edit-doctor" data-id="${doctor.id}">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger delete-doctor" data-id="${doctor.id}">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                    `
                ]).draw(false);
            });

            // Add event listeners to action buttons
            document.querySelectorAll('.edit-doctor').forEach(btn => {
                btn.addEventListener('click', () => editDoctor(btn.dataset.id));
            });
            
            document.querySelectorAll('.delete-doctor').forEach(btn => {
                btn.addEventListener('click', () => confirmDelete('doctor', btn.dataset.id));
            });

        } catch (error) {
            console.error('Error loading doctors:', error);
            showAlert('error', 'Failed to load doctors');
        }
    }

    // Load all departments
    async function loadDepartments() {
        try {
            const departments = await fetchData('/api/departments');
            departmentsTable.clear().draw();
            
            departments.forEach(dept => {
                departmentsTable.row.add([
                    dept.name,
                    dept.description.length > 50 ? dept.description.substring(0, 50) + '...' : dept.description,
                    dept.doctorCount,
                    `
                    <div class="btn-group">
                        <button class="btn btn-sm btn-outline-primary edit-department" data-id="${dept.id}">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger delete-department" data-id="${dept.id}">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                    `
                ]).draw(false);
            });

            // Add event listeners to action buttons
            document.querySelectorAll('.edit-department').forEach(btn => {
                btn.addEventListener('click', () => editDepartment(btn.dataset.id));
            });
            
            document.querySelectorAll('.delete-department').forEach(btn => {
                btn.addEventListener('click', () => confirmDelete('department', btn.dataset.id));
            });

        } catch (error) {
            console.error('Error loading departments:', error);
            showAlert('error', 'Failed to load departments');
        }
    }

    // Load all blogs
    async function loadBlogs() {
        try {
            const blogs = await fetchData('/api/blogs');
            blogsTable.clear().draw();
            
            blogs.forEach(blog => {
                blogsTable.row.add([
                    
                    `<img src="https://localhost:7230${blog.imagePath}" alt="${blog.title}" width="50">`,
                    blog.title.length > 50 ? blog.title.substring(0, 50) + '...' : blog.title,
                    new Date(blog.createdAt).toLocaleDateString(),
                    `
                    <div class="btn-group">
                        <button class="btn btn-sm btn-outline-primary edit-blog" data-id="${blog.id}">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger delete-blog" data-id="${blog.id}">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                    `
                ]).draw(false);
            });

            // Add event listeners to action buttons
            document.querySelectorAll('.edit-blog').forEach(btn => {
                btn.addEventListener('click', () => editBlog(btn.dataset.id));
            });
            
            document.querySelectorAll('.delete-blog').forEach(btn => {
                btn.addEventListener('click', () => confirmDelete('blog', btn.dataset.id));
            });

        } catch (error) {
            console.error('Error loading blogs:', error);
            showAlert('error', 'Failed to load blogs');
        }
    }

    // Load all appointments
    async function loadAppointments() {
        try {
            const appointments = await fetchData('/api/Bookings');
            appointmentsTable.clear().draw();
            
            appointments.forEach(appt => {
                appointmentsTable.row.add([
                    appt.fullName,
                    appt.email,
                    appt.phoneNumber,
                    appt.departmentName,
                    new Date(appt.preferredDate).toLocaleDateString(),
                    `
                    <span class="badge ${appt.isConfirmed ? 'bg-success' : 'bg-warning'}">
                        ${appt.isConfirmed ? 'Confirmed' : 'Pending'}
                    </span>
                    `,
                    `
                    <div class="btn-group">
                        <button class="btn btn-sm ${appt.isConfirmed ? 'btn-outline-warning' : 'btn-outline-success'} update-status" data-id="${appt.id}" data-status="${!appt.isConfirmed}">
                            <i class="fas ${appt.isConfirmed ? 'fa-times' : 'fa-check'}"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger delete-appointment" data-id="${appt.id}">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                    `
                ]).draw(false);
            });

            // Add event listeners to action buttons
            document.querySelectorAll('.update-status').forEach(btn => {
                btn.addEventListener('click', () => updateAppointmentStatus(btn.dataset.id, btn.dataset.status === 'true'));
            });
            
            document.querySelectorAll('.delete-appointment').forEach(btn => {
                btn.addEventListener('click', () => confirmDelete('appointment', btn.dataset.id));
            });

        } catch (error) {
            console.error('Error loading appointments:', error);
            showAlert('error', 'Failed to load appointments');
        }
    }

    // Load all contacts
    async function loadContacts() {
        try {
            const contacts = await fetchData('/api/contact');
            contactsTable.clear().draw();
            
            contacts.forEach(contact => {
                contactsTable.row.add([
                    contact.name,
                    contact.email,
                    contact.message.length > 50 ? contact.message.substring(0, 50) + '...' : contact.message,
                    new Date(contact.createdAt).toLocaleDateString(),
                    `
                    <button class="btn btn-sm btn-outline-danger delete-contact" data-id="${contact.id}">
                        <i class="fas fa-trash"></i>
                    </button>
                    `
                ]).draw(false);
            });

            // Add event listeners to action buttons
            document.querySelectorAll('.delete-contact').forEach(btn => {
                btn.addEventListener('click', () => confirmDelete('contact', btn.dataset.id));
            });

        } catch (error) {
            console.error('Error loading contacts:', error);
            showAlert('error', 'Failed to load contacts');
        }
    }

    // Doctor CRUD operations
    document.getElementById('addDoctorBtn').addEventListener('click', () => {
        document.getElementById('doctorModalTitle').textContent = 'Add New Doctor';
        document.getElementById('doctorId').value = '';
        document.getElementById('doctorForm').reset();
        document.getElementById('doctorImagePreviewContainer').style.display = 'none';
        
        // Load departments for dropdown
        loadDepartmentsForDropdown().then(() => {
            const modal = new bootstrap.Modal(document.getElementById('doctorModal'));
            modal.show();
        });
    });

    async function editDoctor(id) {
        try {
            const doctor = await fetchData(`/api/doctors/${id}`);
            
            document.getElementById('doctorModalTitle').textContent = 'Edit Doctor';
            document.getElementById('doctorId').value = doctor.id;
            document.getElementById('doctorName').value = doctor.name;
            document.getElementById('doctorSpecialty').value = doctor.specialty;
            document.getElementById('doctorBio').value = doctor.bio;
            
            // Load departments for dropdown and set selected
            await loadDepartmentsForDropdown();
            document.getElementById('doctorDepartment').value = doctor.departmentId;
            
            // Show image preview if exists
            if (doctor.imagePath) {
                 document.getElementById('doctorImagePreview').src = `https://localhost:7230${doctor.imagePath}`;
                
                document.getElementById('doctorImagePreviewContainer').style.display = 'block';
            } else {
                document.getElementById('doctorImagePreviewContainer').style.display = 'none';
            }
            
            const modal = new bootstrap.Modal(document.getElementById('doctorModal'));
            modal.show();
            
        } catch (error) {
            console.error('Error loading doctor:', error);
            showAlert('error', 'Failed to load doctor details');
        }
    }

    async function loadDepartmentsForDropdown() {
        try {
            const departments = await fetchData('/api/departments');
            const dropdown = document.getElementById('doctorDepartment');
            dropdown.innerHTML = '<option value="">Select Department</option>';
            
            departments.forEach(dept => {
                const option = document.createElement('option');
                option.value = dept.id;
                option.textContent = dept.name;
                dropdown.appendChild(option);
            });
            
        } catch (error) {
            console.error('Error loading departments:', error);
            showAlert('error', 'Failed to load departments');
        }
    }

    document.getElementById('saveDoctorBtn').addEventListener('click', async () => {
        const form = document.getElementById('doctorForm');
        const id = document.getElementById('doctorId').value;
        const isEdit = !!id;
        
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }
        
        try {
            const formData = new FormData();
            formData.append('Name', document.getElementById('doctorName').value);
            formData.append('Specialty', document.getElementById('doctorSpecialty').value);
            formData.append('Bio', document.getElementById('doctorBio').value);
            formData.append('DepartmentId', document.getElementById('doctorDepartment').value);
            
            const imageFile = document.getElementById('doctorImage').files[0];
            if (imageFile) {
                formData.append('ImageFile', imageFile);
            }
            
            let response;
            if (isEdit) {
                formData.append('Id', id);
                response = await fetchData(`/api/doctors/${id}`, {
                    method: 'PUT',
                    body: formData
                }, false);
            } else {
                response = await fetchData('/api/doctors', {
                    method: 'POST',
                    body: formData
                }, false);
            }
            
            showAlert('success', `Doctor ${isEdit ? 'updated' : 'added'} successfully`);
            loadDoctors();
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('doctorModal'));
            modal.hide();
            
        } catch (error) {
            console.error('Error saving doctor:', error);
            showAlert('error', `Failed to ${isEdit ? 'update' : 'add'} doctor`);
        }
    });

    // Department CRUD operations
    document.getElementById('addDepartmentBtn').addEventListener('click', () => {
        document.getElementById('departmentModalTitle').textContent = 'Add New Department';
        document.getElementById('departmentId').value = '';
        document.getElementById('departmentForm').reset();
        document.getElementById('departmentImagePreviewContainer').style.display = 'none';
        document.getElementById('servicesContainer').innerHTML = '';
        
        // Add initial service field
        addServiceField();
        
        const modal = new bootstrap.Modal(document.getElementById('departmentModal'));
        modal.show();
    });

    async function editDepartment(id) {
        try {
            const department = await fetchData(`/api/departments/${id}`);
            const services = await fetchData(`/api/services/department/${id}`);
            
            document.getElementById('departmentModalTitle').textContent = 'Edit Department';
            document.getElementById('departmentId').value = department.id;
            document.getElementById('departmentName').value = department.name;
            document.getElementById('departmentDescription').value = department.description;
            
            // Show image preview if exists
            if (department.imagePath) {
                document.getElementById('departmentImagePreview').src = `https://localhost:7230${department.imagePath}`;
                document.getElementById('departmentImagePreviewContainer').style.display = 'block';
            } else {
                document.getElementById('departmentImagePreviewContainer').style.display = 'none';
            }
            
            // Clear existing services and add current ones
            document.getElementById('servicesContainer').innerHTML = '';
            if (services && services.length > 0) {
                services.forEach(service => {
                    addServiceField(service.id, service.name, service.description);
                });
            } else {
                addServiceField();
            }
            
            const modal = new bootstrap.Modal(document.getElementById('departmentModal'));
            modal.show();
            
        } catch (error) {
            console.error('Error loading department:', error);
            showAlert('error', 'Failed to load department details');
        }
    }

    // Add service field to department form
    function addServiceField(id = '', name = '', description = '') {
        const container = document.getElementById('servicesContainer');
        const serviceId = id || Date.now(); // Use existing ID or generate a temporary one
        
        const serviceDiv = document.createElement('div');
        serviceDiv.className = 'service-entry mb-3 p-3 border rounded';
        serviceDiv.dataset.serviceId = serviceId;
        
        serviceDiv.innerHTML = `
            <div class="row">
                <div class="col-md-5 mb-2">
                    <label class="form-label">Service Name</label>
                    <input type="text" class="form-control service-name" value="${name}" required>
                </div>
                <div class="col-md-5 mb-2">
                    <label class="form-label">Description</label>
                    <input type="text" class="form-control service-description" value="${description}" required>
                </div>
                <div class="col-md-2 d-flex align-items-end mb-2">
                    <button type="button" class="btn btn-danger btn-sm remove-service">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
        `;
        
        container.appendChild(serviceDiv);
        
        // Add event listener to remove button
        serviceDiv.querySelector('.remove-service').addEventListener('click', function() {
            if (container.children.length > 1) {
                container.removeChild(serviceDiv);
            } else {
                // Clear fields instead of removing the last service
                serviceDiv.querySelector('.service-name').value = '';
                serviceDiv.querySelector('.service-description').value = '';
            }
        });
    }

    // Add service button
    document.getElementById('addServiceBtn').addEventListener('click', function() {
        addServiceField();
    });

    document.getElementById('saveDepartmentBtn').addEventListener('click', async () => {
    const form = document.getElementById('departmentForm');
    const id = document.getElementById('departmentId').value;
    const isEdit = !!id;
    
    // Validate required fields
    const name = document.getElementById('departmentName').value;
    const description = document.getElementById('departmentDescription').value;
    
    if (!name || !description) {
        showAlert('error', 'Name and Description are required fields');
        return;
    }
    
    try {
        const formData = new FormData();
        formData.append('Name', name);
        formData.append('Description', description);
        
        // Add image file if selected
        const imageFile = document.getElementById('departmentImage').files[0];
        if (imageFile) {
            formData.append('ImageFile', imageFile);
        }
        
        // Collect services data
        const services = [];
        document.querySelectorAll('.service-entry').forEach(serviceDiv => {
            const serviceName = serviceDiv.querySelector('.service-name').value;
            const serviceDescription = serviceDiv.querySelector('.service-description').value;
            
            if (serviceName && serviceDescription) {
                services.push({
                    Name: serviceName,
                    Description: serviceDescription
                });
            }
        });
        
        // Add services to form data as JSON string
        formData.append('ServicesJson', JSON.stringify(services));
        
        if (isEdit) {
            formData.append('Id', id);
            await fetchData(`/api/departments/${id}`, {
                method: 'PUT',
                body: formData
            }, false);
        } else {
            await fetchData('/api/departments', {
                method: 'POST',
                body: formData
            }, false);
        }
        
        showAlert('success', `Department ${isEdit ? 'updated' : 'added'} successfully`);
        loadDepartments();
        
        const modal = bootstrap.Modal.getInstance(document.getElementById('departmentModal'));
        modal.hide();
        
    } catch (error) {
        console.error('Error saving department:', error);
        showAlert('error', `Failed to ${isEdit ? 'update' : 'add'} department: ${error.message}`);
    }
});





    // Blog CRUD operations
    document.getElementById('addBlogBtn').addEventListener('click', () => {
        document.getElementById('blogModalTitle').textContent = 'Add New Blog Post';
        document.getElementById('blogId').value = '';
        document.getElementById('blogForm').reset();
        document.getElementById('blogImagePreviewContainer').style.display = 'none';
        
        const modal = new bootstrap.Modal(document.getElementById('blogModal'));
        modal.show();
    });

    async function editBlog(id) {
        try {
            const blog = await fetchData(`/api/blogs/${id}`);
            
            document.getElementById('blogModalTitle').textContent = 'Edit Blog Post';
            document.getElementById('blogId').value = blog.id;
            document.getElementById('blogTitle').value = blog.title;
            document.getElementById('blogContent').value = blog.content;
            
            // Show image preview if exists
            if (blog.imageUrl) {
                document.getElementById('blogImagePreview').src = blog.imageUrl;
                document.getElementById('blogImagePreviewContainer').style.display = 'block';
            } else {
                document.getElementById('blogImagePreviewContainer').style.display = 'none';
            }
            
            const modal = new bootstrap.Modal(document.getElementById('blogModal'));
            modal.show();
            
        } catch (error) {
            console.error('Error loading blog:', error);
            showAlert('error', 'Failed to load blog details');
        }
    }

    // Blog Form Submission
    document.getElementById('saveBlogBtn').addEventListener('click', async () => {
        const form = document.getElementById('blogForm');
        const id = document.getElementById('blogId').value;
        const isEdit = !!id;
        
        // Validate required fields
        const title = document.getElementById('blogTitle').value;
        const content = document.getElementById('blogContent').value;
        
        if (!title || !content) {
            showAlert('error', 'Title and Content are required fields');
            return;
        }

        try {
            const formData = new FormData();
            formData.append('Title', title);
            formData.append('Content', content);
            
            const imageFile = document.getElementById('blogImage').files[0];
            if (imageFile) {
                formData.append('ImageFile', imageFile);
            }

            if (isEdit) {
                formData.append('Id', id);
                await fetchData(`/api/blogs/${id}`, {
                    method: 'PUT',
                    body: formData
                }, false);
            } else {
                await fetchData('/api/blogs', {
                    method: 'POST',
                    body: formData
                }, false);
            }
            
            showAlert('success', `Blog post ${isEdit ? 'updated' : 'added'} successfully`);
            loadBlogs();
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('blogModal'));
            modal.hide();
            
        } catch (error) {
            console.error('Error saving blog:', error);
            showAlert('error', `Failed to ${isEdit ? 'update' : 'add'} blog post: ${error.message}`);
        }
    });

    // Appointment status update
    async function updateAppointmentStatus(id, status) {
        try {
            await fetchData(`/api/bookings/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    Id: parseInt(id),
                    IsConfirmed: status
                })
            });
            
            showAlert('success', 'Appointment status updated successfully');
            loadAppointments();
            
        } catch (error) {
            console.error('Error updating appointment:', error);
            showAlert('error', 'Failed to update appointment status');
        }
    }

    // Delete confirmation
    let currentDeleteType = '';
    let currentDeleteId = '';

    function confirmDelete(type, id) {
        currentDeleteType = type;
        currentDeleteId = id;
        
        const modal = new bootstrap.Modal(document.getElementById('confirmationModal'));
        document.getElementById('confirmationMessage').textContent = `Are you sure you want to delete this ${type}?`;
        modal.show();
    }

    document.getElementById('confirmActionBtn').addEventListener('click', async () => {
        try {
            let endpoint = '';
            switch (currentDeleteType) {
                case 'doctor':
                    endpoint = `/api/doctors/${currentDeleteId}`;
                    break;
                case 'department':
                    endpoint = `/api/departments/${currentDeleteId}`;
                    break;
                case 'blog':
                    endpoint = `/api/blogs/${currentDeleteId}`;
                    break;
                case 'appointment':
                    endpoint = `/api/bookings/${currentDeleteId}`;
                    break;
                case 'contact':
                    endpoint = `/api/contacts/${currentDeleteId}`;
                    break;
            }
            
            await fetchData(endpoint, {
                method: 'DELETE'
            });
            
            showAlert('success', `${currentDeleteType.charAt(0).toUpperCase() + currentDeleteType.slice(1)} deleted successfully`);
            
            // Refresh the appropriate table
            switch (currentDeleteType) {
                case 'doctor':
                    loadDoctors();
                    break;
                case 'department':
                    loadDepartments();
                    break;
                case 'blog':
                    loadBlogs();
                    break;
                case 'appointment':
                    loadAppointments();
                    break;
                case 'contact':
                    loadContacts();
                    break;
            }
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('confirmationModal'));
            modal.hide();
            
        } catch (error) {
            console.error('Error deleting:', error);
            showAlert('error', `Failed to delete ${currentDeleteType}`);
        }
    });

    // Image preview handlers
    document.getElementById('doctorImage').addEventListener('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(event) {
                document.getElementById('doctorImagePreview').src = event.target.result;
                document.getElementById('doctorImagePreviewContainer').style.display = 'block';
            };
            reader.readAsDataURL(file);
        }
    });

    document.getElementById('blogImage').addEventListener('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(event) {
                document.getElementById('blogImagePreview').src = event.target.result;
                document.getElementById('blogImagePreviewContainer').style.display = 'block';
            };
            reader.readAsDataURL(file);
        }
    });

    document.getElementById('departmentImage').addEventListener('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(event) {
                document.getElementById('departmentImagePreview').src = event.target.result;
                document.getElementById('departmentImagePreviewContainer').style.display = 'block';
            };
            reader.readAsDataURL(file);
        }
    });

    // Logout
    document.getElementById('logoutBtn').addEventListener('click', function(e) {
        e.preventDefault();
        localStorage.removeItem('healthcare_token');
        localStorage.removeItem('healthcare_user');
        window.location.href = 'login.html';
    });

    // Helper function to fetch data
    async function fetchData(endpoint, options = {}, isJson = true) {
        const token = localStorage.getItem('healthcare_token');
        
        if (!token) {
            window.location.href = 'login.html';
            throw new Error('No token found');
        }

        const defaultOptions = {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        };
        
        const finalOptions = {
            ...defaultOptions,
            ...options
        };
        
        // Don't set Content-Type for FormData
        if (!(finalOptions.body instanceof FormData)) {
            finalOptions.headers['Content-Type'] = 'application/json';
        }
        
        const response = await fetch(`https://localhost:7230${endpoint}`, finalOptions);
        
        if (!response.ok) {
            let errorMsg = 'Request failed';
            try {
                const errorData = await response.json();
                errorMsg = errorData.message || JSON.stringify(errorData);
            } catch (e) {
                errorMsg = await response.text();
            }
            throw new Error(errorMsg);
        }
        
        // Handle empty responses (204 No Content)
        if (response.status === 204) {
            return null;
        }
        
        return isJson ? await response.json() : response;
    }

    // Show alert message
    function showAlert(type, message) {
        const alert = document.createElement('div');
        alert.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        alert.style.top = '20px';
        alert.style.right = '20px';
        alert.style.zIndex = '9999';
        alert.style.minWidth = '300px';
        alert.role = 'alert';
        alert.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        document.body.appendChild(alert);
        
        setTimeout(() => {
            alert.classList.remove('show');
            setTimeout(() => alert.remove(), 150);
        }, 5000);
    }

    // Initialize dashboard on load
    loadDashboardData();
    loadDoctors();
    loadDepartments();
    loadBlogs();
    loadAppointments();
    loadContacts();
});