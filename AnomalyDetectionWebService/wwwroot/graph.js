// use prefix gr_ to avoid collision

//const labels = ["label1","label2"];

const labels = ["1","2","3","4","5","6","7"];
const data = {
    labels: labels,
    datasets: [{
        label: 'My First Dataset',
        data: [65, 59, 80, 81, 56, 55, 40],
        fill: false,
        borderColor: 'rgb(75, 192, 192)'
       // tension: 0.1
    },
        {
            label: 'My second Dataset',
            data: [85, 77, 88, 88, 59, 59, 20],
            fill: false,
            borderColor: 'rgb(85, 92, 92)'
            //tension: 0.1
        }
    ]
};
/*
const data = {
    //labels: labels,
    datasets: [
        {
            label: 'Dataset 1',
            data: [-1,-2,2,0,3],
            borderColor: 'rgba(255, 99, 132)',
        },
        {
            label: 'Dataset 2',
            data: [-1, -3, 5, 0, 30],
            borderColor: 'rgba(255, 99, 132)',
        }
    ]
};*/

const gr_config = {
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
                text: 'Chart.js Line Chart'
            }
        }
    }
};

var ctx = document.getElementById('myChart');
var myChart = new Chart(ctx, gr_config);

// to update
// gr_config.data = ....
/*myChart.update('none');//none for no animation 
 myChart.clear() to clear all
 */