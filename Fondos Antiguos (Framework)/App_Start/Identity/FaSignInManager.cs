using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Fondos_Antiguos
{
    public class FaSignInManager : SignInManager<ApplicationUser, string>, IDisposable
    {
        #region Const
        public const string AnonIdUsuario = "BE75D823412D4DA9AEC6236C6CD73BF8";
        #endregion

        #region Constructor
        public FaSignInManager(UserManager<ApplicationUser, string> userManager, IAuthenticationManager authManager) : base(userManager,authManager)
        {
        }
        #endregion
        #region Static
        public static bool IsAnon(IIdentity principal)
        {
            var userId = principal.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                return AnonIdUsuario.Equals(userId);
            }
            return true;
        }
        #endregion
        #region Overrides

        /////<summary>
        /////     Attempts to sign in the specified user and password combination as an asynchronous
        /////     operation.
        /////     </summary>
        //public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        //{
        //    SignInStatus signInStatu = SignInStatus.Failure;
        //    if (this.UserManager != null)
        //    {
        //        /// changed to use email address instead of username
        //        ApplicationUser tUser = await this.UserManager.FindByNameAsync(userName);

        //        if (tUser != null)
        //        {
        //            Task<bool> cultureAwaiter1 = this.UserManager.IsLockedOutAsync(tUser.Id);
        //            if (!await cultureAwaiter1)
        //            {
        //                Task<bool> cultureAwaiter2 = this.UserManager.CheckPasswordAsync(tUser, password);
        //                if (!await cultureAwaiter2)
        //                {
        //                    if (shouldLockout) //enforced lockout
        //                    {
        //                        IdentityResult cultureAwaiter3 = await this.UserManager.AccessFailedAsync(tUser.Id);
        //                        Task<bool> cultureAwaiter4 = this.UserManager.IsLockedOutAsync(tUser.Id);
        //                        if (await cultureAwaiter4)
        //                        {
        //                            signInStatu = SignInStatus.LockedOut;
        //                            return signInStatu;
        //                        }
        //                    }
        //                    signInStatu = SignInStatus.Failure;
        //                }
        //                else
        //                {
        //                    Task<IdentityResult> cultureAwaiter5 = this.UserManager.ResetAccessFailedCountAsync(tUser.Id);
        //                    await cultureAwaiter5;
        //                    signInStatu = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        //                    signInStatu = SignInStatus.Success;
        //                }
        //            }
        //            else
        //            {
        //                signInStatu = SignInStatus.LockedOut;
        //            }
        //        }
        //        else
        //        {
        //            signInStatu = SignInStatus.Failure;
        //        }
        //    }
        //    else
        //    {
        //        signInStatu = SignInStatus.Failure;
        //    }
        //    return signInStatu;
        //}

        //public override async Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser)
        //{
        //    await this.PasswordSignInAsync(user.UserName, user.PasswordHash, isPersistent, rememberBrowser);
        //}

        /////<summary>
        /////     Attempts to sign in the specified user and password combination as an asynchronous
        /////     operation.
        /////     </summary>
        //public async override Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password, bool isPersistent, bool lockoutOnFailure)
        //{
        //    return await this.PasswordSignInAsync(user.UserName, DataSecurity.Hash(password), isPersistent, lockoutOnFailure);
        //}

        //public override Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user)
        //{
        //    return base.CreateUserPrincipalAsync(user);
        //}
        #endregion

        #region IDisposable members
        
        #endregion
    }

}
