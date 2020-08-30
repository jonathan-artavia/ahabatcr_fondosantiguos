using Fondos_Antiguos.DataService;
using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Owin.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Index(byte? incluirHist, long? pagina, byte? operacion, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            string error = null;
            string msg = null;
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

            QueryExpresion filter = null;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (operacion.HasValue)
            {
                filter = this.ObtenerExpressionDeFecha(incluirHist.GetValueOrDefault(0), operacion.Value, fechaDesde, fechaHasta, ref parameters);
                if (filter == null && !this.ModelState.IsValid)
                    return View();
            }

            IEnumerable<CatalogoModel> r = this.dataService.GetCatalogos(filter, parameters, pagina ?? 1, incluirHist.GetValueOrDefault(0), true, this.HttpContext);
            PagingResult<CatalogoModel> result = this.dataService.CrearPagingResult(filter, parameters, pagina ?? 1, incluirHist.GetValueOrDefault(0), this.HttpContext);
            result.SelectedPage = pagina.GetValueOrDefault(1);
            result.PagedResult = r;
            result.OrigenIncluido = incluirHist;
            result.Operacion = operacion;
            result.FechaInicial = fechaDesde;
            result.FechaFinal = fechaHasta;
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

        public ActionResult Ver(long id, byte origen)
        {
            var results = this.dataService.GetCatalogo(new QueryExpresion("AND", SqlUtil.Equals("ID", "@id", false)), new Dictionary<string, object>() { { "@id", id } }, origen == 1 ? (byte)0 : (byte)2, this.HttpContext);
            return this.View("VerView", results);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Editar(long id, byte origen)
        {
            var dataServiceSeries = new SeriesDataService();
            var seriesList = dataServiceSeries.GetSeries(null, null, this.HttpContext);
            ViewBag.SerieList = seriesList;
            var results = this.dataService.GetCatalogo(new QueryExpresion("AND", SqlUtil.Equals("ID", "@id", false)), new Dictionary<string, object>() { { "@id", id } }, origen == 1 ? (byte)0 : (byte)2, this.HttpContext);
            return this.View("Editar", results);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
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

        [Authorize]
        [HttpGet]
        public ActionResult Crear()
        {
            ViewBag.SerieList = new SeriesDataService().GetSeries(null, null, this.HttpContext);
            return View("Crear");
        }

        [Authorize]
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

        [Authorize(Roles = "Admin")]
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
        #endregion
    }
}