$(document).ready(function () {
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });

    createChart('generation-fitness-canvas', window.generationFitnessChartData);
    createChart('chromosome-fitness-canvas', window.chromosomeFitnessChartData);
    createChart('average-elapsed-time-canvas', window.averageElapsedTimeChartData);
    createChart('average-depth-canvas', window.generationFitnessChartData);
    createChart('average-nodes-count-canvas', window.generationFitnessChartData);
    createChart('average-nps-canvas', window.generationFitnessChartData);

    function createChart(name, data) {
        var config = {
            type: 'line',
            data: data,
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

        var ctx = document.getElementById(name).getContext('2d');
        window.myLine = new Chart(ctx, config);
    }
});