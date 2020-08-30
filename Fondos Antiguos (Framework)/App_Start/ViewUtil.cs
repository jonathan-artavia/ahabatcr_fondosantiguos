using Fondos_Antiguos.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public static string ObtenerDescrOperacion(byte val)
        {
            return ComunResource.ResourceManager.GetString($"OperacionEnum.{val}");
        }
    }
}