$(document).ready(function () {
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });

    createChart('generation-fitness-canvas', 'Generation fitness', window.generationFitnessChartData);
    createChart('chromosome-fitness-canvas', 'Chromosome fitness', window.chromosomeFitnessChartData);
    createChart('average-elapsed-time-canvas', 'Average elapsed time', window.averageElapsedTimeChartData);
    createChart('average-depth-canvas', 'Average depth', window.averageDepthChartData);
    createChart('average-nodes-count-canvas', 'Average nodes count', window.averageNodesChartData);
    createChart('average-time-per-game-canvas', 'Average time per game', window.averageTimePerGameChartData);

    function createChart(canvasId, title, data) {
        var config = {
            type: 'line',
            data: data,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                title: {
                    display: true,
                    text: title,
                    padding: 0
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
                        display: true
                    }],
                    yAxes: [{
                        display: true
                    }]
                }
            }
        };

        var ctx = document.getElementById(canvasId).getContext('2d');
        window.myLine = new Chart(ctx, config);
    }
});