using Fondos_Antiguos.DataService;
using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Fondos_Antiguos
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class FaAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Divisor de multiples direcciones en un permiso
        /// </summary>
        private const string DIV_DIRECCIONES = ",";
        /// <summary>
        /// Divisor de multiples permisos
        /// </summary>
        private const string DIV_PERMISOS = "|";
        /// <summary>
        /// Divisor de herarquia de direccion URL
        /// </summary>
        private const string DIV_DIRECCION = "/";
        public FaAuthorizeAttribute():base()
        {

        }

        static FaAuthorizeAttribute()
        {
            Cache = new Cache();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            IsAuthorized(filterContext);
        }


        #region Static
        public static void IsAuthorized(AuthorizationContext filterContext)
        {
            string ctrl = null;
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.EndsWith("Controller"))
                ctrl = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Replace("Controller", string.Empty);
            else
                ctrl = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (!IsAuthorized(filterContext.HttpContext.User, ctrl, filterContext.ActionDescriptor.ActionName, filterContext.HttpContext))
                filterContext.Result = new HttpStatusCodeResult(401);
        }

        public static bool IsAuthorized(IPrincipal principal, string controller, string action, HttpContextBase context)
        {
            if (principal == null || string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action) || context == null)
                return false;
            if (controller.EndsWith("Controller"))
                controller = controller.Replace("Controller", string.Empty);
            ApplicationUserManager man = context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            RoleManager<IdentityRole> rolMan = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(FaApplicationDbContext.Create()));
            if (man != null)
            {
                ApplicationUser user = man.FindByNameAsync(principal.Identity.Name).Result;
                if (user == null)
                    return false;
                IdentityRole role = rolMan.FindById(user.Roles.FirstOrDefault().RoleId);
                CuentaDataService ds = new CuentaDataService(user, man, null);

                if (role.Name != "Admin")
                {
                    bool found = false;
                    List<IdentityRolPermit> views = ObtenerPermits(ds, user.Roles.FirstOrDefault().RoleId, context);
                    foreach (IdentityRolPermit item in views)
                    {
                        if (item.TodasLasVistas == 0)
                        {
                            string direcciones = ViewUtil.ObtenerDireccionDeView(item.ViewPath);

                            if (!direcciones.Contains(DIV_DIRECCIONES))
                            {
                                string[] vista = direcciones.Split(new string[] { DIV_DIRECCION }, StringSplitOptions.RemoveEmptyEntries);
                                if (vista[0] == controller && vista[1] == action)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            else
                            {
                                string[] vistas = direcciones.Split(new string[] { DIV_DIRECCIONES }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string vista in vistas)
                                {
                                    string[] div = vista.Split(new string[] { DIV_DIRECCION }, StringSplitOptions.RemoveEmptyEntries);
                                    if (div[0] == controller && div[1] == action)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (found)
                                    break;
                            }
                        }
                        else
                        {
                            if (item.TodasLasVistas == 2)
                            {
                                found = false;
                            }
                            else if (item.TodasLasVistas == 1)
                            {
                                found = true;
                            }
                            break;
                        }
                    }
                    if (!found)
                        return false;
                }
            }
            return true;
        }

        public static void RolActualizado(string idRol)
        {
            if (Cache.Get(idRol) != null)
                Cache.Remove(idRol);
        }

        private static List<IdentityRolPermit> ObtenerPermits(CuentaDataService ds, string idRol, HttpContextBase context)
        {
            List<IdentityRolPermit> res = (List<IdentityRolPermit>)Cache.Get(idRol);
            if (res == null)
            {
                res = ds.GetViewsPermitidas(idRol, context).Result;
                Cache.Add(idRol, res, null, Cache.NoAbsoluteExpiration, new TimeSpan(8, 0, 0), CacheItemPriority.Normal, null);
            }
            return res;
        }
        #endregion

        #region Properties
        private static Cache Cache { get; set; }
        #endregion
    }
}