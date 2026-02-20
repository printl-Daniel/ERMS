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
    if (!container) return;

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

// ── TABLE FILTER + SORT + PAGINATION ──────────────────────────────────
function createTableFilter(options) {
    var pageSize = options.pageSize || 10;
    var entityName = options.entityName || 'records';

    var searchInput = document.getElementById(options.searchInputId);
    var pills = document.querySelectorAll(options.pillSelector);
    var countEl = document.getElementById(options.countId);
    var sortSelectEl = options.sortSelectId
        ? document.getElementById(options.sortSelectId) : null;

    var allRows = Array.prototype.slice.call(
        document.querySelectorAll(options.rowSelector));

    var dateFromInput = options.dateFromId
        ? document.getElementById(options.dateFromId) : null;
    var dateToInput = options.dateToId
        ? document.getElementById(options.dateToId) : null;
    var extraSelects = (options.selectFilters || []).map(function (f) {
        return { el: document.getElementById(f.id), field: f.dataField };
    }).filter(function (f) { return f.el; });

    var currentFilter = 'all';
    var currentPage = 1;
    var filteredRows = [];

    var sortCol = null;
    var sortDir = 'asc';

    var tableSelector = options.rowSelector.replace(' tbody tr', '');
    var thead = document.querySelector(tableSelector + ' thead');

    if (thead) {
        thead.querySelectorAll('th[data-sort]').forEach(function (th) {
            var indicator = document.createElement('span');
            indicator.className = 'sort-indicator';
            indicator.textContent = ' ↕';
            th.appendChild(indicator);

            th.addEventListener('click', function () {
                var col = th.dataset.sort;
                sortDir = (sortCol === col && sortDir === 'asc') ? 'desc' : 'asc';
                sortCol = col;

                if (sortSelectEl) sortSelectEl.value = '';

                thead.querySelectorAll('th[data-sort] .sort-indicator').forEach(function (s) {
                    s.textContent = ' ↕';
                    s.classList.remove('sort-asc', 'sort-desc');
                });
                indicator.textContent = sortDir === 'asc' ? ' ↑' : ' ↓';
                indicator.classList.add(sortDir === 'asc' ? 'sort-asc' : 'sort-desc');

                currentPage = 1;
                render();
            });
        });
    }

    // ── Sort resolver ──────────────────────────────────────────────────
    function getSortedRows(rows) {
        // 1. Sort-select takes priority
        if (sortSelectEl && sortSelectEl.value) {
            var val = sortSelectEl.value;
            var customFns = options.sortSelectOptions || {};

            if (customFns[val]) {
                return rows.slice().sort(customFns[val]);
            }

            var parts = val.match(/^(.+)-(asc|desc)$/);
            if (parts) {
                var field = parts[1];
                var dir = parts[2];
                return rows.slice().sort(function (a, b) {
                    var aVal = (a.dataset[field] || '').toLowerCase();
                    var bVal = (b.dataset[field] || '').toLowerCase();
                    var aNum = parseFloat(aVal);
                    var bNum = parseFloat(bVal);
                    if (!isNaN(aNum) && !isNaN(bNum)) {
                        return dir === 'asc' ? aNum - bNum : bNum - aNum;
                    }
                    if (aVal < bVal) return dir === 'asc' ? -1 : 1;
                    if (aVal > bVal) return dir === 'asc' ? 1 : -1;
                    return 0;
                });
            }
        }

        // 2. Column-header sort
        if (!sortCol) return rows;
        return rows.slice().sort(function (a, b) {
            var aVal = (a.dataset[sortCol] || '').toLowerCase();
            var bVal = (b.dataset[sortCol] || '').toLowerCase();

            var aNum = parseFloat(aVal);
            var bNum = parseFloat(bVal);
            if (!isNaN(aNum) && !isNaN(bNum)) {
                return sortDir === 'asc' ? aNum - bNum : bNum - aNum;
            }

            var aDate = new Date(aVal);
            var bDate = new Date(bVal);
            if (!isNaN(aDate.getTime()) && !isNaN(bDate.getTime())) {
                return sortDir === 'asc' ? aDate - bDate : bDate - aDate;
            }

            if (aVal < bVal) return sortDir === 'asc' ? -1 : 1;
            if (aVal > bVal) return sortDir === 'asc' ? 1 : -1;
            return 0;
        });
    }

    // ── Filter logic ───────────────────────────────────────────────────
    function getFilteredRows() {
        var query = searchInput ? searchInput.value.toLowerCase().trim() : '';
        var dateFrom = dateFromInput && dateFromInput.value
            ? new Date(dateFromInput.value) : null;
        var dateTo = dateToInput && dateToInput.value
            ? new Date(dateToInput.value) : null;

        return allRows.filter(function (row) {
            var matchSearch = !query || (row.dataset.search || '').includes(query);
            var matchStatus = currentFilter === 'all' || row.dataset.status === currentFilter;

            var matchDate = true;
            if (dateFrom || dateTo) {
                var rowDate = row.dataset.date ? new Date(row.dataset.date) : null;
                if (rowDate) {
                    if (dateFrom && rowDate < dateFrom) matchDate = false;
                    if (dateTo && rowDate > dateTo) matchDate = false;
                } else {
                    matchDate = false;
                }
            }

            var matchSelects = extraSelects.every(function (f) {
                var val = f.el.value;
                return !val || (row.dataset[f.field] || '') === val;
            });

            return matchSearch && matchStatus && matchDate && matchSelects;
        });
    }

    // ── Pagination ─────────────────────────────────────────────────────
    function renderPagination(total) {
        var totalPages = Math.ceil(total / pageSize) || 1;
        var footer = options.footerSelector
            ? document.querySelector(options.footerSelector) : null;

        var old = document.getElementById(options.paginationId);
        if (old) old.parentNode.removeChild(old);
        if (!footer || totalPages <= 1) return;

        var nav = document.createElement('div');
        nav.id = options.paginationId;
        nav.className = options.paginationClass || 'table-pagination';

        function makeBtn(label, page, disabled, active) {
            var btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'page-btn' +
                (active ? ' active' : '') +
                (disabled ? ' disabled' : '');
            btn.textContent = label;
            btn.disabled = disabled;
            if (!disabled && !active) {
                btn.addEventListener('click', function () {
                    currentPage = page;
                    render();
                });
            }
            return btn;
        }

        function makeDots() {
            var span = document.createElement('span');
            span.className = 'page-dots';
            span.textContent = '…';
            return span;
        }

        var start = Math.max(1, currentPage - 2);
        var end = Math.min(totalPages, start + 4);
        start = Math.max(1, end - 4);

        nav.appendChild(makeBtn('‹', currentPage - 1, currentPage === 1, false));

        if (start > 1) {
            nav.appendChild(makeBtn('1', 1, false, false));
            if (start > 2) nav.appendChild(makeDots());
        }

        for (var p = start; p <= end; p++) {
            nav.appendChild(makeBtn(String(p), p, false, p === currentPage));
        }

        if (end < totalPages) {
            if (end < totalPages - 1) nav.appendChild(makeDots());
            nav.appendChild(makeBtn(String(totalPages), totalPages, false, false));
        }

        nav.appendChild(makeBtn('›', currentPage + 1, currentPage === totalPages, false));

        footer.appendChild(nav);
    }

    // ── Master render ──────────────────────────────────────────────────
    function render() {
        filteredRows = getSortedRows(getFilteredRows());

        var total = filteredRows.length;
        var totalPages = Math.ceil(total / pageSize) || 1;

        if (currentPage > totalPages) currentPage = totalPages;
        if (currentPage < 1) currentPage = 1;

        var start = (currentPage - 1) * pageSize;
        var end = start + pageSize;

        // ── Reorder rows in the DOM so the sort is visually applied ────
        if (allRows.length > 0) {
            var tbody = allRows[0].parentNode;
            var sortedAll = getSortedRows(allRows); // full set, sorted
            sortedAll.forEach(function (r) { tbody.appendChild(r); });
        }
        // ── End reorder ────────────────────────────────────────────────

        allRows.forEach(function (r) { r.style.display = 'none'; });
        filteredRows.slice(start, end).forEach(function (r) { r.style.display = ''; });

        if (countEl) {
            countEl.innerHTML = total === 0
                ? 'No ' + entityName + ' found'
                : 'Showing <strong>' + (start + 1) + '–' + Math.min(end, total) +
                '</strong> of <strong>' + total + '</strong> ' + entityName;
        }

        renderPagination(total);
    }

    // ── Reset ──────────────────────────────────────────────────────────
    function resetAll() {
        if (searchInput) searchInput.value = '';
        if (dateFromInput) dateFromInput.value = '';
        if (dateToInput) dateToInput.value = '';
        if (sortSelectEl) sortSelectEl.value = '';

        extraSelects.forEach(function (f) { f.el.value = ''; });

        pills.forEach(function (p) { p.classList.remove('active'); });
        var allPill = document.querySelector(
            options.pillSelector + '[data-filter="all"]');
        if (allPill) allPill.classList.add('active');
        currentFilter = 'all';

        sortCol = null;
        sortDir = 'asc';
        if (thead) {
            thead.querySelectorAll('th[data-sort] .sort-indicator').forEach(function (s) {
                s.textContent = ' ↕';
                s.classList.remove('sort-asc', 'sort-desc');
            });
        }

        currentPage = 1;
        render();
    }

    // ── Event listeners ────────────────────────────────────────────────
    function resetPage() { currentPage = 1; render(); }

    if (searchInput) searchInput.addEventListener('input', resetPage);
    if (sortSelectEl) sortSelectEl.addEventListener('change', resetPage);

    pills.forEach(function (pill) {
        pill.addEventListener('click', function () {
            pills.forEach(function (p) { p.classList.remove('active'); });
            pill.classList.add('active');
            currentFilter = pill.dataset.filter || 'all';
            resetPage();
        });
    });

    if (dateFromInput) dateFromInput.addEventListener('change', resetPage);
    if (dateToInput) dateToInput.addEventListener('change', resetPage);
    extraSelects.forEach(function (f) {
        f.el.addEventListener('change', resetPage);
    });

    if (options.resetBtnId) {
        var resetBtn = document.getElementById(options.resetBtnId);
        if (resetBtn) resetBtn.addEventListener('click', resetAll);
    }

    // ── Init ───────────────────────────────────────────────────────────
    render();
}

window.createTableFilter = createTableFilter;