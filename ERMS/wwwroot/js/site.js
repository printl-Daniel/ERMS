'use strict';

// ── THEME ─────────────────────────────────────────────────────────────
(function () {
    var STORAGE_KEY = 'erms-theme';
    var body = document.body;

    function setTheme(theme) {
        body.classList.toggle('dark-mode', theme === 'dark');
        localStorage.setItem(STORAGE_KEY, theme);
    }

    function getCurrentTheme() {
        return localStorage.getItem(STORAGE_KEY) ||
            (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
    }

    // Apply theme immediately to avoid flash
    setTheme(getCurrentTheme());

    document.addEventListener('DOMContentLoaded', function () {
        var toggle = document.getElementById('themeToggle');
        if (!toggle) return;

        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            setTheme(body.classList.contains('dark-mode') ? 'light' : 'dark');
        });

        if (window.matchMedia) {
            window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function (e) {
                if (!localStorage.getItem(STORAGE_KEY)) setTheme(e.matches ? 'dark' : 'light');
            });
        }

        setTimeout(function () {
            body.style.transition = 'background-color 0.3s ease, color 0.3s ease';
        }, 50);
    });
})();

// ── ACTIVE NAV ────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    var path = window.location.pathname;
    document.querySelectorAll('.navbar-nav > li > a').forEach(function (a) {
        var href = a.getAttribute('href');
        if (href && path.indexOf(href) === 0 && href !== '/') {
            a.parentElement.classList.add('active');
        }
    });
});

// ── TOAST ─────────────────────────────────────────────────────────────
var TOAST_DURATION = 4500;

var TOAST_CONFIG = {
    success: { icon: '✓', title: 'Success' },
    error: { icon: '✕', title: 'Error' },
    warning: { icon: '⚠', title: 'Warning' },
    info: { icon: 'ℹ', title: 'Info' }
};

function showToast(type, message) {
    var cfg = TOAST_CONFIG[type] || TOAST_CONFIG.info;
    var container = document.getElementById('toast-container');

    var toast = document.createElement('div');
    toast.className = 'toast-item toast-' + type;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'polite');

    toast.innerHTML =
        '<span class="toast-icon" aria-hidden="true">' + cfg.icon + '</span>' +
        '<div class="toast-body">' +
        '<div class="toast-title">' + cfg.title + '</div>' +
        '<div class="toast-message">' + message + '</div>' +
        '</div>' +
        '<button class="toast-close" aria-label="Dismiss" onclick="dismissToast(this.parentElement)">✕</button>' +
        '<div class="toast-progress" style="animation-duration:' + TOAST_DURATION + 'ms"></div>';

    container.appendChild(toast);

    var timer = setTimeout(function () { dismissToast(toast); }, TOAST_DURATION);

    toast.addEventListener('mouseenter', function () {
        clearTimeout(timer);
        toast.querySelector('.toast-progress').style.animationPlayState = 'paused';
    });

    toast.addEventListener('mouseleave', function () {
        toast.querySelector('.toast-progress').style.animationPlayState = 'running';
        timer = setTimeout(function () { dismissToast(toast); }, 1200);
    });
}

function dismissToast(toast) {
    if (!toast || toast.classList.contains('toast-hide')) return;
    toast.classList.add('toast-hide');
    setTimeout(function () {
        if (toast.parentElement) toast.parentElement.removeChild(toast);
    }, 320);
}

window.showToast = showToast;
window.dismissToast = dismissToast;