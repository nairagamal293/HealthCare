document.addEventListener('DOMContentLoaded', function () {

    /*** UI BEHAVIOR ***/

    // Preloader
    const preloader = document.querySelector('.preloader');
    if (preloader) {
        window.addEventListener('load', function () {
            preloader.style.opacity = '0';
            preloader.style.visibility = 'hidden';
            setTimeout(() => preloader.style.display = 'none', 500);
        });
    }

    // Sticky Navbar
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function () {
            navbar.classList.toggle('sticky', window.scrollY > 100);
        });
    }

    // Back to Top Button
    const backToTop = document.querySelector('.back-to-top');
    if (backToTop) {
        window.addEventListener('scroll', function () {
            backToTop.classList.toggle('active', window.scrollY > 300);
        });

        backToTop.addEventListener('click', function (e) {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // Counter Animation
    const counters = document.querySelectorAll('.counter');
    if (counters.length > 0) {
        const speed = 200;

        function animateCounters() {
            counters.forEach(counter => {
                const target = +counter.getAttribute('data-count');
                const count = +counter.innerText;
                const increment = target / speed;

                if (count < target) {
                    counter.innerText = Math.ceil(count + increment);
                    setTimeout(animateCounters, 1);
                } else {
                    counter.innerText = target;
                }
            });
        }

        const observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    animateCounters();
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.5 });

        counters.forEach(counter => observer.observe(counter));
    }

    // Smooth Scroll for Anchors
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();

            const targetId = this.getAttribute('href');
            if (targetId === '#') return;

            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                window.scrollTo({
                    top: targetElement.offsetTop - 80,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Bootstrap Tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(el => new bootstrap.Tooltip(el));

    // Bootstrap Carousels
    const testimonialCarousel = document.getElementById('testimonialCarousel');
    if (testimonialCarousel) {
        new bootstrap.Carousel(testimonialCarousel, {
            interval: 5000,
            pause: 'hover'
        });
    }

    const heroCarousel = document.getElementById('heroCarousel');
    if (heroCarousel) {
        new bootstrap.Carousel(heroCarousel, {
            interval: 6000,
            pause: 'hover',
            ride: 'carousel'
        });
    }

    // Mobile Dropdown Animations
    document.querySelectorAll('.navbar .dropdown').forEach(dropdown => {
        dropdown.addEventListener('show.bs.dropdown', function () {
            this.querySelector('.dropdown-menu').classList.add('animate__animated', 'animate__fadeIn');
        });
    });

    // Form Validation
    document.querySelectorAll('.needs-validation').forEach(form => {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });

    /*** API DATA LOADING ***/
    const API_BASE_URL = 'https://localhost:7230/api';

    // Load Departments
    async function loadDepartments() {
        try {
            const response = await fetch(`${API_BASE_URL}/departments`);
            const departments = await response.json();
            const container = document.getElementById('departments-container');
            if (!container) return;

            const limited = departments.slice(0, 6);
            container.innerHTML = limited.map(dept => `
                <div class="col-md-6 col-lg-4">
                    <div class="service-card">
                        <div class="service-icon">
                            <i class="${getDepartmentIcon(dept.name)}"></i>
                        </div>
                        <h3>${dept.name}</h3>
                        <p>${dept.description || 'Comprehensive care and treatment for all related conditions.'}</p>
                        <a href="departments.html#${dept.name.toLowerCase()}" class="service-link">Read More <i class="fas fa-arrow-right"></i></a>
                    </div>
                </div>
            `).join('');
        } catch (err) {
            console.error('Error loading departments:', err);
        }
    }

    // Get Icon for Department
    function getDepartmentIcon(name) {
        const icons = {
            'Cardiology': 'fas fa-heartbeat',
            'Neurology': 'fas fa-brain',
            'Orthopedics': 'fas fa-bone',
            'Pediatrics': 'fas fa-baby',
            'Radiology': 'fas fa-x-ray',
            'Surgery': 'fas fa-procedures'
        };
        return icons[name] || 'fas fa-stethoscope';
    }

    // Load Doctors
    async function loadDoctors() {
        try {
            const response = await fetch(`${API_BASE_URL}/doctors`);
            const doctors = await response.json();
            const container = document.getElementById('doctors-container');
            if (!container) return;

            const limited = doctors.slice(0, 4);
            container.innerHTML = limited.map(doctor => `
                <div class="col-md-6 col-lg-3">
                    <div class="doctor-card">
                        <div class="doctor-img">
                        
                            <img src="https://localhost:7230${doctor.imagePath}" alt="${doctor.name}" class="img-fluid">
                            
                        </div>
                        <div class="doctor-info">
                            <h4>${doctor.name}</h4>
                            <p>${doctor.specialty}</p>
                            
                            <div class="doctor-meta">
                                <span><i class="fas fa-star"></i> 4.9 (120 reviews)</span>
                            </div>
                            <a href="doctor-details.html?id=${doctor.id}" class="btn btn-sm btn-outline-primary">View Profile</a>
                        </div>
                    </div>
                </div>
            `).join('');
        } catch (err) {
            console.error('Error loading doctors:', err);
        }
    }

    // Load Blogs
    async function loadBlogs() {
        try {
            const response = await fetch(`${API_BASE_URL}/blogs`);
            const blogs = await response.json();
            const container = document.getElementById('blogs-container');
            if (!container) return;

            const limited = blogs.slice(0, 3);
            container.innerHTML = limited.map(blog => {
                const date = new Date(blog.createdAt);
                const month = date.toLocaleString('default', { month: 'short' });
                const day = date.getDate();

                return `
                    <div class="col-md-6 col-lg-4">
                        <div class="blog-card">
                            <div class="blog-img">
                            
                                <img src="https://localhost:7230${blog.imagePath}" alt="${blog.title}" class="img-fluid">
                                <div class="blog-date">
                                    <span>${day}</span>
                                    <span>${month}</span>
                                </div>
                            </div>
                            <div class="blog-content">
                                
                                <h3><a href="blog-single.html?id=${blog.id}">${blog.title}</a></h3>
                                <p>${blog.content.substring(0, 100)}...</p>
                                <a href="blog-single.html?id=${blog.id}" class="read-more">Read More <i class="fas fa-arrow-right"></i></a>
                            </div>
                        </div>
                    </div>
                `;
            }).join('');
        } catch (err) {
            console.error('Error loading blogs:', err);
        }
    }

    /*** INITIALIZE DATA FETCHING ***/
    loadDepartments();
    loadDoctors();
    loadBlogs();
});
