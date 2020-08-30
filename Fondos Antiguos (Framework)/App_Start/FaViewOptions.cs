using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fondos_Antiguos
{
    public static class FaViewOptions
    {
        /// <summary>
        /// Cantidad de registros por pagina. Appsettings > RegistrosPorPagina.
        /// </summary>
        public static int RegistrosPorPagina { get; set; }

        /// <summary>
        /// Cantidad de  links de pagina al final de la tabla. Appsettings > CantidadPaginasMostradas.
        /// </summary>
        public static int CantidadPaginasMostradas { get; set; }

        /// <summary>
        /// Cantidad de caracteres visibles en Vista, para campos en especifico
        /// </summary>
        public static int TamanoStringsVista { get; set; }
    }
}