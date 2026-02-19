(function () {
    var PAGE_SIZE = 10;

    var searchInput = document.getElementById('empSearchInput');
    var pills = document.querySelectorAll('.emp-pill');
    var allRows = Array.prototype.slice.call(document.querySelectorAll('#empTable tbody tr'));
    var countEl = document.getElementById('empCount');

    var currentFilter = 'all';
    var currentPage = 1;
    var filteredRows = [];

    // ── Filter ────────────────────────────────────────────────────────────────
    function getFilteredRows() {
        var query = searchInput ? searchInput.value.toLowerCase().trim() : '';
        return allRows.filter(function (row) {
            var matchSearch = !query || (row.dataset.search && row.dataset.search.includes(query));
            var matchStatus = currentFilter === 'all' || row.dataset.status === currentFilter;
            return matchSearch && matchStatus;
        });
    }

    // ── Pagination controls ───────────────────────────────────────────────────
    function renderPagination(total) {
        var totalPages = Math.ceil(total / PAGE_SIZE) || 1;

        // Remove existing pagination
        var old = document.getElementById('empPagination');
        if (old) old.parentNode.removeChild(old);

        if (totalPages <= 1) return;

        var footer = document.querySelector('.emp-table-footer');
        if (!footer) return;

        var nav = document.createElement('div');
        nav.id = 'empPagination';
        nav.className = 'emp-pagination';

        function makeBtn(label, page, disabled, active) {
            var btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'emp-page-btn' + (active ? ' active' : '') + (disabled ? ' disabled' : '');
            // Use textContent so characters render correctly
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
            span.className = 'emp-page-dots';
            span.textContent = '…';
            return span;
        }

        // Prev arrow
        nav.appendChild(makeBtn('‹', currentPage - 1, currentPage === 1, false));

        // Page numbers — show up to 5 around current
        var start = Math.max(1, currentPage - 2);
        var end = Math.min(totalPages, start + 4);
        start = Math.max(1, end - 4);

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

        // Next arrow
        nav.appendChild(makeBtn('›', currentPage + 1, currentPage === totalPages, false));

        footer.appendChild(nav);
    }

    // ── Master render ─────────────────────────────────────────────────────────
    function render() {
        filteredRows = getFilteredRows();
        var total = filteredRows.length;
        var totalPages = Math.ceil(total / PAGE_SIZE) || 1;

        if (currentPage > totalPages) currentPage = totalPages;
        if (currentPage < 1) currentPage = 1;

        var start = (currentPage - 1) * PAGE_SIZE;
        var end = start + PAGE_SIZE;

        allRows.forEach(function (row) { row.style.display = 'none'; });
        filteredRows.slice(start, end).forEach(function (row) { row.style.display = ''; });

        if (countEl) {
            if (total === 0) {
                countEl.innerHTML = 'No employees found';
            } else {
                var from = start + 1;
                var to = Math.min(end, total);
                countEl.innerHTML =
                    'Showing <strong>' + from + '–' + to + '</strong> of <strong>' + total + '</strong> employees';
            }
        }

        renderPagination(total);
    }

    // ── Event listeners ───────────────────────────────────────────────────────
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            currentPage = 1;
            render();
        });
    }

    pills.forEach(function (pill) {
        pill.addEventListener('click', function () {
            pills.forEach(function (p) { p.classList.remove('active'); });
            pill.classList.add('active');
            currentFilter = pill.dataset.filter;
            currentPage = 1;
            render();
        });
    });

    // ── Init ──────────────────────────────────────────────────────────────────
    render();
}());