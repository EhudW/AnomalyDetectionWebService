var data_dictionary = {};
var last_list = [];
var anomalies_dictionary = {};

// When new drop check if file is anomaly or model
function new_drop(input_dictionary, is_anomaly, is_hybrid) {
    cleanGraph();
    data_dictionary = input_dictionary;
    if (is_anomaly) {
        anomaly_path(data_dictionary);
        update_anomalies(data_dictionary, anomalies_dictionary["reason"]);
    }
    else {
        var x = model_path(data_dictionary, is_hybrid);
        anomalies_dictionary = {};
        update_data(data_dictionary);
        return x;
    }

};

// Send anomaly request to server, and refresh list 
function anomaly_path(input_dictionary) {
    var url = 'api/anomaly?model_id=' + getSelectedModelID();
    var data = { "predict_data": input_dictionary }
    anomalies_dictionary = send_to_server(url, 'POST', data);
    refresh_list_from_server();
}

// Send model request to server, and refresh list 
function model_path(input_dictionary, is_hybrid) {
    var url = 'api/model?model_type=';
    if (is_hybrid) {
        url = url + 'hybrid';
    } else {
        url = url + 'regression';
    }
    var data = { "train_data": input_dictionary }
    var x = send_to_server(url, 'POST', data);
    refresh_list_from_server();
    return x.model_id;
}

// Update the graph that a new attribute has been selected
function add_attr(name) {
    var array = data_dictionary[name];
    var anomalies = anomalies_dictionary["anomalies"];
    
    if ((anomalies != undefined) && (anomalies[name] != undefined) && (anomalies[name].length > 0)) {
        add_anomaly_attribute(name, array, anomalies[name]);
    } else {
        add_attribute(name, array);
    }

}

// Update the graph that a new attribute has been removed
function remove_attr(name) {
    var array = data_dictionary[name];
    remove_attribute(name, array);
}

// Send http request to server
function send_to_server(url, type, data_) {
    var data_JSON = JSON.stringify(data_);
    var resp = $.ajax({
        url: url,
        type: type,
        data: data_JSON,
        contentType: "text/json",
        processData: false,
        async: false
    }).done(function (rs, textStatus, xhr) {
        console.log(xhr.getResponseHeader('X-CUSTOM-HEADER'));
        console.log(xhr.status);
    });
    return JSON.parse(resp.responseText);
}


// Check if list comtain in server is updated
function refresh_list_from_server() {
    var new_list = send_to_server("api/models", "GET", "");
    if (same_lists(last_list, new_list))
        return;
    else {
        refresh(new_list);
        last_list = new_list;
    }
}

// Check if two list are identical
function same_lists(a, b) {
    if (a.length != b.length) return false;
    for (var i = 0; i < a.length; i++) {
        if (a[i].model_id != b[i].model_id) return false;
        if (a[i].upload_time != b[i].upload_time) return false;
        if (a[i].status != b[i].status) return false;
    }
    return true;
}

// Check that not change happen every 10 sec
setInterval(refresh_list_from_server, 10000);