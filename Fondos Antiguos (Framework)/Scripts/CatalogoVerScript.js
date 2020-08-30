$(document).ready(function () {
    $('#tblCatalogo').dataTable({
        "paging": false,
        "searching": false,
        "ordering": false,
        "info" : false,
        "scrollY": 450,
        "oLanguage": {
            "sEmptyTable": "No hay registros"
        }
    });
    $('.dataTables_length').addClass('bs-select');
    $('.alert').alert();
});

function PaginaSeleccionada(numPagina) {
    var origen = $('#OrigenIncluido').val();
    var fechaOp = $('#Operacion').val();
    var urlNueva = document.location.pathname + '?incluirHist=' + origen + '&pagina=' + numPagina;

    if (fechaOp !== null & fechaOp !== '') {
        urlNueva += '&operacion=' + fechaOp;
        var fechaDesde = $('#FechaInicial').val();
        urlNueva += '&fechaDesde=' + encodeURI(fechaDesde);
        if (fechaOp === '32' | fechaOp === '64') {
            var fechaHasta = $('#FechaFinal').val();
            urlNueva += '&fechaHasta=' + encodeURI(fechaHasta);
        }
    }

    document.location.assign(urlNueva);
}

function OrigenSeleccionado(numOrigen) {
    var pagina = $('#SelectedPage').val();
    var fechaOp = $('#Operacion').val();

    var urlNueva = document.location.pathname + '?incluirHist=' + numOrigen + '&pagina=' + pagina;

    if (fechaOp !== null & fechaOp !== '') {
        urlNueva += '&operacion=' + fechaOp;
        var fechaDesde = $('#FechaInicial').val();
        urlNueva += '&fechaDesde=' + encodeURI(fechaDesde);
        if (sel === '32' | sel === '64') {    
            var fechaHasta = $('#FechaFinal').val();
            urlNueva += '&fechaHasta='+ encodeURI(fechaHasta);
        }
    }

    document.location.assign(urlNueva);
}

function OperacionFechaSeleccionado() {
    var sel = $('#Operacion').val();
    if (sel !== null & sel !== '') {
        $('#divFechaFiltro').show();
        if (sel === '32' | sel === '64') {
            $('#lblFechaDesde').show();
            $('#divFechaHasta').show();
        } else {
            $('#lblFechaDesde').hide();
            $('#divFechaHasta').hide();
        }
    }else {
        $('#divFechaFiltro').hide();
    }
}

function AplicarFiltro() {
    var pagina = $('#SelectedPage').val();
    var origen = $('#OrigenIncluido').val();
    var fechaOp = $('#Operacion').val();

    var urlNueva = document.location.pathname + '?incluirHist=' + origen + '&pagina=' + pagina;

    if (fechaOp !== null & fechaOp !== '') {
        urlNueva += '&operacion=' + fechaOp;
        var fechaDesde = $('#FechaInicial').val();
        urlNueva += '&fechaDesde=' + encodeURI(fechaDesde);
        if (fechaOp === '32' | fechaOp === '64') {
            var fechaHasta = $('#FechaFinal').val();
            urlNueva += '&fechaHasta=' + encodeURI(fechaHasta);
        }
    }

    document.location.assign(urlNueva);
}

function OnBtnBorrarClick(id, origen) {
    $('#BorrarRegistroID').val(id + '');
    $('#BorrarRegistroOrigen').val(origen + '');
}

function LimpiarFiltro() {
    $('#SelectedPage').val(null);
    $('#Operacion').val('');
    $('#FechaInicial').val(null);
    $('#FechaFinal').val(null);
}