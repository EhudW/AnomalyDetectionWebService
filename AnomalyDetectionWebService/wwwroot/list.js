//ml_ prefix to avoid collision with table
const ml_columnDefs = [
    { headerName: "id", field: "model_id", sortable: true },
    { headerName: "upload", field: "upload_time", sortable: true },
    { headerName: "status", field: "status", sortable: true }
];

// specify the data [example]
const ml_rowData = [
    {
        "model_id": 123, "status": "ready", "upload_time" : "2021-04-22T19:15:32+02.00" },
    { "model_id": 124, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00"},
    { "model_id": 125, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00"},
    { "model_id": 1267, "status": "pending", "upload_time": "2021-04-22T19:15:32+02.00"},
    { "model_id": 333, "status": "pending", "upload_time": "2021-04-22T19:15:32+02.00"},
    { "model_id": 456, "status": "pending", "upload_time": "2021-04-22T19:15:32+02.00"}
];

// let the grid know which columns and what data to use
const ml_gridOptions = {
    columnDefs: ml_columnDefs,
    rowData: ml_rowData,
    rowSelection: 'multiple',
    rowMultiSelectWithClick: false,
    suppressRowDeselection: true,
    suppressRowClickSelection: true,
    onRowSelected: ml_onRowSelected,
    pagination: false,
    paginationAutoPageSize: true
};

// setup the grid after the page has finished loading
document.addEventListener('DOMContentLoaded', () => {
    const ml_gridDiv = document.querySelector('#myGrid_model_list');
    new agGrid.Grid(ml_gridDiv, ml_gridOptions);
});

// NOT called if all the data changed by    gridOptions.api.setRowData(x)
function ml_onRowSelected(event) {
    //event.node.data.feature  speed/roll ...
    //event.node.isSelected()  true/false
}


// change all data by 
// var ml_x = [{ "feature": "speed", "some information": "WOW"}, ...]
// ml_gridOptions.api.setRowData(ml_x);
