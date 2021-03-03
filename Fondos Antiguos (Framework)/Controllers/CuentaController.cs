using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Fondos_Antiguos.Models;
using Fondos_Antiguos.DataService;
using Fondos_Antiguos.Localization;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Fondos_Antiguos.Controllers
{
    public class CuentaController : Controller
    {
        #region Fields
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        #endregion

        #region Properties
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get
            {
                if (this._roleManager == null)
                    this._roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.DbContext));
                return this._roleManager;
            }
            set
            {
                this._roleManager = value;
            }
        }

        public FaApplicationDbContext DbContext { get; set; }

        #endregion

        #region Constructors
        public CuentaController()
        {
            this.DbContext = FaApplicationDbContext.Create();
        }

        public CuentaController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.RoleManager = roleManager;
            this.DbContext = FaApplicationDbContext.Create();
        }
        #endregion

        #region Action Results
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Usuario, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Inicio de sesión incorrecto.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [FaAuthorize]
        public ActionResult Registrar()
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.DbContext));
            try
            {
                RegisterViewModel model = new RegisterViewModel();
                model.Password = DataSecurity.GenerateRandomPassword();


                model.Roles = ViewBag.RolesList = roleManager.Roles.ToList();
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [FaAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registrar(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.RolIdSeleccionado))
                {
                    this.ModelState.AddModelError("", CuentaResource.RolEsReq);
                    return View(model);
                }

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.DbContext));
                var user = new ApplicationUser() { UserName = model.Usuario };
                user.ReqCambioContraseña = true;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var result2 = await this.UserManager.AddToRoleAsync(this.UserManager.FindByName(model.Usuario).Id, roleManager.FindById(model.RolIdSeleccionado).Name);

                    if (result2.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        return RedirectToAction(nameof(ListaCuentas));
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Usuario);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Cuenta");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Cuenta");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);

            this.DataService.RemoverCache(this.HttpContext);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> ListaCuentas()
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);
            bool cuentasAuth = FaAuthorizeAttribute.IsAuthorized(this.User, nameof(CuentaController), nameof(ListaCuentas), this.HttpContext);
            if (!cuentasAuth && FaAuthorizeAttribute.IsAuthorized(this.User, nameof(CuentaController), nameof(ListaRoles), this.HttpContext))
                return RedirectToAction(nameof(ListaRoles));

            if (cuentasAuth)
                return this.View(await this.DataService.GetCuentas());
            else
                return new HttpStatusCodeResult(401);
        }

        [HttpGet]
        [FaAuthorize]
        public async Task<ActionResult> Editar(string idUsuario)
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(await this.GetIdentityUser(this.User.Identity.Name), this.UserManager, this.RoleManager);
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.DbContext));
            try
            {
                List<IdentityRole> roles = ViewBag.RolesList = roleManager.Roles.ToList();
                CuentaModel model = await this.DataService.GetCuenta(idUsuario);
                model.RolIdSeleccionado = roles.Find(x => x.Name == model.Roles[0]).Id;
                model.RolSeleccionado = roles.Find(x => x.Name == model.Roles[0]).Name;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [FaAuthorize]
        public async Task<ActionResult> Editar(CuentaModel model)
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(await this.GetIdentityUser(this.User.Identity.Name), this.UserManager, this.RoleManager);
            
            try
            {
                if (!string.IsNullOrEmpty(model.RolSeleccionado))
                {

                    var roles = await this.UserManager.GetRolesAsync(model.IdUsuario);
                    var r = await this.UserManager.RemoveFromRolesAsync(model.IdUsuario, roles.ToArray());
                    if (r.Succeeded)
                    {
                        r = await this.UserManager.AddToRoleAsync(model.IdUsuario, model.RolSeleccionado);
                        if (r.Succeeded)
                        {
                            return RedirectToAction(nameof(ListaCuentas));
                        }
                        else
                        {
                            this.ModelState.AddModelError("", r.Errors.FirstOrDefault());
                        }
                    }
                    return View(model);
                }
                else
                {
                    this.ModelState.AddModelError("", CuentaResource.RolEsReq);
                    return RedirectToAction(nameof(Editar), model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [FaAuthorize]
        public async Task<ActionResult> GenerarNuevaClave(string idUsuario)
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);
            try
            {
                CuentaModel model = await this.DataService.CrearNewCtrña(idUsuario);
                return Json(new { newValue = model.NuevaContraseña, msg = CuentaResource.MsgCntrñGenerada });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [FaAuthorize]
        public virtual ActionResult Eliminar(string borrarIdUsuario)
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);

            try
            {
                this.DataService.Eliminar(borrarIdUsuario);
                return RedirectToAction(nameof(ListaCuentas));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Cambio de contraseña temporal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> CambiarContraseña()
        {
            return View(new ChangePasswordViewModel() { IdUsuario = this.GetIdentityUser(this.User.Identity.Name).Result.Id });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CambiarContraseña(ChangePasswordViewModel model)
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);

            try
            {
                if (string.IsNullOrEmpty(model.Error))
                {
                    model.OldPassword = "temp123456789";
                    if (this.TryValidateModel(model))
                    {
                        await this.DataService.CambiarCtrña(model.IdUsuario, model.NewPassword);
                        return this.RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        this.FillModel(model);
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            return View();
        }












        [FaAuthorize]
        public async Task<ActionResult> VerRol(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("Rol ID");
            if (this.DataService == null)
                this.DataService = new CuentaDataService(await this.GetIdentityUser(this.User.Identity.Name), this.UserManager, this.RoleManager);

            try
            {
                var results = await this.DataService.GetRol(id, this.HttpContext);

                this.ViewBag.ViewsPermitidos = await this.DataService.GetViewsPermitidas(id, this.HttpContext);
                this.ViewBag.ViewList = CuentaResource.Views.Split('|').Select(x => new PermitirViewModel() { Nombre = x.Split(':')[0] }).ToList();
                return this.View(results);
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        [HttpGet]
        [FaAuthorize]
        public async Task<ActionResult> ListaRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.DbContext));

            bool cuentasAuth = FaAuthorizeAttribute.IsAuthorized(this.User, nameof(CuentaController), nameof(ListaRoles), this.HttpContext);
            if (!cuentasAuth && FaAuthorizeAttribute.IsAuthorized(this.User, nameof(CuentaController), nameof(ListaCuentas), this.HttpContext))
                return RedirectToAction(nameof(ListaCuentas));

            if (cuentasAuth)
                return this.View(roleManager.Roles.ToList());
            else
                return new HttpStatusCodeResult(401);
        }

        [FaAuthorize]
        public ActionResult RegistrarRol()
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.DbContext));
            try
            {
                //model.Roles = ViewBag.RolesList = roleManager.Roles.ToList();
                return View();
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        //
        [HttpPost]
        [FaAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrarRol(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.DbContext));

                var result = await roleManager.CreateAsync(model);
                if (result.Succeeded)
                {
                    FaAuthorizeAttribute.RolActualizado(model.Id);
                    return RedirectToAction(nameof(ListaRoles));
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [FaAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrarRolViewPermit(string idRol, string viewName, byte? todas)
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);
            if (ModelState.IsValid)
            {
                try
                {
                    await this.DataService.CrearRolViewPermit(idRol, ViewUtil.ObtenerDireccionDeView(viewName), todas.GetValueOrDefault(0), this.HttpContext);
                    FaAuthorizeAttribute.RolActualizado(idRol);
                    return RedirectToAction(nameof(VerRol), new { id = idRol });
                }
                catch (Exception ex)
                {
                    this.ModelState.AddModelError("", ex);
                    return RedirectToAction(nameof(ListaRoles));
                }
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction(nameof(RegistrarRolViewPermit));
        }

        [HttpPost]
        [FaAuthorizeAttribute]
        public virtual async Task<ActionResult> EliminarRol(string borrarRolId)
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);

            try
            {
                await this.DataService.Eliminar(borrarRolId);
                FaAuthorizeAttribute.RolActualizado(borrarRolId);
                return RedirectToAction(nameof(ListaRoles));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        [FaAuthorizeAttribute]
        public virtual async Task<ActionResult> EliminarRolView(string idRol, long idView)
        {
            if (this.DataService == null)
                this.DataService = new CuentaDataService(this.GetIdentityUser(this.User.Identity.Name).Result, this.UserManager, this.RoleManager);

            try
            {
                await this.DataService.EliminarRolView(idRol, idView, this.HttpContext);
                FaAuthorizeAttribute.RolActualizado(idRol);
                return RedirectToAction(nameof(VerRol), new { id = idRol });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected virtual void FillModel(ChangePasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                string errorDesc = string.Join(Environment.NewLine, this.ModelState.Values.Select(m => m.Errors.Count > 0 ? m.Errors.First().ErrorMessage : string.Empty));
                model.Error = errorDesc;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        #region Protected
        protected async virtual Task<ApplicationUser> GetIdentityUser(string usuario)
        {
            return await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
        }
        #endregion

        #region Properties
        public CuentaDataService DataService { get; set; }
        #endregion
    }
}