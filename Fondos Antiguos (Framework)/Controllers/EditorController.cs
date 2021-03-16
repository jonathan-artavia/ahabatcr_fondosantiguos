using Fondos_Antiguos.DataServices;
using Fondos_Antiguos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fondos_Antiguos.Controllers
{
    [FaAuthorize]
    public class EditorController : FaController
    {
        #region Constructor
        public EditorController() : this(Models.FaApplicationDbContext.Create())
        {

        }
        public EditorController(Models.FaApplicationDbContext context) : base(context)
        {

        }
        #endregion

        #region Action Results
        // GET: Editor
        public ActionResult Inicio()
        {
            EditorDataService dataService = new EditorDataService();
            var model = dataService.ObtenerTexto("Home/Inicio", this.HttpContext) ?? new EditorEnunciadoModel() { Nombre = "Home/Inicio", UltimaModificacion = DateTime.Today };
            return View(model);
        }

        [HttpPost]
        public ActionResult Inicio(EditorEnunciadoModel modelo)
        {
            try
            {
                EditorDataService dataService = new EditorDataService();
                dataService.CrearOEditar(modelo, this.HttpContext);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("", ex);
                throw;
            }

            return View();
        }
        #endregion
    }
}