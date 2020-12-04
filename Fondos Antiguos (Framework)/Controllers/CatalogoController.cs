using ExcelDataReader;
using Fondos_Antiguos.DataService;
using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Owin.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Fondos_Antiguos.Controllers
{
    public class CatalogoController : FaController
    {
        #region Fields
        CatalogoDataService dataService;
        #endregion

        #region Constructors
        public CatalogoController(FaApplicationDbContext context) : base(context)
        {
            this.dataService = new CatalogoDataService();
        }
        public CatalogoController() : this(FaApplicationDbContext.Create())
        {

        }
        #endregion

        #region Actions
        [AllowAnonymous()]
        public ActionResult Index(byte? incluirHist, long? pagina, byte? operacionFecha, DateTime? fechaDesde, DateTime? fechaHasta,
            byte? operacionMateria, string filtroMateria, byte? operacionSerie, long? filtroSerie, byte? operacionLugar, string filtroLugar,
            byte? operacionCaja, string filtroCaja, string filtroTexto)
        {
            string error = null;
            string msg = null;

            if (!string.IsNullOrEmpty(filtroMateria) && filtroMateria.Equals("undefined", StringComparison.InvariantCultureIgnoreCase))
                filtroMateria = null;
            if (!string.IsNullOrEmpty(filtroLugar) && filtroLugar.Equals("undefined", StringComparison.InvariantCultureIgnoreCase))
                filtroLugar = null;
            if (!string.IsNullOrEmpty(filtroCaja) && filtroCaja.Equals("undefined", StringComparison.InvariantCultureIgnoreCase))
                filtroCaja = null;
            if (!string.IsNullOrEmpty(filtroTexto) && filtroTexto.Equals("undefined", StringComparison.InvariantCultureIgnoreCase))
                filtroTexto = null;

            if (incluirHist.HasValue)
            {
                if (!this.TempData.ContainsKey("includeHist"))
                    this.TempData.Add("includeHist", incluirHist.Value);
                else
                    this.TempData["includeHist"] = incluirHist.Value;
            }
            else
            {
                if (this.TempData.ContainsKey("includeHist") && (this.TempData.ContainsKey("guardoInsertoReg") && (bool)this.TempData["guardoInsertoReg"]))
                {
                    incluirHist = byte.Parse(this.TempData["includeHist"].ToString());
                    this.TempData.Remove("includeHist");
                    this.TempData.Remove("guardoInsertoReg");
                }
            }

            if (this.TempData.ContainsKey("Error") && (this.TempData.ContainsKey("guardoInsertoReg") && (bool)this.TempData["guardoInsertoReg"]))
            {
                error = this.TempData["Error"].ToString();
            }
            if (this.TempData.ContainsKey("Msg"))
            {
                msg = this.TempData["Msg"].ToString();
            }
            var seriesList = new SeriesDataService().ObtenerSeries(null, null, this.HttpContext);
            this.ViewBag.SerieList = seriesList;

            //iniciar armado de filtros
            QueryExpresion filter = null;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (operacionFecha.HasValue)
            {
                filter = this.ObtenerExpressionDeFecha(incluirHist.GetValueOrDefault(0), operacionFecha.Value, fechaDesde, fechaHasta, ref parameters);
            }
            if (operacionMateria.HasValue)
            {
                if (filter == null)
                    filter = this.ObtenerExpressionDeMaterias(incluirHist.GetValueOrDefault(0), operacionMateria.Value, filtroMateria, ref parameters);
                else
                {
                    QueryExpresion extracto = this.ObtenerExpressionDeMaterias(incluirHist.GetValueOrDefault(0), operacionMateria.Value, filtroMateria, ref parameters);
                    if (extracto != null)
                        filter.And(extracto.ToString());
                }
            }

            if (operacionSerie.HasValue)
            {
                if (filter == null)
                    filter = this.ObtenerExpressionDeSeries(incluirHist.GetValueOrDefault(0), operacionSerie.Value, filtroSerie, filtroSerie.HasValue ? seriesList.FirstOrDefault(x => x.ID == filtroSerie.Value).Nombre : string.Empty, ref parameters);
                else
                {
                    QueryExpresion extracto = this.ObtenerExpressionDeSeries(incluirHist.GetValueOrDefault(0), operacionSerie.Value, filtroSerie, filtroSerie.HasValue ? seriesList.FirstOrDefault(x => x.ID == filtroSerie.Value)?.Nombre : string.Empty, ref parameters);
                    if (extracto != null)
                        filter.And(extracto.ToString());
                }
            }

            if (operacionLugar.HasValue)
            {
                if (filter == null)
                    filter = this.ObtenerExpressionDeLugar(incluirHist.GetValueOrDefault(0), operacionLugar.Value, filtroLugar, ref parameters);
                else
                {
                    QueryExpresion extracto = this.ObtenerExpressionDeLugar(incluirHist.GetValueOrDefault(0), operacionLugar.Value, filtroLugar, ref parameters);
                    if (extracto != null)
                        filter.And(extracto.ToString());
                }
            }

            if (operacionCaja.HasValue)
            {
                if (filter == null)
                    filter = this.ObtenerExpressionDeCajas(incluirHist.GetValueOrDefault(0), operacionCaja.Value, filtroCaja, ref parameters);
                else
                {
                    QueryExpresion extracto = this.ObtenerExpressionDeCajas(incluirHist.GetValueOrDefault(0), operacionCaja.Value, filtroCaja, ref parameters);
                    if (extracto != null)
                        filter.And(extracto.ToString());
                }
            }

            if (!string.IsNullOrEmpty(filtroTexto))
            {
                if (filter == null)
                    filter = this.ObtenerExpressionDeTextoPlano(incluirHist.GetValueOrDefault(0), filtroTexto, ref parameters);
                else
                {
                    QueryExpresion extracto = this.ObtenerExpressionDeTextoPlano(incluirHist.GetValueOrDefault(0), filtroTexto, ref parameters);
                    if (extracto != null)
                        filter.And(extracto.ToString());
                }
            }

            if (filter == null && !this.ModelState.IsValid)
                return View();

            //fin de armado de filtros

            IEnumerable<CatalogoModel> r = this.dataService.GetCatalogos(filter, parameters, pagina ?? 1, incluirHist.GetValueOrDefault(0), true, this.HttpContext);
            CatalogoPaginadoModel<CatalogoModel> result = this.dataService.CrearModelosPaginados(filter, parameters, pagina ?? 1, incluirHist.GetValueOrDefault(0), this.HttpContext);
            result.SelectedPage = pagina.GetValueOrDefault(1);
            result.PagedResult = r;
            result.OrigenIncluido = incluirHist;
            result.OperacionFecha = operacionFecha;
            result.FechaInicial = fechaDesde;
            result.FechaFinal = fechaHasta;
            result.OperacionMateria = operacionMateria;
            result.FiltroMateria = filtroMateria;
            result.OperacionSerie = operacionSerie;
            result.FiltroSerie = filtroSerie;
            result.TextoPlano = filtroTexto;
            result.OperacionLugar = operacionLugar;
            result.FiltroLugar = filtroLugar;
            result.OperacionCaja = operacionCaja;
            result.FiltroCaja = filtroCaja;
            if (!string.IsNullOrEmpty(error))
            {
                result.Exception = error;
                result.Message = error;
                result.MessageType = 1;
            }
            else
            {
                result.Message = msg;
                result.MessageType = 0;
            }
            return this.View("Index", result);
        }

        [AllowAnonymous()]
        public ActionResult Ver(long id, byte origen)
        {
            try
            {
                var results = this.dataService.GetCatalogo(new QueryExpresion("AND", SqlUtil.Equals("ID", "@id", false)), new Dictionary<string, object>() { { "@id", id } }, origen == 1 ? (byte)0 : (byte)2, this.HttpContext);
                if (results == null)
                    throw new Exception(CatalogoRes.RegistroNoExisteError);
                return this.View(nameof(Ver), results);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("", ex.Message);
                return this.View();
            }
        }

        [FaAuthorize]
        public ActionResult Editar(long id, byte origen)
        {
            var dataServiceSeries = new SeriesDataService();
            var seriesList = dataServiceSeries.ObtenerSeries(null, null, this.HttpContext);
            try
            {
                var results = this.dataService.GetCatalogo(new QueryExpresion("AND", SqlUtil.Equals("ID", "@id", false)), new Dictionary<string, object>() { { "@id", id } }, origen == 1 ? (byte)0 : (byte)2, this.HttpContext);
                if (results == null)
                    throw new Exception(CatalogoRes.RegistroNoExisteError);
                ViewBag.SerieList = seriesList;
                return this.View(nameof(Editar), results);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("", ex.Message);
                return View(nameof(Editar), new CatalogoModel());
            }
        }

        [FaAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(CatalogoModel model)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    this.dataService.Actualizar(model, this.HttpContext);

                    if (!this.TempData.ContainsKey("guardoInsertoReg"))
                        this.TempData.Add("guardoInsertoReg", true);
                    else
                        this.TempData["guardoInsertoReg"] = true;
                }
                catch (Exception ex)
                {
                    model.Exception = ex.Message;
                    return View(model);
                    //throw ex;
                }
            }
            return this.RedirectToAction(nameof(Index));
        }

        [FaAuthorize]
        [HttpGet]
        public ActionResult Crear()
        {
            ViewBag.SerieList = new SeriesDataService().ObtenerSeries(null, null, this.HttpContext);
            return View("Crear");
        }

        [FaAuthorize]
        [HttpPost]
        public ActionResult Crear(CatalogoModel model)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    byte? includeHist = null;
                    this.dataService.Insertar(model, this.HttpContext);

                    if (!this.TempData.ContainsKey("guardoInsertoReg"))
                        this.TempData.Add("guardoInsertoReg", true);
                    else
                        this.TempData["guardoInsertoReg"] = true;

                    if (!this.TempData.ContainsKey("Msg"))
                        this.TempData.Add("Msg", CatalogoRes.RegistroAgregadoMsg);
                    else
                        this.TempData["Msg"] = CatalogoRes.RegistroAgregadoMsg;

                    if (this.TempData.ContainsKey("includeHist"))
                        return RedirectToAction(nameof(Index), new { includeHist });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (!this.TempData.ContainsKey("Error"))
                        this.TempData.Add("Error", ex.Message);
                    else
                        this.TempData["Error"] = ex.Message;

                    return this.RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return View(model);
            }
        }

        [FaAuthorize]
        public ActionResult Eliminar(long id, byte origen)
        {
            if (!this.TempData.ContainsKey("guardoInsertoReg"))
                this.TempData.Add("guardoInsertoReg", true);
            else
                this.TempData["guardoInsertoReg"] = true;
            try
            {
                this.dataService.Eliminar(id, origen, this.HttpContext);

                if (!this.TempData.ContainsKey("Msg"))
                    this.TempData.Add("Msg", CatalogoRes.RegistroBorradoMsg);
                else
                    this.TempData["Msg"] = CatalogoRes.RegistroBorradoMsg;

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!this.TempData.ContainsKey("Error"))
                    this.TempData.Add("Error", ex.Message);
                else
                    this.TempData["Error"] = ex.Message;
                return this.RedirectToAction(nameof(Index));
            }
        }

        #region Subir Registros Por Lote
        [FaAuthorize]
        [HttpGet]
        public ActionResult SubirLoteRegistros()
        {
            return View();
        }

        [FaAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubirLoteRegistros(SubirLoteRegistrosModel model)
        {
            if (this.ModelState.IsValid)
            {
                byte[] uploadedFile = new byte[model.Archivo.InputStream.Length];
                model.Archivo.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                string nombre = this.GuardarArchivoLoteTemporal(uploadedFile, model.Archivo.ContentType);
                
                return RedirectToAction(nameof(RevisionLoteRegistros), new { archivoId = nombre });
            }

            return View();
        }

        [FaAuthorize]
        [HttpGet]
        public async Task<ActionResult> RevisionLoteRegistros(string archivoId, int? pagina = 1)
        {
            IEnumerable<SeleccionableCatalogoModel> catalogos = this.dataService.GetCatalogoSinRevisar(archivoId, pagina, this.dataService.GetCantidadCatalogoSinRevisar(archivoId, this.HttpContext), this.HttpContext);
            RevisionCatalogoPaginadoModel res = null;
            if (catalogos == null || catalogos == default(SeleccionableCatalogoModel))
            {
                int cantidadTotalRegistros = 0;
                try
                {
                    catalogos = this.ConvertirLoteASeleccionable(archivoId);
                }
                catch (FileNotFoundException fnfEx)
                {
                    this.ModelState.AddModelError("", CatalogoRes.RevisionNoExisteMsg);
                    res = this.dataService.CrearModelosRevisionPaginados(archivoId, 1, 0, 0, this.HttpContext);
                    res.Registros = new List<SeleccionableCatalogoModel>();
                    return View(res);
                }
                catch (Exception)
                {
                    throw;
                }
                cantidadTotalRegistros = catalogos.Count();
                this.dataService.InsertarLote(archivoId, catalogos, false, this.HttpContext);
                catalogos = this.dataService.GetCatalogoSinRevisar(archivoId, pagina, cantidadTotalRegistros, this.HttpContext);

                res = this.dataService.CrearModelosRevisionPaginados(archivoId, pagina, catalogos?.Count() ?? 0, cantidadTotalRegistros, this.HttpContext);
                res.FechaVencimiento = DateTime.Now.AddDays(1);
                res.PagedResult = catalogos;
            }
            else
            {
                res = this.dataService.CrearModelosRevisionPaginados(archivoId, pagina, catalogos?.Count() ?? 0, this.dataService.GetCantidadCatalogoSinRevisar(archivoId, this.HttpContext), this.HttpContext);
                res.PagedResult = catalogos;
            }
            res.PaginaActual =
            res.SelectedPage = pagina.GetValueOrDefault(1);
            return View(res);
        }

        [FaAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RevisionLoteRegistros(RevisionCatalogoPaginadoModel res)
        {
            List<SeleccionableCatalogoModel> catalogosNuevos = (List<SeleccionableCatalogoModel>)this.dataService.GetCatalogoSinRevisar(res.ArchivoId, this.HttpContext);
            this.AplicarSeleccionAListaCatalogo(res.Registros, catalogosNuevos);

            this.dataService.ActualizarLote(res.ArchivoId, catalogosNuevos, this.HttpContext);
            return RedirectToAction(nameof(RevisionLoteRegistros), new { archivoId = res.ArchivoId, pagina = res.SelectedPage });
        }

        [FaAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PersistirRevisionLoteRegistros(RevisionCatalogoPaginadoModel res)
        {
            if (!res.DescartarSN)
            {
                List<SeleccionableCatalogoModel> catalogosNuevos = (List<SeleccionableCatalogoModel>)this.dataService.GetCatalogoSinRevisar(res.ArchivoId, this.HttpContext);
                this.AplicarSeleccionAListaCatalogo(res.Registros, catalogosNuevos);

                this.dataService.InsertarLote(res.ArchivoId, catalogosNuevos, true, this.HttpContext);
            }
            else
            {
                this.dataService.DescartarRevision(res.ArchivoId, this.HttpContext);
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
        #endregion

        #region Methods
        protected virtual QueryExpresion ObtenerExpressionDeFecha(byte includeHist, byte operacion, DateTime? valor1, DateTime? valor2, ref Dictionary<string, object> parameters)
        {
            QueryExpresion ExprLive(ref Dictionary<string, object> parametersIn)
            {
                if ((operacion == 32 || operacion == 64) && valor1.HasValue && valor2.HasValue)
                {
                    parametersIn.Add("@fechaDesde", valor1.Value.Date);
                    parametersIn.Add("@fechaHasta", valor2.Value.Date);
                    return new QueryExpresion(SqlUtil.AND, String.Format("{0} >= @fechaDesde", SqlUtil.SurroundColumn("Fecha")))
                        .And(String.Format("{0} <= @fechaHasta", SqlUtil.SurroundColumn("Fecha")));
                }
                else if (operacion < 32)
                {
                    string op = "";
                    switch (operacion)
                    {
                        case 0:
                            op = "!=";
                            break;
                        case 1:
                            op = "=";
                            break;
                        case 2:
                            op = ">";
                            break;
                        case 4:
                            op = "<";
                            break;
                        case 8:
                            op = ">=";
                            break;
                        case 16:
                            op = "<=";
                            break;
                        default:
                            op = "=";
                            break;
                    }
                    if (valor1.HasValue)
                    {
                        parametersIn.Add("@fechaDesde", valor1.Value.Date);
                        return new QueryExpresion(SqlUtil.AND, String.Format("{0} {1} @fechaDesde", SqlUtil.SurroundColumn("Fecha"), op));
                    }
                    else
                    {
                        if (operacion == 0)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NOT NULL", SqlUtil.SurroundColumn("Fecha")));
                        else if (operacion == 1)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NULL", SqlUtil.SurroundColumn("Fecha")));
                        this.ModelState.AddModelError("FechaInicial", "Busqueda incorrecta. La Fecha Inicial en vacio solo se permite en operaciones de 'Igual A' y 'Diferente A'.");
                        return null;
                    }
                }
                else
                {
                    this.ModelState.AddModelError("FechaInicial", "Busqueda incorrecta. Intente de nuevo.");
                    return null;
                }
            }

            QueryExpresion ExprHist(ref Dictionary<string, object> parametersIn)
            {
                if ((operacion == 32 || operacion == 64) && valor1.HasValue && valor2.HasValue)
                {
                    parametersIn.Add("@fechaCodDesde", FormatFechaCod(valor1.Value.Date));
                    parametersIn.Add("@fechaCodHasta", FormatFechaCod(valor2.Value.Date));
                    return new QueryExpresion(SqlUtil.AND, String.Format("{0} >= @fechaCodDesde", SqlUtil.SurroundColumn("FechaCod")))
                            .And(String.Format("{0} <= @fechaCodHasta", SqlUtil.SurroundColumn("FechaCod")));
                }
                else if (operacion < 32)
                {
                    string op = "";
                    switch (operacion)
                    {
                        case 0:
                            op = "!=";
                            break;
                        case 1:
                            op = "=";
                            break;
                        case 2:
                            op = ">";
                            break;
                        case 4:
                            op = "<";
                            break;
                        case 8:
                            op = ">=";
                            break;
                        case 16:
                            op = "<=";
                            break;
                        default:
                            op = "=";
                            break;
                    }
                    if (valor1.HasValue)
                    {
                        parametersIn.Add("@fechaCodDesde", FormatFechaCod(valor1.Value.Date));
                        return new QueryExpresion(SqlUtil.AND, String.Format("{0} {1} @fechaCodDesde", SqlUtil.SurroundColumn("FechaCod"), op));
                    }
                    else
                    {
                        if (operacion == 0)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NOT NULL", SqlUtil.SurroundColumn("FechaCod")))
                                .And(string.Format("{0} > 0", SqlUtil.SurroundColumn("FechaCod")));
                        else if (operacion == 1)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NULL", SqlUtil.SurroundColumn("FechaCod")))
                                .Or(string.Format("{0} <= 0", SqlUtil.SurroundColumn("FechaCod")));
                        this.ModelState.AddModelError("FechaInicial", "Busqueda incorrecta. La Fecha Inicial en vacio solo se permite en operaciones de 'Igual A' y 'Diferente A'.");
                        return null;
                    }
                }
                else
                {
                    this.ModelState.AddModelError("FechaInicial", "Busqueda incorrecta. Intente de nuevo.");
                    return null;
                }
            }

            string FormatFechaCod(DateTime fecha)
            {
                return string.Format("{0:D4}{1:D2}{2:D2}", fecha.Date.Year, fecha.Date.Month, fecha.Date.Day);
            }

            if (parameters == null)
                parameters = new Dictionary<string, object>();
            switch (includeHist)
            {
                case 0: //only live
                    return ExprLive(ref parameters);
                case 1: //both
                    QueryExpresion e1 = ExprLive(ref parameters);
                    QueryExpresion e2 = ExprHist(ref parameters);
                    if (e1 != null && e2 != null)
                        return new QueryExpresion($"({e1} {SqlUtil.OR} {e2})");
                    else if (e1 != null && e2 == null)
                        return e1;
                    else if (e1 == null && e2 != null)
                        return e2;
                    return null;
                case 2: //only hist
                    return ExprHist(ref parameters);
            }
            return null;
        }

        protected virtual QueryExpresion ObtenerExpressionDeMaterias(byte includeHist, byte operacion, string valor, ref Dictionary<string, object> parameters)
        {
            QueryExpresion ExprLive(string filtro, int filtroCount, ref Dictionary<string, object> parametersIn)
            {
                string nombreParam = $"@filtroMateria{filtroCount}";
                if (operacion == 64 && !string.IsNullOrEmpty(filtro))
                {
                    parametersIn.Add(nombreParam, "%" + filtro + "%");
                    return new QueryExpresion(String.Format("{0} like {1}", SqlUtil.SurroundColumn("Materias"), nombreParam));
                }
                else if (operacion == 32 && !string.IsNullOrEmpty(filtro))
                {
                    parametersIn.Add(nombreParam, "%" + filtro + "%");
                    return new QueryExpresion(String.Format("{0} not like {1}", SqlUtil.SurroundColumn("Materias"), nombreParam));
                }
                else if (operacion < 32)
                {
                    string op = "";
                    switch (operacion)
                    {
                        case 0:
                            op = "!=";
                            break;
                        default:
                            op = "=";
                            break;
                    }
                    if (!String.IsNullOrEmpty(filtro))
                    {
                        parametersIn.Add(nombreParam, filtro);
                        return new QueryExpresion(SqlUtil.AND, String.Format("{0} {1} {2}", SqlUtil.SurroundColumn("Materias"), op, nombreParam));
                    }
                    else
                    {
                        if (operacion == 0)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NOT NULL", SqlUtil.SurroundColumn("Materias")));
                        else if (operacion == 1)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NULL", SqlUtil.SurroundColumn("Materias")));
                        this.ModelState.AddModelError("FiltroMateria", "Busqueda incorrecta. El Filtro de Materia en vacio solo se permite en operaciones de 'Igual A' y 'Diferente A'.");
                        return null;
                    }
                }
                else
                {
                    this.ModelState.AddModelError("FiltroMateria", "Busqueda incorrecta. Intente de nuevo.");
                    return null;
                }
            }

            string[] sepMaterias = valor != null ? valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            if (sepMaterias.Length > 1)
            {
                QueryExpresion res = null;
                int filterCount = 0;
                foreach (string i in sepMaterias)
                {
                    if (res == null)
                        res = ExprLive(i, filterCount, ref parameters);
                    else
                    {
                        if (((OperacionEnum)operacion) == OperacionEnum.DiferenteA || ((OperacionEnum)operacion) == OperacionEnum.EntreExc)
                            res.And(ExprLive(i, filterCount, ref parameters)?.ToString());
                        else
                            res.Or(ExprLive(i, filterCount, ref parameters)?.ToString());
                    }
                    filterCount++;
                }
                return new QueryExpresion($"({res})");
            }
            else
            {
                QueryExpresion exp = ExprLive(valor, 0, ref parameters);
                if (exp != null)
                    return new QueryExpresion($"({exp})");
                return null;
            }
        }

        protected virtual QueryExpresion ObtenerExpressionDeSeries(byte includeHist, byte operacion, long? serieId, string serieNombre, ref Dictionary<string, object> parameters)
        {
            QueryExpresion ExprLive(ref Dictionary<string, object> parametersIn)
            {
                if ((operacion == 32 || operacion == 64) && !string.IsNullOrEmpty(serieNombre))
                {
                    parametersIn.Add("@filtroSeries", serieNombre);
                    return new QueryExpresion(String.Format("{0} like @filtroSeries", SqlUtil.SurroundColumn("IdSerie")));
                }
                else if (operacion < 32)
                {
                    string op = "";
                    switch (operacion)
                    {
                        case 0:
                            op = "!=";
                            break;
                        default:
                            op = "=";
                            break;
                    }
                    if (serieId.HasValue)
                    {
                        parametersIn.Add("@filtroSeries", serieId);
                        return new QueryExpresion(SqlUtil.AND, String.Format("{0} {1} @filtroSeries", SqlUtil.SurroundColumn("IdSerie"), op));
                    }
                    else
                    {
                        if (operacion == 0)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NOT NULL", SqlUtil.SurroundColumn("IdSerie")));
                        else if (operacion == 1)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NULL", SqlUtil.SurroundColumn("IdSerie")));
                        this.ModelState.AddModelError("FiltroSeries", "Busqueda incorrecta. El Filtro de Series en vacio solo se permite en operaciones de 'Igual A' y 'Diferente A'.");
                        return null;
                    }
                }
                else
                {
                    this.ModelState.AddModelError("FiltroSeries", "Busqueda incorrecta. Intente de nuevo.");
                    return null;
                }
            }

            if (parameters == null)
                parameters = new Dictionary<string, object>();
            if (includeHist == 2) //only hist, no hay Series en Historico
                return null;

            return ExprLive(ref parameters);
        }

        protected virtual QueryExpresion ObtenerExpressionDeLugar(byte includeHist, byte operacion, string valor, ref Dictionary<string, object> parameters)
        {
            QueryExpresion ExprLive(string filtro, int numFiltro, ref Dictionary<string, object> parametersIn)
            {
                string nombreParam = $"@filtroLugar{numFiltro}";
                if (operacion == 64 && !string.IsNullOrEmpty(filtro))
                {
                    parametersIn.Add(nombreParam, "%" + filtro + "%");
                    return new QueryExpresion(String.Format("{0} like {1}", SqlUtil.SurroundColumn("Lugar"), nombreParam));
                }
                else if (operacion == 32 && !string.IsNullOrEmpty(filtro))
                {
                    parametersIn.Add(nombreParam, "%" + filtro + "%");
                    return new QueryExpresion(String.Format("{0} not like {1}", SqlUtil.SurroundColumn("Lugar"), nombreParam));
                }
                else if (operacion < 32)
                {
                    string op = "";
                    switch (operacion)
                    {
                        case 0:
                            op = "!=";
                            break;
                        default:
                            op = "=";
                            break;
                    }
                    if (!String.IsNullOrEmpty(filtro))
                    {
                        parametersIn.Add(nombreParam, filtro);
                        return new QueryExpresion(SqlUtil.AND, String.Format("{0} {1} {2}", SqlUtil.SurroundColumn("Lugar"), op, nombreParam));
                    }
                    else
                    {
                        if (operacion == 0)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NOT NULL", SqlUtil.SurroundColumn("Lugar")));
                        else if (operacion == 1)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NULL", SqlUtil.SurroundColumn("Lugar")));
                        this.ModelState.AddModelError("FiltroLugar", "Busqueda incorrecta. El Filtro de Lugar en vacio solo se permite en operaciones de 'Igual A' y 'Diferente A'.");
                        return null;
                    }
                }
                else
                {
                    this.ModelState.AddModelError("FiltroLugar", "Busqueda incorrecta. Intente de nuevo.");
                    return null;
                }
            }
            string[] sepLugares = valor != null ? valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            if (sepLugares.Length > 1)
            {
                QueryExpresion res = null;
                int filterCount = 0;
                foreach (string i in sepLugares)
                {
                    if (res == null)
                        res = ExprLive(i, filterCount, ref parameters);
                    else
                    {
                        if (((OperacionEnum)operacion) == OperacionEnum.DiferenteA || ((OperacionEnum)operacion) == OperacionEnum.EntreExc)
                            res.And(ExprLive(i, filterCount, ref parameters)?.ToString());
                        else
                            res.Or(ExprLive(i, filterCount, ref parameters)?.ToString());
                    }
                    filterCount++;
                }
                return new QueryExpresion($"({res})");
            }
            else
            {
                QueryExpresion exp = ExprLive(valor, 0, ref parameters);
                if (exp != null)
                    return new QueryExpresion($"({exp})");
                return null;
            }

        }

        protected virtual QueryExpresion ObtenerExpressionDeCajas(byte includeHist, byte operacion, string valor, ref Dictionary<string, object> parameters)
        {
            QueryExpresion ExprLive(string val, int count, ref Dictionary<string, object> parametersIn)
            {
                string varName = "@filtroCaja" + count.ToString();
                if (operacion == 64 && !string.IsNullOrEmpty(val))
                {
                    if (!parametersIn.ContainsKey(varName))
                        parametersIn.Add(varName, "%" + val + "%");
                    return new QueryExpresion(String.Format("{0} LIKE {1}", SqlUtil.SurroundColumn("NumCaja"), varName));
                }
                else if (operacion == 32 && !string.IsNullOrEmpty(val))
                {
                    if (!parametersIn.ContainsKey(varName))
                        parametersIn.Add(varName, "%" + val + "%");
                    return new QueryExpresion(String.Format("{0} NOT LIKE {1}", SqlUtil.SurroundColumn("NumCaja"), varName));
                }
                else if (operacion < 32)
                {
                    string op = "";
                    switch (operacion)
                    {
                        case 0:
                            op = "!=";
                            break;
                        default:
                            op = "=";
                            break;
                    }
                    if (!String.IsNullOrEmpty(val))
                    {
                        parametersIn.Add(varName, val);
                        return new QueryExpresion(SqlUtil.AND, String.Format("{0} {1} {2}", SqlUtil.SurroundColumn("NumCaja"), op, varName));
                    }
                    else
                    {
                        if (operacion == 0)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NOT NULL", SqlUtil.SurroundColumn("NumCaja")));
                        else if (operacion == 1)
                            return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NULL", SqlUtil.SurroundColumn("NumCaja")));
                        this.ModelState.AddModelError("FiltroCaja", "Busqueda incorrecta. El Filtro de Caja en vacio solo se permite en operaciones de 'Igual A' y 'Diferente A'.");
                        return null;
                    }
                }
                else
                {
                    this.ModelState.AddModelError("FiltroCaja", "Busqueda incorrecta. Intente de nuevo.");
                    return null;
                }
            }

            QueryExpresion ExprHist(string val, int count, ref Dictionary<string, object> parametersIn) //Solo se usa LIKE como operador, porque Fichero puede tener cualquier cosa, no solo Caja
            {
                string varName = "@filtroHistCaja" + count.ToString();
                if (!String.IsNullOrEmpty(val))
                {
                    if (!parametersIn.ContainsKey(varName))
                        parametersIn.Add(varName, "%" + val + "%");
                    return new QueryExpresion(SqlUtil.AND, String.Format("{0} LIKE {1}", SqlUtil.SurroundColumn("Fichero"), varName));
                }
                else
                {
                    if (operacion == 0)
                        return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NOT NULL", SqlUtil.SurroundColumn("Fichero")));
                    else if (operacion == 1)
                        return new QueryExpresion(SqlUtil.AND, string.Format("{0} IS NULL", SqlUtil.SurroundColumn("Fichero")));
                    this.ModelState.AddModelError("FiltroCaja", "Busqueda incorrecta. El Filtro de Caja en vacio solo se permite en operaciones de 'Igual A' y 'Diferente A'.");
                    return null;
                }
            }

            if (parameters == null)
                parameters = new Dictionary<string, object>();

            string[] sepCajas = valor != null ? valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
            if (sepCajas.Length > 1)
            {
                QueryExpresion res = null;
                int filterCount = 0;
                foreach (string i in sepCajas)
                {
                    if (res == null)
                    {
                        switch (includeHist)
                        {
                            case 0: //only live
                                res = ExprLive(i, filterCount, ref parameters);
                                break;
                            case 1: //both
                                QueryExpresion e1 = ExprLive(i, filterCount, ref parameters);
                                QueryExpresion e2 = ExprHist(i, filterCount, ref parameters);
                                if (e1 != null && e2 != null)
                                    res = new QueryExpresion("(" + e1 + ")")
                                        .Or("(" + e2 + ")");
                                else if (e1 != null && e2 == null)
                                    res = e1;
                                else if (e1 == null && e2 != null)
                                    res = e2;
                                break;
                            case 2: //only hist
                                res = ExprHist(i, filterCount, ref parameters);
                                break;
                        }
                    }
                    else
                    {
                        switch (includeHist)
                        {
                            case 0: //only live
                                if (((OperacionEnum)operacion) == OperacionEnum.DiferenteA)
                                    res.And(ExprLive(i, filterCount, ref parameters)?.ToString());
                                else
                                    res.Or(ExprLive(i, filterCount, ref parameters)?.ToString());
                                break;
                            case 1: //both
                                QueryExpresion e1 = ExprLive(i, filterCount, ref parameters);
                                QueryExpresion e2 = ExprHist(i, filterCount, ref parameters);
                                if (e1 != null && e2 != null)
                                    return new QueryExpresion("(" + e1 + ")")
                                        .Or("(" + e2 + ")");
                                else if (e1 != null && e2 == null)
                                {
                                    if (((OperacionEnum)operacion) == OperacionEnum.DiferenteA || ((OperacionEnum)operacion) == OperacionEnum.DiferenteA)
                                        res.And(e1.ToString());
                                    else
                                        res.Or(e1.ToString());
                                }
                                else if (e1 == null && e2 != null)
                                {
                                    if (((OperacionEnum)operacion) == OperacionEnum.DiferenteA || ((OperacionEnum)operacion) == OperacionEnum.DiferenteA)
                                        res.And(e2.ToString());
                                    else
                                        res.Or(e2.ToString());
                                }
                                break;
                            case 2: //only hist
                                if (((OperacionEnum)operacion) == OperacionEnum.DiferenteA)
                                    res.And(ExprHist(i, filterCount, ref parameters)?.ToString());
                                else
                                    res.Or(ExprHist(i, filterCount, ref parameters)?.ToString());
                                break;
                        }
                    }
                    filterCount++;
                }
                if (res != null && !string.IsNullOrEmpty(res.Expresion))
                    return new QueryExpresion($"({res})");
                return null;
            }
            else
            {
                switch (includeHist)
                {
                    case 0: //only live
                        QueryExpresion expL = ExprLive(valor, 0, ref parameters);
                        if (expL != null)
                            return new QueryExpresion($"({expL})");
                        return null;
                    case 1: //both
                        QueryExpresion e1 = ExprLive(valor, 0, ref parameters);
                        QueryExpresion e2 = ExprHist(valor, 0, ref parameters);
                        if (e1 != null && e2 != null)
                            return new QueryExpresion("(" + (new QueryExpresion("(" + e1 + ")")
                                .Or("(" + e2 + ")").ToString() + ")"));
                        else if (e1 != null && e2 == null)
                            return new QueryExpresion($"({e1})");
                        else if (e1 == null && e2 != null)
                            return new QueryExpresion($"({e2})");
                        return null;
                    case 2: //only hist
                        QueryExpresion expH = ExprHist(valor, 0, ref parameters);
                        if (expH != null)
                            return new QueryExpresion($"({expH})");
                        return null;
                    default:
                        return null;
                }
            }
        }

        protected virtual QueryExpresion ObtenerExpressionDeTextoPlano(byte includeHist, string valor, ref Dictionary<string, object> parameters)
        {
            QueryExpresion ExprLive(ref Dictionary<string, object> parametersIn)
            {
                if (!string.IsNullOrEmpty(valor))
                {
                    if (!parametersIn.ContainsKey("@filtroTexto"))
                        parametersIn.Add("@filtroTexto", "%" + valor + "%");
                    return new QueryExpresion(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Lugar")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Contenido")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Observaciones")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Libro")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("NumExpediente")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("NumCarpeta")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Folio")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Fichero")));
                }
                return null;
            }

            QueryExpresion ExprHist(ref Dictionary<string, object> parametersIn)
            {
                if (!string.IsNullOrEmpty(valor))
                {
                    if (!parametersIn.ContainsKey("@filtroTexto"))
                        parametersIn.Add("@filtroTexto", "%" + valor + "%");
                    return new QueryExpresion(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Lugar")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Contenido"))) //Descripcion
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("FechaOrig")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Signatura")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Observaciones"))) //datos
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Fichero")))
                        .Or(String.Format("{0} like @filtroTexto", SqlUtil.SurroundColumn("Materias")));
                }
                return null;
            }

            if (parameters == null)
                parameters = new Dictionary<string, object>();

            switch (includeHist)
            {
                case 0: //only live
                    return ExprLive(ref parameters);
                case 1: //both
                    QueryExpresion e1 = ExprLive(ref parameters);
                    QueryExpresion e2 = ExprHist(ref parameters);
                    if (e1 != null && e2 != null)
                        return new QueryExpresion("(" + e1 + ")")
                            .Or("(" + e2 + ")");
                    else if (e1 != null && e2 == null)
                        return e1;
                    else if (e1 == null && e2 != null)
                        return e2;
                    return null;
                case 2: //only hist
                    return ExprHist(ref parameters);
            }

            return null;
        }

        protected virtual string GuardarArchivoLoteTemporal(byte[] contenido, string formato)
        {
            string soloDireccion = this.Server.MapPath("~/Content/Subidas/");
            string soloNombreArchivo = Guid.NewGuid().ToString();

            System.IO.File.WriteAllBytes(Path.Combine(soloDireccion, soloNombreArchivo), contenido);

            return soloNombreArchivo;
        }

        protected virtual IEnumerable<SeleccionableCatalogoModel> ConvertirLoteASeleccionable(string archivoId)
        {
            IEnumerable<SeleccionableCatalogoModel> models = null;
            string direccionCompletaArchivo = Path.Combine(this.Server.MapPath("~/Content/Subidas/"), archivoId);
            using (FileStream fileStream = new FileStream(direccionCompletaArchivo, FileMode.Open))
            using (IExcelDataReader reader = ExcelDataReader.ExcelReaderFactory.CreateReader(fileStream))
            {
                models = this.dataService.ConvertirLoteDataReader<SeleccionableCatalogoModel>(reader, HttpContext);
            }
            System.IO.File.Delete(direccionCompletaArchivo);
            return models;
        }

        protected virtual void AplicarSeleccionAListaCatalogo(List<SeleccionableCatalogoModel> seleccionados, List<SeleccionableCatalogoModel> modelosExsitentes)
        {
            foreach (SeleccionableCatalogoModel item in seleccionados)
            {
                SeleccionableCatalogoModel m = modelosExsitentes.Find(x => x.ID == item.ID);
                if (m != null)
                    m.Seleccionado = item.Seleccionado;
            }
        }
        #endregion
    }
}