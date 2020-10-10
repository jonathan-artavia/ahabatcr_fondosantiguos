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
    var fechaOp = $('#OperacionFecha').val();
    var urlNueva = document.location.pathname + '?incluirHist=' + origen + '&pagina=' + numPagina;

    urlNueva = urlNueva + ArmarFiltro();

    document.location.assign(urlNueva);
}

function OrigenSeleccionado(numOrigen) {
    var pagina = $('#SelectedPage').val();
    var fechaOp = $('#OperacionFecha').val();

    var urlNueva = document.location.pathname + '?incluirHist=' + numOrigen + '&pagina=' + pagina;

    urlNueva = urlNueva + ArmarFiltro();

    document.location.assign(urlNueva);
}

function OperacionFechaSeleccionado() {
    var sel = $('#OperacionFecha').val();

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


function OperacionMateriaSeleccionado() {
    var sel = $('#OperacionMateria').val();
    
    if (sel !== null & sel !== '') {
        $('#divMateriaFiltro').show();
    } else {
        $('#divMateriaFiltro').hide();
    }
}

function OperacionSerieSeleccionado() {
    var sel = $('#OperacionSerie').val();
    if (sel !== null & sel !== '') {
        $('#divSerieFiltro').show();
    } else {
        $('#divSerieFiltro').hide();
    }
}

function OperacionLugarSeleccionado() {
    var sel = $('#OperacionLugar').val();
    if (sel !== null & sel !== '') {
        $('#divLugarFiltro').show();
    } else {
        $('#divLugarFiltro').hide();
    }
}

function OperacionCajaSeleccionado() {
    var sel = $('#OperacionCaja').val();
    if (sel !== null & sel !== '') {
        $('#divCajaFiltro').show();
    } else {
        $('#divCajaFiltro').hide();
    }
}

function ArmarFiltro() {
    var fechaOp = $('#OperacionFecha').val();
    var materiaOp = $('#OperacionMateria').val();
    var serieOp = $('#OperacionSerie').val();
    var lugarOp = $('#OperacionLugar').val();
    var cajaOp = $('#OperacionCaja').val();
    var urlNueva = "";

    if (fechaOp !== null & fechaOp !== '') {
        urlNueva += '&operacionFecha=' + fechaOp;
        var fechaDesde = $('#FechaInicial').val();
        urlNueva += '&fechaDesde=' + encodeURI(fechaDesde);
        if (fechaOp === '32' | fechaOp === '64') {
            var fechaHasta = $('#FechaFinal').val();
            urlNueva += '&fechaHasta=' + encodeURI(fechaHasta);
        }
    }

    if (materiaOp !== null & materiaOp !== '') {
        var filtroMateria = $('#FiltroMateria').val();
        urlNueva += '&operacionMateria=' + materiaOp;
        if (filtroMateria !== null & filtroMateria !== '') {
            urlNueva += '&filtroMateria=' + encodeURI(filtroMateria);
        }
    }

    if (serieOp !== null & serieOp !== '') {
        var filtroSerie = $("#IdSerieDropDown").val();
        urlNueva += '&operacionSerie=' + serieOp;
        if (filtroSerie !== null & filtroSerie !== '') {
            urlNueva += '&filtroSerie=' + encodeURI(filtroSerie);
        }
    }

    if (lugarOp !== null & lugarOp !== '') {
        var filtroLugar = $('#FiltroLugar').val();
        urlNueva += '&operacionLugar=' + lugarOp;
        if (filtroLugar !== null & filtroLugar !== '') {
            urlNueva += '&filtroLugar=' + encodeURI(filtroLugar);
        }
    }

    if (cajaOp !== null & cajaOp !== '') {
        var filtroCaja = $('#FiltroCaja').val();
        urlNueva += '&operacionCaja=' + cajaOp;
        if (filtroCaja !== null & filtroCaja !== '') {
            urlNueva += '&filtroCaja=' + encodeURI(filtroCaja);
        }
    }

    var filtroTexto = $('#TextoPlano').val();
    if (filtroTexto !== null & filtroTexto !== '') {
        urlNueva += '&filtroTexto=' + filtroTexto;
    }

    return urlNueva;
}

function AplicarFiltro() {
    var pagina = $('#SelectedPage').val();
    var origen = $('#OrigenIncluido').val();

    var urlNueva = document.location.pathname + '?incluirHist=' + origen + '&pagina=' + pagina;

    urlNueva = urlNueva + ArmarFiltro();

    document.location.assign(urlNueva);
}

function OnBtnBorrarClick(id, origen) {
    $('#BorrarRegistroID').val(id + '');
    $('#BorrarRegistroOrigen').val(origen + '');
}

function LimpiarFiltro() {
    $('#SelectedPage').val(null);
    $('#OperacionFecha').val('');
    $('#OperacionFecha').change();
    $('#OperacionMateria').val('');
    $('#OperacionMateria').change();
    $('#OperacionSerie').val('');
    $('#OperacionSerie').change();
    $('#OperacionLugar').val('');
    $('#OperacionLugar').change();
    $('#OperacionCaja').val('');
    $('#OperacionCaja').change();
    $('#TextoPlano').val('');
}