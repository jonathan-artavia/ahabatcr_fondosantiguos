$(document).ready(function () {
    $("#rolModal").on('hidden.bs.modal', function () {
        if ($("#IdRolDropDown").val() != '') {
            $("#RolSeleccionado").val($("#IdRolDropDown").children("option:selected").text());
            $("#RolIdSeleccionado").val($("#IdRolDropDown").val());
        } else {
            $("#RolSeleccionado").val(null);
            $("#RolIdSeleccionado").val(null);
        }
    });
});