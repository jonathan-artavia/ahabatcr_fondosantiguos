using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fondos_Antiguos
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity role store iterfaces
    /// </summary>
    //public class FaRoleStore : IRoleStore<FaIdentityRole>
    //{
        //private RoleTable roleTable;
        ////public MySQLDatabase Database { get; private set; }

        //public IQueryable<FaIdentityRole> Roles
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //private IUser _user;

        //public IUser User
        //{
        //    get { return this._user; }
        //    set { this._user = value; }
        //}

        //public FaRoleStore() : this(null)
        //{

        //}


        ///// <summary>
        ///// Default constructor that initializes a new MySQLDatabase
        ///// instance using the Default Connection string
        ///// </summary>
        //public FaRoleStore(IUser user)
        //{
        //    roleTable = new RoleTable(user);
        //}

        //public Task CreateAsync(FaIdentityRole role)
        //{
        //    if (role == null)
        //    {
        //        throw new ArgumentNullException("role");
        //    }

        //    roleTable.Insert(role);

        //    return Task.FromResult<object>(null);
        //}

        //public Task DeleteAsync(FaIdentityRole role)
        //{
        //    if (role == null)
        //    {
        //        throw new ArgumentNullException("user");
        //    }

        //    roleTable.Delete(role.Id);

        //    return Task.FromResult<Object>(null);
        //}

        //public Task<FaIdentityRole> FindByIdAsync(string roleId)
        //{
        //    FaIdentityRole result = roleTable.GetRoleById(roleId) as FaIdentityRole;

        //    return Task.FromResult<FaIdentityRole>(result);
        //}

        //public Task<FaIdentityRole> FindByNameAsync(string roleName)
        //{
        //    FaIdentityRole result = roleTable.GetRoleByName(roleName) as FaIdentityRole;

        //    return Task.FromResult<FaIdentityRole>(result);
        //}

        //public Task UpdateAsync(FaIdentityRole role)
        //{
        //    if (role == null)
        //    {
        //        throw new ArgumentNullException("user");
        //    }

        //    roleTable.Update(role);

        //    return Task.FromResult<Object>(null);
        //}

        //public void Dispose()
        //{
        //    DataConnection.Instance.ClearConnection(this.User);
        //}

        //#region Microsoft.AspNetCore.Identity.IRoleStore members
        //public async Task<IdentityResult> CreateAsync(FaIdentityRole role, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        await this.CreateAsync(role);
        //    }
        //    catch (Exception ex)
        //    {
        //        return IdentityResult.Failed(ex.Message );
        //    }
        //    return IdentityResult.Success;
        //}

        //public async Task<IdentityResult> DeleteAsync(FaIdentityRole role, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        await this.DeleteAsync(role);
        //    }
        //    catch (Exception ex)
        //    {
        //        return IdentityResult.Failed(ex.Message);
        //    }
        //    return IdentityResult.Success;
        //}

        //public async Task<FaIdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        return await this.FindByIdAsync(roleId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<FaIdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        //{
        //    return await this.FindByNameAsync(normalizedRoleName);
        //}

        //public async Task<string> GetNormalizedRoleNameAsync(FaIdentityRole role, CancellationToken cancellationToken)
        //{
        //    return role.Name.ToLowerInvariant();
        //}

        //public async Task<string> GetRoleIdAsync(FaIdentityRole role, CancellationToken cancellationToken)
        //{
        //    return this.roleTable.GetRoleId(role.Name);
        //}

        //public async Task<string> GetRoleNameAsync(FaIdentityRole role, CancellationToken cancellationToken)
        //{
        //    return this.roleTable.GetRoleName(role.Id);
        //}

        //public async Task SetNormalizedRoleNameAsync(FaIdentityRole role, string normalizedName, CancellationToken cancellationToken)
        //{
        //    role.Name = normalizedName;
        //}

        //public async Task SetRoleNameAsync(FaIdentityRole role, string roleName, CancellationToken cancellationToken)
        //{
        //    role.Name = roleName;
        //}

        //public async Task<IdentityResult> UpdateAsync(FaIdentityRole role, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        await this.UpdateAsync(role);
        //    }
        //    catch (Exception ex)
        //    {
        //        return IdentityResult.Failed(ex.Message);
        //    }
        //    return IdentityResult.Success;
        //}

        //#endregion
    //}

}
