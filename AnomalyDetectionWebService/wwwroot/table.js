//tb_ prefix to avoid collision with modle_list
// use headerName for title
const tb_columnDefs = [
    //{ headerName:"", flex: 1, checkboxSelection: true },
    { field: "feature", sortable: true, filter: 'agTextColumnFilter', floatingFilter: true, checkboxSelection: true },
    { field: "highest", sortable: true },
    { field: "lowest", sortable: true },
    { field: "average", sortable: true },
    { field: "is anomaly", sortable: true },
    { field: "reason", sortable: true }
];

// specify the data [example]
const tb_rowData = [
    { "feature": "speed", "highest": "WOW", "lowest": "PFF", "average": "100", "is anomaly": "NO", "reason": "--" },
    { "feature": "height", "highest": "Amazing", "lowest": "WHAT", "average": "98", "is anomaly": "YES", "reason": "Linear Regerssion with B" },
    { "feature": "yaw", "highest": "ok", "lowest": "LOW", "average": "99", "is anomaly": "NO", "reason": "--" },
    //{ "feature": "pitch", "highest value": "sounds good"},
    //{ "feature": "roll", "highest value": "101.2"},
   // { "feature": "temp", "highest value": "Boxter"}
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

// NOT called if all the data changed by gridOptions.api.setRowData(x)
function tb_onRowSelected(event) {
    //event.node.data.feature  speed/roll ...
    //event.node.isSelected()  true/false
}


// change all data by
// var tb_x = [{ "feature": "speed", "some information": "WOW"}, ...]
// tb_gridOptions.api.setRowData(tb_x);

//average, highest value, lowest value, is anomaly, reason



function update_data(values_dictionary) {
    var tb_x = [];
    for (let key in values_dictionary) {
        var sum = 0;
        var high = values_dictionary[key][0];
        var low = values_dictionary[key][0];
        var tmp;
        for (let i = 0; i < values_dictionary[key].length; i++) {
            (high < values_dictionary[key][i]) ? (high = values_dictionary[key][i]) : tmp = high;
            (low > values_dictionary[key][i]) ? (low = values_dictionary[key][i]) : tmp = high;
            sum += values_dictionary[key][i];
        }
        var avg = sum / values_dictionary[key].length;

    }
}