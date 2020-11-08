$(document).ready(function () {
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });

	var config = {
        type: 'line',
        data: {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            datasets: [{
                label: 'My First dataset',
                backgroundColor: '#4dc9f6',
                borderColor: '#4dc9f6',
                data: [
                    1, 2, 3, 4, 5
                ],
                fill: false,
            }, {
                label: 'My Second dataset',
                fill: false,
                backgroundColor: '#4dc9f6',
                borderColor: '#4dc9f6',
                data: [
                    2, 3, 4, 5, 6
                ],
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            title: {
                display: true,
                text: 'Chart.js Line Chart'
            },
            tooltips: {
                mode: 'index',
                intersect: false,
            },
            hover: {
                mode: 'nearest',
                intersect: true
            },
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Month'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Value'
                    }
                }]
            }
        }
    };

    createChart('generation-fitness-canvas', config);
    createChart('chromosome-fitness-canvas', config);
    createChart('average-elapsed-time-canvas', config);
    createChart('average-depth-canvas', config);
    createChart('average-nodes-count-canvas', config);
    createChart('average-nps-canvas', config);

    function createChart(name, config) {
        var ctx = document.getElementById(name).getContext('2d');
        window.myLine = new Chart(ctx, config);
    }
});