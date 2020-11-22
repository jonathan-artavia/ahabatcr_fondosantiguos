using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fondos_Antiguos.Models
{
    public class SeleccionableCatalogoModel : CatalogoModel
    {
        [DefaultValue(false)]
        [Display(ResourceType = typeof(Localization.CatalogoRes), Name = "colBorrarSN")]
        public bool Seleccionado { get; set; }
    }
}