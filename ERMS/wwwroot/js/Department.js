'use strict';

document.addEventListener('DOMContentLoaded', function () {
    createTableFilter({
        searchInputId: 'deptSearchInput',
        pillSelector: '.emp-pill',
        countId: 'deptCount',
        rowSelector: '#deptTable tbody tr',
        footerSelector: '.emp-table-footer',
        paginationId: 'deptPagination',
        resetBtnId: 'deptResetFilters',
        pageSize: 15,
        entityName: 'departments',

        sortSelectId: 'deptSortSelect',
        sortSelectOptions: {
            'name-asc': function (a, b) {
                var an = (a.dataset.name || '');
                var bn = (b.dataset.name || '');
                return an < bn ? -1 : an > bn ? 1 : 0;
            },
            'name-desc': function (a, b) {
                var an = (a.dataset.name || '');
                var bn = (b.dataset.name || '');
                return an > bn ? -1 : an < bn ? 1 : 0;
            },
            'newest': function (a, b) {
                return parseInt(b.dataset.id || 0, 10) -
                    parseInt(a.dataset.id || 0, 10);
            },
            'employees-desc': function (a, b) {
                return parseInt(b.dataset.employees || 0, 10) -
                    parseInt(a.dataset.employees || 0, 10);
            }
        }
    });
});