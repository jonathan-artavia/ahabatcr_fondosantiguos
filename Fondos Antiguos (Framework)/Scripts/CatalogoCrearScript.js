$(document).ready(function () {
    $("#seriesModal").on('hidden.bs.modal', function () {
        if ($("#IdSerieDropDown").val() != '') {
            $("#SeriesNombre").val($("#IdSerieDropDown").text());
            $("#SeriesNombre").attr("readonly", "true");
            $("#IdSerie").val($("#IdSerieDropDown").val());
        } else {
            $("#SeriesNombre").val(null);
            $("#SeriesNombre").removeAttr("readonly");
            $("#IdSerie").val(null);
        }
    });
});