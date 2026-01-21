$(document).ready(function(){

    function changeImage() {
        var expression = $('#pather').val();
        var scale = parseFloat($('#scale').val());
        var axisStep = parseFloat($('#axisStep').val());
        var url = '/chart/draw/' + btoa(expression);
        var params = [];

        if (isFinite(scale) && scale > 0) {
            params.push('scale=' + encodeURIComponent(scale));
        }

        if (isFinite(axisStep) && axisStep > 0) {
            params.push('step=' + encodeURIComponent(axisStep));
        }

        if (params.length > 0) {
            url += '?' + params.join('&');
        }

        $('#imgchart').prop('src', url);
    }

    $("#clicker").click(function () {
        changeImage();

    });

    $('#pather').on('keypress', function (e) {
        if (e.which == 13) {
            changeImage();

        }
    });

})
