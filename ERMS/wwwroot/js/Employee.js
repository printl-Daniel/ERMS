(function () {
    var searchInput = document.getElementById('empSearchInput');
    var pills = document.querySelectorAll('.emp-pill');
    var rows = document.querySelectorAll('#empTable tbody tr');
    var countEl = document.getElementById('empCount');
    var currentFilter = 'all';

    function applyFilters() {
        var query = searchInput ? searchInput.value.toLowerCase().trim() : '';
        var visible = 0;

        rows.forEach(function (row) {
            var matchSearch = !query || row.dataset.search.includes(query);
            var matchStatus = currentFilter === 'all' || row.dataset.status === currentFilter;
            var show = matchSearch && matchStatus;
            row.style.display = show ? '' : 'none';
            if (show) visible++;
        });

        if (countEl) {
            countEl.innerHTML = 'Showing <strong>' + visible + '</strong> of <strong>' + rows.length + '</strong> employees';
        }
    }

    if (searchInput) {
        searchInput.addEventListener('input', applyFilters);
    }

    pills.forEach(function (pill) {
        pill.addEventListener('click', function () {
            pills.forEach(function (p) { p.classList.remove('active'); });
            pill.classList.add('active');
            currentFilter = pill.dataset.filter;
            applyFilters();
        });
    });
} ());