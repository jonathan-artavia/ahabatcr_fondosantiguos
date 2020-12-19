using Fondos_Antiguos.Controllers;
using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fondos_Antiguos
{
    public class FaController : Controller
    {
        #region Fields
        private readonly ILogger<FaController> _logger;
        #endregion

        #region Constructor
        public FaController(FaApplicationDbContext context) : base()
        {
            //this._logger = logger;
            this.Store = context;
        }
        #endregion

        #region Overrides
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var item in context.ActionParameters)
            {
                if (item.Value is IValidatableObject _obj)
                {
                    List<ValidationResult> result = new List<ValidationResult>();
                    ValidationContext valContext = new ValidationContext(_obj);
                    foreach (var prop in _obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    {
                        valContext.MemberName = prop.Name;
                        Validator.TryValidateObject(_obj, valContext, result, true);
                        foreach (var res in result)
                        {
                            this.ModelState.AddModelError(prop.Name, res.ErrorMessage);
                        }
                    }
                }
            }

            if (this.Store is FaApplicationDbContext store && !string.IsNullOrEmpty(this.User.Identity.Name) && (!(context.Controller is CuentaController) || context.ActionDescriptor.ActionName != "CambiarContraseña" && (context.ActionDescriptor).ActionName != "Salir"))
            {
                string userId = this.User.Identity.GetUserId<string>();
                ApplicationUser user = store.Users.FirstOrDefault(x => x.Id.Equals(userId, StringComparison.InvariantCultureIgnoreCase));
                if (user != null && user.ReqCambioContraseña)
                {
                    context.Result = this.RedirectToAction("CambiarContraseña", "Cuenta");
                }
            }
            base.OnActionExecuting(context);
        }
        #endregion

        #region Metodos
        protected virtual void AgregarErrorParaRedir(Exception ex)
        {
            Exception nex = null;

            if (ex is MySql.Data.MySqlClient.MySqlException _sqlEx && (_sqlEx.Number == 1451 || _sqlEx.Message.Contains("foreign key constraint fails")))
                nex = new Exception(Localization.CatalogoRes.ErrorNoSePudoBorrarForeignKey);
            else
                nex = ex;
            if (this.TempData.ContainsKey("RedirError"))
                this.TempData.Remove("RedirError");
            this.TempData.Add("RedirError", nex);
        }

        protected virtual void AgregarMensajeSuccessParaRedir(string msj)
        {
            if (this.TempData.ContainsKey("RedirSucessMsj"))
                this.TempData.Remove("RedirSucessMsj");
            this.TempData.Add("RedirSucessMsj", msj);
        }

        protected virtual Exception ObtenerErrorRedir()
        {
            if (!this.TempData.ContainsKey("RedirError"))
                return null;
            string error = this.TempData["RedirError"].ToString();
            if (this.TempData["RedirError"] is Exception ex)
                error = ex.Message;
            this.TempData.Remove("RedirError");
            return new Exception(error);
        }

        protected virtual string ObtenerMensajeRedir()
        {
            if (!this.TempData.ContainsKey("RedirSucessMsj"))
                return null;
            string error = this.TempData["RedirSucessMsj"].ToString();
            this.TempData.Remove("RedirSucessMsj");
            return error;
        }
        #endregion

        #region Properties
        public virtual IdentityDbContext<ApplicationUser> Store { get; set; }
        protected virtual ILogger<FaController> Logger => this._logger;
        #endregion
    }
}