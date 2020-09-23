using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Fondos_Antiguos.Models;
using Fondos_Antiguos.Localization;
using MySql.Data.MySqlClient;
using System.Data.Entity.Utilities;
using System.Globalization;

namespace Fondos_Antiguos
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class FaUserManager : UserManager<ApplicationUser>
    {
        #region Fields

        #endregion

        #region Constructors
        public FaUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            PasswordHasher = new DataSecurity();
        }
        #endregion
        #region Overrides
        public override Task<bool> IsInRoleAsync(string userId, string role)
        {
            return base.IsInRoleAsync(userId, role);
        }

        public virtual async Task<IdentityResult> ChangePasswordHashedCurrentAsync(string userId, string currentPassword, string newPassword)
        {
            IUserPasswordStore<ApplicationUser, string> passwordStore = (IUserPasswordStore<ApplicationUser, string>)Store;
            ApplicationUser user = await FindByIdAsync(userId).WithCurrentCulture();
            if (user == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "User ID no encontrado", new object[1]
                {
                userId
                }));
            }
            if (await VerifyHashedPasswordAsync(passwordStore, user, currentPassword).WithCurrentCulture())
            {
                IdentityResult identityResult = await UpdatePassword(passwordStore, user, newPassword).WithCurrentCulture();
                if (!identityResult.Succeeded)
                {
                    return identityResult;
                }
                return await UpdateAsync(user).WithCurrentCulture();
            }
            return IdentityResult.Failed("Contraseña invalida");
        }

        /// <summary>
        /// Same as <see cref="VerifyPasswordAsync(IUserPasswordStore{ApplicationUser, string}, ApplicationUser, string)"/> but the <paramref name="password"/> is the hashed password.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected virtual async Task<bool> VerifyHashedPasswordAsync(IUserPasswordStore<ApplicationUser, string> store, ApplicationUser user, string password)
        {
            string hashedPassword = await store.GetPasswordHashAsync(user).WithCurrentCulture();
            return ((DataSecurity)PasswordHasher).VerifyUnHashedPassword(hashedPassword, password) != PasswordVerificationResult.Failed;
        }


        #endregion

        #region Static
        public static bool IsValid(string username, byte[] password, HttpContextBase context)
        {
            int result = 0;
            QueryExpresion expr = new QueryExpresion("AND", SqlUtil.Equals("Usuario", "@username", false));
            expr = expr.And(SqlUtil.Equals("Cntrña", "@password", false));

            string cmd = string.Format(SqlResource.SqlUsuariosResource, expr.ToString());
            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            mySqlParameters.Add(new MySqlParameter("@username", username));
            mySqlParameters.Add(new MySqlParameter("@password", password));

            result = (int)DataConnection.Instance.ExecuteScalar(cmd, mySqlParameters, context);
            return result >= 1;
        }

        public static bool IsValid(string username, byte[] password, IUser user)
        {
            long result = 0;
            QueryExpresion expr = new QueryExpresion("AND", SqlUtil.Equals("Usuario", "@username", false));
            expr = expr.And(SqlUtil.Equals("Cntrña", "@password", false));

            string cmd = string.Format(SqlResource.SqlUsuariosResourceCount, expr.ToString());
            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            mySqlParameters.Add(new MySqlParameter("@username", username));
            mySqlParameters.Add(new MySqlParameter("@password", password));

            result = (long)DataConnection.Instance.ExecuteScalar(cmd, mySqlParameters, user);
            return result >= 1;
        }
        #endregion

        #region Properties
        public override IQueryable<ApplicationUser> Users => base.Users;
        #endregion
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : FaUserManager
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<FaApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : FaSignInManager
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
