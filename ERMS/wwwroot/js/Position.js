'use strict';

document.addEventListener('DOMContentLoaded', function () {
    createTableFilter({
        searchInputId: 'posSearchInput',
        pillSelector: '.emp-pill',
        countId: 'posCount',
        rowSelector: '#posTable tbody tr',
        footerSelector: '.emp-table-footer',
        paginationId: 'posPagination',
        resetBtnId: 'posResetFilters',
        pageSize: 15,
        entityName: 'positions',

        sortSelectId: 'posSortSelect',
        sortSelectOptions: {
            'title-asc': function (a, b) {
                var at = (a.dataset.title || '');
                var bt = (b.dataset.title || '');
                return at < bt ? -1 : at > bt ? 1 : 0;
            },
            'title-desc': function (a, b) {
                var at = (a.dataset.title || '');
                var bt = (b.dataset.title || '');
                return at > bt ? -1 : at < bt ? 1 : 0;
            },
            'salary-desc': function (a, b) {
                return parseFloat(b.dataset.salary || 0) - parseFloat(a.dataset.salary || 0);
            },
            'salary-asc': function (a, b) {
                return parseFloat(a.dataset.salary || 0) - parseFloat(b.dataset.salary || 0);
            },
            'employees-desc': function (a, b) {
                return parseInt(b.dataset.employees || 0, 10) - parseInt(a.dataset.employees || 0, 10);
            }
        }
    });
});