using Fondos_Antiguos.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fondos_Antiguos.Models
{
    public class CambiarContraseñaModel : IValidatableObject
    {
        [Display(ResourceType = typeof(CuentaResource), Name = "ContraseñaNueva")]
        public string Nueva { get; set; }
        [Display(ResourceType = typeof(CuentaResource), Name = "ContraseñaConfirmacion")]
        public string Confirmacion { get; set; }

        public string IdUsuario { get; set; }

        public string Error { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (validationContext.MemberName)
            {
                case "Nueva":
                    yield return this.ValidateNueva();
                    break;
                case "Confirmacion":
                    yield return this.ValidateConfirmacion();
                    break;
            }
        }

        protected virtual ValidationResult ValidateNueva()
        {
            if (string.IsNullOrEmpty(this.Nueva))
                return new ValidationResult("La confirmación es requerida");

            return ValidationResult.Success;
        }

        protected virtual ValidationResult ValidateConfirmacion()
        {
            if (string.IsNullOrEmpty(this.Confirmacion))
                return new ValidationResult("La confirmación es requerida");

            if(!string.Equals(this.Nueva, this.Confirmacion))
            {
                return new ValidationResult("La contraseña y la confirmación no coninciden");
            }
            return ValidationResult.Success;
        }
    }
}
