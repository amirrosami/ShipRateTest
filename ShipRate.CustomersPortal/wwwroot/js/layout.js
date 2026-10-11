(function () {
    'use strict';

    // ── DOM ELEMENTS ─────────────────────────
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    const hamburgerBtn = document.getElementById('hamburgerBtn');
    const sidebarCloseBtn = document.getElementById('sidebarCloseBtn');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    const themeToggleBtn = document.getElementById('themeToggleBtn');
    const themeIcon = document.getElementById('themeIcon');
    const html = document.documentElement;

    // ── HELPERS ──────────────────────────────
    function isDesktop() {
        return window.innerWidth >= 1200;
    }

    // ── SIDEBAR TOGGLE ───────────────────────
    function openSidebarMobile() {
        sidebar.classList.add('mobile-open');
        sidebar.classList.remove('collapsed');
        sidebarOverlay.classList.add('show');
        document.body.style.overflow = 'hidden';
    }

    function closeSidebarMobile() {
        sidebar.classList.remove('mobile-open');
        sidebarOverlay.classList.remove('show');
        document.body.style.overflow = '';
    }

    function toggleSidebarDesktop() {
        sidebar.classList.toggle('collapsed');
        mainContent.classList.toggle('expanded');
        localStorage.setItem('sidebar-collapsed', sidebar.classList.contains('collapsed'));
    }

    // Hamburger button click
    hamburgerBtn.addEventListener('click', function () {
        if (isDesktop()) {
            toggleSidebarDesktop();
        } else {
            openSidebarMobile();
        }
    });

    // Close button (mobile)
    sidebarCloseBtn.addEventListener('click', function () {
        closeSidebarMobile();
    });

    // Overlay click (mobile)
    sidebarOverlay.addEventListener('click', function () {
        closeSidebarMobile();
    });

    // ── CLOSE SIDEBAR ON MENU CLICK (MOBILE) ─
    const allMenuLinks = sidebar.querySelectorAll('.menu-link:not(.has-submenu > .menu-link), .submenu-link');
    allMenuLinks.forEach(function (link) {
        link.addEventListener('click', function () {
            if (!isDesktop()) {
                closeSidebarMobile();
            }
        });
    });

    // ── RESTORE SIDEBAR STATE (DESKTOP) ─────
    function restoreSidebarState() {
        if (isDesktop()) {
            const isCollapsed = localStorage.getItem('sidebar-collapsed') === 'true';
            if (isCollapsed) {
                sidebar.classList.add('collapsed');
                mainContent.classList.add('expanded');
            } else {
                sidebar.classList.remove('collapsed');
                mainContent.classList.remove('expanded');
            }
            sidebar.classList.remove('mobile-open');
            sidebarOverlay.classList.remove('show');
            document.body.style.overflow = '';
        } else {
            sidebar.classList.remove('collapsed');
            mainContent.classList.remove('expanded');
            closeSidebarMobile();
        }
    }

    restoreSidebarState();
    window.addEventListener('resize', restoreSidebarState);

    // ── SUBMENU TOGGLE ───────────────────────
    const submenuItems = document.querySelectorAll('.menu-item.has-submenu');
    submenuItems.forEach(function (item) {
        const link = item.querySelector('.menu-link');
        link.addEventListener('click', function (e) {
            e.preventDefault();
            submenuItems.forEach(function (other) {
                if (other !== item) {
                    other.classList.remove('open');
                }
            });
            item.classList.toggle('open');
        });
    });

    // ── THEME TOGGLE ─────────────────────────
    function setTheme(theme) {
        html.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
        if (theme === 'dark') {
            themeIcon.className = 'bi bi-sun-fill';
        } else {
            themeIcon.className = 'bi bi-moon-fill';
        }
    }

    // Restore theme
    const savedTheme = localStorage.getItem('theme') || 'light';
    setTheme(savedTheme);

    themeToggleBtn.addEventListener('click', function () {
        const current = html.getAttribute('data-theme');
        setTheme(current === 'dark' ? 'light' : 'dark');
    });

    // ── ACTIVE MENU HIGHLIGHT ────────────────
    // This section highlights the current page in the sidebar
    // based on the URL. You can customize the matching logic.
    (function highlightActiveMenu() {
        var currentPath = window.location.pathname.toLowerCase();

        // Check submenu links first
        var submenuLinks = document.querySelectorAll('.submenu-link');
        submenuLinks.forEach(function (link) {
            var href = link.getAttribute('href');
            if (href && href !== '#' && currentPath.indexOf(href.toLowerCase()) !== -1) {
                link.classList.add('active');
                // Open parent submenu
                var parentSubmenu = link.closest('.menu-item.has-submenu');
                if (parentSubmenu) {
                    parentSubmenu.classList.add('open');
                }
            }
        });

        // Check main menu links
        var menuLinks = document.querySelectorAll('.menu-link:not(.has-submenu > .menu-link)');
        menuLinks.forEach(function (link) {
            var href = link.getAttribute('href');
            if (href && href !== '#' && currentPath.indexOf(href.toLowerCase()) !== -1) {
                // Remove active from dashboard first
                document.querySelectorAll('.menu-link.active').forEach(function (l) {
                    l.classList.remove('active');
                });
                link.classList.add('active');
            }
        });
    })();

})();
