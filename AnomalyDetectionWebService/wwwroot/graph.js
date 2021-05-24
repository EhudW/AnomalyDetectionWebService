

var data = {
    labels: [],
    datasets: []
};

var gr_config = {
    type: 'line',
    data: data,
    options: {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'מאפיינים'
            }
        }
    }
};

var ctx = document.getElementById('myChart');
var myChart = new Chart(ctx, gr_config);
var dicForAmonaly = {}

function addLables(num) {
    for (var i = 0; i < num; i++) 
        myChart.data.labels.push(i.toString());
}

function add_attribute(name, array) {
    if (myChart.data.labels.length == 0) 
        addLables(array.length);
    
    adding_without_update(name, array)
    myChart.update()
}

function findIndexAttribute(name) {
    for (var i in myChart.data.datasets)
        if (myChart.data.datasets[i].label === name)
            return i;
    return undefined
}

function remove_attribute(name) {
    let x = findIndexAttribute(name)
    if ( x != undefined) {
        myChart.data.datasets.splice(x, 1)
        myChart.update()
    }
}

function adding_without_update(name, array) {
    let ranR = Math.floor(Math.random() * 200) + 1   // ranR can be any number from 1-201
    let ranB = Math.floor(Math.random() * 200) + 51 // ranB can be any number from 51-251
    let ranG = Math.floor(Math.random() * 200) + 51 // ranG can be any number from 51-251
    myChart.data.datasets.push({
        label: name,
        data: array,
        fill: false,
        pointRadius: 0 ,
        borderColor: 'rgb(' + ranR + ',' + ranG + ',' + ranB + ')'
        //tension: 0.1
    })
}

function inSpan(p, p1, span) {
    let b = false
    if (p >= span[0] && p < span[1] && p1 >= span[0] && p1 < span[1]) {

        b = true
    }
    return b;
}

function inSpanPoint(p, span) {
    let b = false
    if (p == span[0] || p == span[1] - 1) {
        b = true
    }
    return b;
}

const lineAnomaly = (ctx, spanList, v) => {
    for (var i in spanList)
        if (inSpan(ctx.p0.parsed.x, ctx.p1.parsed.x, spanList[i]))
            return v;
    return undefined;
};

const pointAnomaly = (ctx, spanList, v, v2) => {
    for (var i in spanList)
        if (inSpanPoint(ctx.dataIndex, spanList[i]))
            return v;
    return v2;
};

function adding_anomaly_without_update(name, array, span) {
    let ranR = Math.floor(Math.random() * 200) + 1   // ranR can be any number from 1-201
    let ranB = Math.floor(Math.random() * 200) + 51 // ranB can be any number from 51-251
    let ranG = Math.floor(Math.random() * 200) + 51 // ranG can be any number from 51-251
    let str = 'rgb(' + ranR + ',' + ranG + ',' + ranB + ')'
    myChart.data.datasets.push({
        label: name,
        data: array,
        fill: false,
        borderColor: str,
        pointRadius: ctx => pointAnomaly(ctx, span, 5, 0),
        //pointStyle: 'circle',
        pointStyle: ctx => pointAnomaly(ctx, span, 'crossRot', undefined),
        //pointBackgroundColor: '',
        pointBorderWidth: 3,
        pointBorderColor: ctx => pointAnomaly(ctx, span, 'red', str),
        segment: {
            borderColor: ctx => lineAnomaly(ctx, span, 'rgb(255,0,0)'),
        }
    })
}

function add_anomaly_attribute(name, array, span) {
    if (myChart.data.labels.length == 0)
        addLables(array.length);

    adding_anomaly_without_update(name, array, span)
    myChart.update();
}

function testGraph() {
    let d = {
        A: [1, 20, 3, 4, 5, 6],
        B: [10, 2, 3, 4, 5, 6],
        C: [1, 2, 3, 4, 5, 6],
        //  0  1  2  3  4   5   6   7
        D: [2, 4, 6, 8, 10, 12]
    };
    add_attribute("A", d.A);
    //add_attribute("B", d.B);
    add_anomaly_attribute("B", d.B, [[0, 2]]);
    
    remove_attribute("A");
    remove_attribute("B");
    add_attribute("C", d.C);

    add_anomaly_attribute("D", d.D, [[0, 2], [3, 6]]);
    
}

function testGraph2() {
    let x = { "A": [], "B": [] };
    let s = -1;
    for (var i = 0; i < 2000; i++) {
        if (i % 400 == 0) {
            s = s * -1;
        }
        x.A.push(2 * i * s);

        x.B.push(3 * i);

    }
    add_anomaly_attribute("B", x.B, [[2, 40], [500, 600]]);
    add_attribute("A", x.A);
}

function cleanGraph() {
    myChart.data.datasets = [];
    myChart.data.labels = [];
    myChart.update();
}

testGraph2()
// myChart.update('none');//none for no animation 
/**
 * 'circle'
 * 'cross'
 * 'crossRot'
*/