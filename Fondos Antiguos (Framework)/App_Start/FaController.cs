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

        #region Properties
        public virtual IdentityDbContext<ApplicationUser> Store { get; set; }
        protected virtual ILogger<FaController> Logger => this._logger;
        #endregion
    }
}