using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Fondos_Antiguos
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class FaUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        private UserTable userTable;
        //private RoleTable roleTable;
        private RoleStore<IdentityRole> roleStore;
        private UserClaimsTable userClaimsTable;
        Microsoft.AspNet.Identity.IUser _user;

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public FaUserStore(Microsoft.AspNet.Identity.IUser user)
        {
            this._user = user;
            userTable = new UserTable(user);
            //roleTable = new RoleTable(user);
            //userRolesTable = new UserRolesTable(user);
            roleStore = new RoleStore<IdentityRole>();
            userClaimsTable = new UserClaimsTable(user);
        }

        public FaUserStore() : this(null)
        {

        }

        /// <summary>
        /// Insert a new ApplicationUser in the UserTable
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task CreateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            userTable.Insert(user);

        }

        /// <summary>
        /// Returns an ApplicationUser instance based on a userId query 
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns></returns>
        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            ApplicationUser result = userTable.GetUserById(userId) as ApplicationUser;
            if (result != null)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Returns an ApplicationUser instance based on a userName query 
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            List<ApplicationUser> result = userTable.GetUserByName(userName) as List<ApplicationUser>;

            // Should I throw if > 1 user?
            if (result != null && result.Count == 1)
            {
                return result[0];
            }

            return null;
        }

        /// <summary>
        /// Updates the UsersTable with the ApplicationUser instance values
        /// </summary>
        /// <param name="user">ApplicationUser to be updated</param>
        /// <returns></returns>
        public async Task UpdateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            userTable.Update(user);
        }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// Inserts a claim to the UserClaimsTable for the given user
        /// </summary>
        /// <param name="user">User to have claim added</param>
        /// <param name="claim">Claim to be added</param>
        /// <returns></returns>
        public Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("user");
            }

            userClaimsTable.Insert(claim, user.Id);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns all claims for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity identity = userClaimsTable.FindByUserId(user.Id);

            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
        }

        /// <summary>
        /// Removes a claim froma user
        /// </summary>
        /// <param name="user">User to have claim removed</param>
        /// <param name="claim">Claim to be removed</param>
        /// <returns></returns>
        public Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            userClaimsTable.Delete(user, claim);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Inserts a Login in the UserLoginsTable for a given User
        /// </summary>
        /// <param name="user">User to have login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public Task AddLoginAsync(ApplicationUser user, Microsoft.AspNet.Identity.UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an ApplicationUser based on the Login info
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<ApplicationUser> FindAsync(Microsoft.AspNet.Identity.UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns list of UserLoginInfo for a given ApplicationUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Microsoft.AspNet.Identity.UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            return null;
        }

        /// <summary>
        /// Deletes a login from UserLoginsTable for a given ApplicationUser
        /// </summary>
        /// <param name="user">User to have login removed</param>
        /// <param name="login">Login to be removed</param>
        /// <returns></returns>
        public Task RemoveLoginAsync(ApplicationUser user, Microsoft.AspNet.Identity.UserLoginInfo login)
        {
            return null;
        }

        /// <summary>
        /// Removes a user from a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task RemoveFromRoleAsync(ApplicationUser user, string role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task DeleteAsync(ApplicationUser user)
        {
            if (user != null)
            {
                userTable.Delete(user);
            }

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Returns the PasswordHash for a given ApplicationUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            string passwordHash = userTable.GetPasswordHash(user.Id);

            return Task.FromResult<string>(passwordHash);
        }

        /// <summary>
        /// Verifies if user has password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            var hasPassword = !string.IsNullOrEmpty(userTable.GetPasswordHash(user.Id));

            return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
        }

        /// <summary>
        /// Sets the password hash for a given ApplicationUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        ///  Set security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            user.SecurityStamp = stamp;

            return Task.FromResult(0);

        }

        /// <summary>
        /// Get security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// Set email on user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            user.Email = email;
            userTable.Update(user);

            return Task.FromResult(0);

        }

        /// <summary>
        /// Get email from user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Get if user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Set when user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            ApplicationUser result = userTable.GetUserByEmail(email).FirstOrDefault();
            if (result != null)
            {
                return Task.FromResult<ApplicationUser>(result);
            }

            return Task.FromResult<ApplicationUser>(null);
        }

        /// <summary>
        /// Set user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Get if user phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Set phone number if confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Set two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get if two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <summary>
        /// Get user lock out end date
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }


        /// <summary>
        /// Set user lockout end date
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Increment failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            user.AccessFailedCount++;
            userTable.Update(user);

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Reset failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            user.AccessFailedCount = 0;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Get if lockout is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// Set lockout enabled for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                await this.CreateAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(ex.Message);
            }
        }

        //Task IUserStore<ApplicationUser>.FindByIdAsync(string userId)
        //{
        //    return this.FindByIdAsync(userId);
        //}

        //Task IUserStore<ApplicationUser>.FindByNameAsync(string normalizedUserName)
        //{
        //    return this.FindByNameAsync(normalizedUserName);
        //}

        public async Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return user.UserName.ToLowerInvariant();
        }

        public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return user.Id;
        }

        //Task<string> IUserStore<ApplicationUser>.GetUserNameAsync(ApplicationUser user)
        //{
        //    return Task.FromResult(user.UserName);
        //}

        public async Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
        }

        public async Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            
            try
            {
                await this.UpdateAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(ex.Message);
            }
        }

        #region IUserPasswordStore members
        public async Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return user.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return !string.IsNullOrEmpty(await this.GetPasswordHashAsync(user, cancellationToken));
        }

        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            await this.UpdateAsync(user, cancellationToken);
        }
        #endregion
    }

}
