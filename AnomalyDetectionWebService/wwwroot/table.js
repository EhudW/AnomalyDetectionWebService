//tb_ prefix to avoid collision with modle_list
// use headerName for title
const tb_columnDefs = [
    { headerName:"Show In graph", flex: 2, checkboxSelection: true },
    { field: "feature", flex: 2, sortable: true, filter: 'agTextColumnFilter', floatingFilter: true },
    { field: "some information", flex: 2, sortable: true }
];

// specify the data [example]
const tb_rowData = [
    { "feature": "speed", "some information": "WOW"},
    { "feature": "height", "some information": "Amazing"},
    { "feature": "yaw", "some information": "ok"},
    { "feature": "pitch", "some information": "sounds good"},
    { "feature": "roll", "some information": "101.2"},
    { "feature": "temp", "some information": "Boxter"}
];

// let the grid know which columns and what data to use
const tb_gridOptions = {
    columnDefs: tb_columnDefs,
    rowData: tb_rowData,
    rowSelection: 'multiple',
    rowMultiSelectWithClick: false,
    suppressRowDeselection: true,
    suppressRowClickSelection: true,
    onRowSelected: tb_onRowSelected,
    pagination: true,
    paginationAutoPageSize: true
};

// setup the grid after the page has finished loading
document.addEventListener('DOMContentLoaded', () => {
    const tb_gridDiv = document.querySelector('#myGrid_table');
    new agGrid.Grid(tb_gridDiv, tb_gridOptions);
});

// NOT called if all the data changed by    gridOptions.api.setRowData(x)
function tb_onRowSelected(event) {
    //event.node.data.feature  speed/roll ...
    //event.node.isSelected()  true/false
}


// change all data by 
// var tb_x = [{ "feature": "speed", "some information": "WOW"}, ...]
// tb_gridOptions.api.setRowData(tb_x);