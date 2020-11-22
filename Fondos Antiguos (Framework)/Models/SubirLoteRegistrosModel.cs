using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fondos_Antiguos.Models
{
    public class SubirLoteRegistrosModel
    {
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Upload)]
        public HttpPostedFileBase Archivo { get; set; }

        [HiddenInput]
        public string ArchivoId { get; set; }
    }
}