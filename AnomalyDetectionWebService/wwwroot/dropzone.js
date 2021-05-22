//var csv is the CSV file with headers
function csvJSON(csv) {

    csv = csv.replace("\r\n", "\n").replace("\r", "\n");
    var lines = csv.split("\n");
    var result = {};

    var tmp_headers = lines[0].split(",");
    //manage duplicate headers
    var counter = {};
    var current = {};
    for (var i = 0; i < tmp_headers.length; i++) {
        current[tmp_headers[i]] = 0;
        if (tmp_headers[i] in counter)
            counter[tmp_headers[i]]++;
        else
            counter[tmp_headers[i]] = 1;
    }
    var headers = [];
    for (var i = 0; i < tmp_headers.length; i++) {
        var suffix = "";
        if (counter[tmp_headers[i]] != 1) {
            suffix = "[" + current[tmp_headers[i]] + "]";
            current[tmp_headers[i]]++;
        }
        headers.push(tmp_headers[i] + suffix);
    }

    //initialize dictionary according to new headers
    for (var i = 0; i < headers.length; i++) {
        result[headers[i]] = [];
    }

    //add values to headers line by line
    for (var i = 1; i < lines.length; i++) {
        if (lines[i] == "")
            continue;
        var currentline = lines[i].split(",");
        for (var j = 0; j < headers.length; j++) {
            result[headers[j]].push(currentline[j]);
        }
    }

    //return the dictionary
    return result;
}

//this part manage the reading from the input file
async function parseFile(file) {
    var data = await new Response(file).text();
    return await csvJSON(data);
}

//receive file from drop and pass it to the model as a dictionary
async function dropHandler(event) {
    allowDrop(event);
    var is_hybrid = document.getElementById("hybrid").checked == true;
    var is_anomaly = document.getElementById("anomaly").checked == true;
    var drop = document.getElementById("The_File");
    drop.innerHTML = event.dataTransfer.items[0].getAsFile().name + " uploaded.<br/>" +
        (is_anomaly ? "anomaly data" : "create new model using<br/>") +
        (is_anomaly ? "" : (is_hybrid ? "hybrid " : "regerssion ") + "algorithem.");
    var input_dictionary = await parseFile(event.dataTransfer.items[0].getAsFile());
    //dropzone should know the model in order to  activate model.new_drop
    //model.new_drop(input_dictionary, (is_anomaly ? true : false), (is_hybrid ? true : false));
    //here we print the dictionary to console for testings
    console.log(input_dictionary);
    //update_data(input_dictionary);
    var myAnomaliesReasons = { "B": "Line Regression with C", "C": "Line Regression with B" };
    update_anomalies(input_dictionary, myAnomaliesReasons);
}