//ml_ prefix to avoid collision with table
const ml_columnDefs = [
    { headerName: "id", field: "model_id", sortable: true },
    { headerName: "status", field: "status", sortable: true }, 
    { headerName: "upload", field: "upload_time", sortable: true }
];

// specify the data [example]
const ml_rowData = [
    { "model_id": 123, "status": "ready", "upload_time" : "2021-04-22T19:15:32+02.00" },
    { "model_id": 124, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00"},
    { "model_id": 125, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00"},
    { "model_id": 1267, "status": "pending", "upload_time": "2021-04-22T19:15:32+02.00"},
    { "model_id": 333, "status": "pending", "upload_time": "2021-04-22T19:15:32+02.00"},
    { "model_id": 456, "status": "pending", "upload_time": "2021-04-22T19:15:32+02.00"}
];

const set1 = [
    { "model_id": 123, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00" },
    { "model_id": 124, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00" },
    
];

const set2= [
    { "model_id": 331, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00" },
    { "model_id": 124, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00" }
];



// let the grid know which columns and what data to use
const ml_gridOptions = {
    columnDefs: ml_columnDefs,
    rowData: ml_rowData,
    rowSelection: 'single',
    rowMultiSelectWithClick: false,
    suppressRowDeselection: true,
    suppressRowClickSelection: false,
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
    var rmButton = document.getElementById("remove_button")
    if (getSelectedModelID() != undefined) {
        rmButton.value = "remove " + getSelectedModelID()
        rmButton.disabled = false
    }
    else {
        rmButton.disabled = true
    }
    //event.node.data.feature  speed/roll ...
    //event.node.isSelected()  true/false
}


function restart() {

}

function refresh(list) {
    var selected_model_id = getSelectedModelID()
    ml_gridOptions.api.setRowData(list)
    selectAfterRefresh(selected_model_id)
}


setInterval(() => {
    if (ml_gridOptions.api.getSelectedRows().length > 0)
        console.log(ml_gridOptions.api.getSelectedRows()[0].model_id)
}, 5000)

/*
setTimeout(() => {
    const ml_x = [
        { "model_id": 123, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00" },
        { "model_id": 128, "status": "ready", "upload_time": "2021-04-22T19:15:32+02.00" },
    ];
    ml_gridOptions.api.setRowData(ml_x)
}, 19000)
*/

function selectAfterRefresh(model_id) {
    document.getElementById("remove_button").disabled = true
    ml_gridOptions.api.forEachNode(function (node) {
        node.setSelected(node.data.model_id === model_id);
    });
}

function remove_selected(event) {
    if (getSelectedModelID() != undefined) {
        console.log("remove the " + getSelectedModelID())
    }
}

function getSelectedModelID() {
    if (ml_gridOptions.api.getSelectedRows()[0] != undefined)
        return ml_gridOptions.api.getSelectedRows()[0].model_id
    return undefined
}
// change all data by 
// var ml_x = [{ "feature": "speed", "some information": "WOW"}, ...]
// ml_gridOptions.api.setRowData(ml_x);
