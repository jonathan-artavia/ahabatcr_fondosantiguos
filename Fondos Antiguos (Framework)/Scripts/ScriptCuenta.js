function generarClave() {
    
    $.ajax({
        type: "POST",
        url: '/Cuenta/GenerarNuevaClave',
        data: {
            idUsuario: $("#IdUsuario").val(),
        },
        dataType: 'json',
        success: function (result) {
            $("#btnNuevaClave").attr("disabled", "true");
            $("#nuevaCtrña").val(result.newValue);
            alert(result.msg);
        }
    })
}