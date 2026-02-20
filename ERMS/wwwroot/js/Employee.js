(function () {
    document.addEventListener('DOMContentLoaded', function () {
        createTableFilter({
            searchInputId: 'empSearchInput',
            pillSelector: '.emp-pill',
            countId: 'empCount',
            rowSelector: '#empTable tbody tr',
            footerSelector: '.emp-table-footer',
            paginationId: 'empPagination',
            paginationClass: 'emp-pagination',
            entityName: 'employees',
            pageSize: 10,
            resetBtnId: 'empResetFilters',    
            selectFilters: [
                { id: 'empDeptFilter', dataField: 'department' },
                { id: 'empPositionFilter', dataField: 'position' },
                { id: 'empManagerFilter', dataField: 'manager' },
                { id: 'empRoleFilter', dataField: 'role' },
                { id: 'empYearFilter', dataField: 'year' }
            ]
        });
    });
}());