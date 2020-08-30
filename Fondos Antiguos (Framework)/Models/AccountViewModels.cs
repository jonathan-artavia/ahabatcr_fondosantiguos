using Fondos_Antiguos.Localization;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Fondos_Antiguos.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(ResourceType = typeof(CuentaResource), Name = "Usuario")]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(CuentaResource), Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(CuentaResource), Name = "AcordarseDeMi")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(ResourceType = typeof(CuentaResource), Name = "Usuario")]
        public string Usuario { get; set; }

        [StringLength(100, ErrorMessage = "{0} debe ser tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(CuentaResource), Name = "Password")]
        public string Password { get; set; }
        
        [Required]
        public string RolIdSeleccionado { get; set; }

        [ReadOnly(true)]
        [Display(ResourceType = typeof(CuentaResource), Name = nameof(RolSeleccionado))]
        public string RolSeleccionado { get; set; }

        public List<IdentityRole> Roles { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(ResourceType = typeof(CuentaResource), Name = "Usuario")]
        public string Usuario { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} debe ser tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(CuentaResource), Name = "ContraseñaConfirmacion")]
        [Compare("Password", ErrorMessageResourceName = "ContraNuevaNoCoinciden", ErrorMessageResourceType = typeof(CuentaResource))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
