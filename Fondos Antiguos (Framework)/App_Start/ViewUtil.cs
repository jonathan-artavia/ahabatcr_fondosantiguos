using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Fondos_Antiguos
{
    public static class ViewUtil
    {
        public static string ObtenerDescrOrigen(byte val)
        {
            switch (val)
            {
                case 0:
                    return CatalogoRes.IncluirHistVal0;
                case 1:
                    return CatalogoRes.IncluirHistVal1;
                case 2:
                    return CatalogoRes.IncluirHistVal2;
            }
            return string.Empty;
        }

        /// <summary>
        /// Obtiene la descripcion de la operacion
        /// </summary>
        /// <param name="val">ID de Operacion</param>
        /// <param name="filtro">Cual filtro. 0 = Fecha, 1 = Strings, 2 = Numeros</param>
        /// <returns></returns>
        public static string ObtenerDescrOperacion(byte val, byte? filtro = null)
        {
            switch (filtro)
            {
                case 1:
                    return ComunResource.ResourceManager.GetString($"OperacionEnum.String.{val}");
                default:
                    return ComunResource.ResourceManager.GetString($"OperacionEnum.{val}");
            }
        }

        public static string ObtenerCuentaCualesVistasPermite(List<IdentityRolPermit> permits)
        {
            if (permits != null)
            {
                if (permits.Any(x => x.TodasLasVistas == 1))
                {
                    return CuentaResource.ViewPermitir1;
                }
                else if (permits.Any(x => x.TodasLasVistas == 2))
                {
                    return CuentaResource.ViewPermitir2;
                }
            }
            return CuentaResource.ViewPermitir0;
        }

        public static string ObtenerDireccionDeView(string nombre)
        {
            return CuentaResource.Views.Split('|').FirstOrDefault(x => x.Split(':')[0].Equals(nombre))?.Split(':')[1];
        }

        public static string ObtenerNombreDeView(string direccion)
        {
            return CuentaResource.Views.Split('|').FirstOrDefault(x => x.Split(':')[1].Equals(direccion))?.Split(':')[0];
        }

        public static string ObtenerMensaje(string nombre, HttpContextBase context)
        {
            DataServices.EditorDataService ds = new DataServices.EditorDataService();
            var res = ds.ObtenerTexto(nombre, context);
            return res.Mensaje;
        }

        public static MvcHtmlString BotonEditorTexto<T>(this HtmlHelper<T> helper, string actionName)
        {
            if(FaAuthorizeAttribute.IsAuthorized(helper.ViewContext.HttpContext.User, helper.ViewContext.RouteData.Values["Controller"].ToString(), helper.ViewContext.RouteData.Values["Action"].ToString(), helper.ViewContext.HttpContext)
                && FaAuthorizeAttribute.IsAuthorized(helper.ViewContext.HttpContext.User, "Editor", actionName, helper.ViewContext.HttpContext))
                return helper.ActionLink(Localization.EditorResource.btnEditar, actionName, "Editor", null, new { @class = "btn btn-info small" });
            return MvcHtmlString.Empty;
        }
    }
}