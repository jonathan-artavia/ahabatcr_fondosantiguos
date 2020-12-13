using Fondos_Antiguos.DataService;
using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fondos_Antiguos.Controllers
{
    [FaAuthorize]
    public class LugaresController : FaController
    {
        #region Properties
        public LugaresDataService DataService { get; set; }
        #endregion

        #region Constructors
        public LugaresController() : this(FaApplicationDbContext.Create())
        {

        }
        public LugaresController(FaApplicationDbContext context) : base(context)
        {
            this.DataService = new LugaresDataService();
        }
        #endregion

        #region Actions
        [HttpGet]
        public virtual ActionResult Index()
        {
            Exception redirError = this.ObtenerErrorRedir();
            ResultadoListaSimpleModel<LugarModel> res = new ResultadoListaSimpleModel<LugarModel>(this.DataService.ObtenerLugares(null, null, this.HttpContext));
            if (redirError != null)
            {
                this.ModelState.AddModelError("Mensaje", redirError.Message);
                res.Mensaje = redirError.Message;
                res.TipoMensaje = 1;
            }
            else if (this.ObtenerMensajeRedir() is string _msj)
            {
                res.Mensaje = _msj;
                res.TipoMensaje = 0;
            }

            return View(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Crear(string nuevo_nombre)
        {
            try
            {
                this.DataService.Insertar(new LugarModel() { Nombre = nuevo_nombre }, this.HttpContext);
                this.AgregarMensajeSuccessParaRedir(CatalogoRes.RegistroMateriaAgregadoMsg);
            }
            catch (Exception ex)
            {
                this.AgregarErrorParaRedir(ex);
                this.View(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public virtual ActionResult Eliminar(long id)
        {
            try
            {
                this.DataService.Borrar(id, this.HttpContext);
                this.AgregarMensajeSuccessParaRedir(CatalogoRes.RegistroMateriaBorradoMsg);
            }
            catch (Exception ex)
            {
                this.AgregarErrorParaRedir(ex);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}