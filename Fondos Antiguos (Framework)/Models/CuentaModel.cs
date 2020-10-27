using Fondos_Antiguos.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fondos_Antiguos.Models
{
    public class CuentaModel
    {
        #region Constructors
        public CuentaModel()
        {
            this.Roles = new List<string>();
        }
        #endregion

        #region Properties
        [Required]
        [Display(ResourceType = typeof(CuentaResource), Name = nameof(Usuario))]
        public string Usuario { get; set; }
        [Required]
        [PasswordPropertyText]
        [Display(ResourceType = typeof(CuentaResource), Name = "Password")]
        public string NuevaContraseña { get; set; }

        public string IdUsuario { get; set; }
        public long Id { get; set; }

        [Display(ResourceType = typeof(CuentaResource), Name = nameof(FechaIngreso))]
        public DateTime? FechaIngreso { get; set; }

        public List<string> Roles { get; set; }

        public string RolSeleccionado { get; set; }
        public string RolIdSeleccionado { get; set; }
        #endregion
    }
}