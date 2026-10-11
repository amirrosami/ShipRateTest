// ============================================
// GlobalTrans Logistics — Site JavaScript
// Bootstrap 5.3 — No jQuery Required
// ============================================

document.addEventListener('DOMContentLoaded', function () {

    // ================================
    // Sidebar Toggle
    // ================================
    const sidebar = document.getElementById('appSidebar');
    const contentWrapper = document.getElementById('contentWrapper');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const overlay = document.getElementById('sidebarOverlay');

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            const isMobile = window.innerWidth < 992;
            if (isMobile) {
                sidebar.classList.toggle('mobile-open');
                overlay.classList.toggle('active');
            } else {
                sidebar.classList.toggle('collapsed');
                contentWrapper.classList.toggle('sidebar-collapsed');
            }
        });
    }

    if (overlay) {
        overlay.addEventListener('click', function () {
            sidebar.classList.remove('mobile-open');
            overlay.classList.remove('active');
        });
    }

    // ================================
    // Sidebar Submenu Toggle
    // ================================
    document.querySelectorAll('[data-menu] > .nav-link').forEach(function (link) {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const parent = link.parentElement;
            const wasOpen = parent.classList.contains('open');

            // Close all other open menus
            document.querySelectorAll('[data-menu].open').forEach(function (item) {
                if (item !== parent) {
                    item.classList.remove('open');
                }
            });

            // Toggle current
            parent.classList.toggle('open', !wasOpen);
        });
    });

    // ================================
    // Fullscreen Toggle
    // ================================
    const fullscreenBtn = document.getElementById('fullscreenBtn');
    if (fullscreenBtn) {
        fullscreenBtn.addEventListener('click', function () {
            if (!document.fullscreenElement) {
                document.documentElement.requestFullscreen();
            } else {
                document.exitFullscreen();
            }
        });
    }

    // ================================
    // Server Time
    // ================================
    const serverTimeEl = document.getElementById('serverTime');
    if (serverTimeEl) {
        function updateServerTime() {
            const now = new Date();
            const options = {
                year: 'numeric',
                month: 'short',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: false,
                timeZoneName: 'short'
            };
            serverTimeEl.textContent = now.toLocaleDateString('en-US', options);
        }
        updateServerTime();
        setInterval(updateServerTime, 1000);
    }

    // ================================
    // Close mobile sidebar on window resize
    // ================================
    window.addEventListener('resize', function () {
        if (window.innerWidth >= 992) {
            if (sidebar) {
                sidebar.classList.remove('mobile-open');
            }
            if (overlay) {
                overlay.classList.remove('active');
            }
        }
    });

    // ================================
    // Sidebar Menu Search Filter
    // ================================
    const sidebarSearchInput = document.querySelector('.sidebar-search .form-control');
    if (sidebarSearchInput) {
        sidebarSearchInput.addEventListener('input', function () {
            const filter = this.value.toLowerCase().trim();
            const navItems = document.querySelectorAll('.sidebar-nav .nav-item[data-menu]');

            navItems.forEach(function (item) {
                const text = item.textContent.toLowerCase();
                if (filter === '') {
                    item.style.display = '';
                    item.classList.remove('open');
                } else if (text.includes(filter)) {
                    item.style.display = '';
                    item.classList.add('open');
                } else {
                    item.style.display = 'none';
                }
            });

            // Also filter non-menu items
            document.querySelectorAll('.sidebar-nav .nav-header').forEach(function (header) {
                if (filter === '') {
                    header.style.display = '';
                } else {
                    header.style.display = 'none';
                }
            });
        });
    }

});
