using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Fondos_Antiguos
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "LocalizedIdentityErrorDescriberResources";
            DefaultModelBinder.ResourceClassKey = "LocalizedIdentityErrorDescriberResources";
        }

        protected void Application_EndRequest(Object sender,
                                           EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            //if (context.Response.Status.Substring(0, 3).Equals("401"))
            //{
            //    context.Response.ClearContent();
            //    context.Response.Write("<script language=\"javascript\">" +
            //                 "self.location='../login.aspx';</script>");
            //}
        }

    }
}
