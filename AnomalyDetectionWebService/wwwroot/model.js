function new_drop(input_dictionary, anomaly_model, is_hybrid) {
    var url;
    var data;
    console.log(${ anomaly_model} );
    if (anomaly_model >= 0) {
        url = '/api/anomaly?model_id=' + anomaly_model;
        data = {predict_data: input_dictionary }
    }
    else {
        url = '/api/model?model_type=';
        if (is_hybrid) {
            url = url + 'hybrid';
        } else {
            url = url + 'regression';
        }
        data = {train_data: input_dictionary }
    }
    var model_status = update_data(url, 'POST', data);
    // add_model_list(model_status);
};

function add_attr(name) {

}

function remove_attr(name) {

}

function update_data(url, type, data) {
    var resp = $.ajax({
        url: url,
        type: type,
        data: data,
        contentType: 'application/json',
        processData: false,
        dataType: 'json',
        async: false
    }).done(function (rs, textStatus, xhr) {
        console.log(xhr.getResponseHeader('X-CUSTOM-HEADER'));
        console.log(xhr.status);
    });
    return JSON.parse(resp.responseText);
}