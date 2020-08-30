using Fondos_Antiguos.Localization;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fondos_Antiguos
{
    /// <summary>
    /// Class that represents the Role table in the MySQL Database
    ///// </summary>
    //public class RoleTable
    //{
    //    IUser _user;

    //    /// <summary>
    //    /// Constructor that takes a MySQLDatabase instance 
    //    /// </summary>
    //    /// <param name="database"></param>
    //    public RoleTable(IUser user)
    //    {
    //        this._user = user;
    //    }

    //    /// <summary>
    //    /// Deltes a role from the Roles table
    //    /// </summary>
    //    /// <param name="roleId">The role Id</param>
    //    /// <returns></returns>
    //    public int Delete(string roleId)
    //    {
    //        string commandText = SqlResource.SqlRolesDelete;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>();
    //        parameters.Add("@id", roleId);

    //        return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
    //    }

    //    /// <summary>
    //    /// Inserts a new Role in the Roles table
    //    /// </summary>
    //    /// <param name="roleName">The role's name</param>
    //    /// <returns></returns>
    //    public int Insert(FaIdentityRole role)
    //    {
    //        string commandText = SqlResource.SqlRolesInsert;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>();
    //        parameters.Add("@name", role.Name);
    //        parameters.Add("@id", role.Id);

    //        return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
    //    }

    //    /// <summary>
    //    /// Returns a role name given the roleId
    //    /// </summary>
    //    /// <param name="roleId">The role Id</param>
    //    /// <returns>Role name</returns>
    //    public string GetRoleName(string roleId)
    //    {
    //        string commandText = SqlResource.SqlRolesNombreByIdRol;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>();
    //        parameters.Add("@id", roleId);

    //        return DataConnection.Instance.ExecuteScalar(commandText, parameters, this._user).ToString();
    //    }

    //    /// <summary>
    //    /// Returns a role name given the roleId
    //    /// </summary>
    //    /// <param name="roleId">The role Id</param>
    //    /// <returns>Role name</returns>
    //    public string GetRoleName(int roleId)
    //    {
    //        string commandText = SqlResource.SqlRolesNombreById;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>();
    //        parameters.Add("@id", roleId);

    //        return DataConnection.Instance.ExecuteScalar(commandText, parameters, this._user).ToString();
    //    }

    //    /// <summary>
    //    /// Returns the role Id given a role name
    //    /// </summary>
    //    /// <param name="roleName">Role's name</param>
    //    /// <returns>Role's Id</returns>
    //    public string GetRoleId(string roleName)
    //    {
    //        string roleId = null;
    //        string commandText = SqlResource.SqlRolesGetId;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", roleName } };

    //        object result = DataConnection.Instance.ExecuteScalar(commandText, parameters, this._user);
    //        if (result != null)
    //        {
    //            return Convert.ToString(result);
    //        }

    //        return roleId;
    //    }

    //    /// <summary>
    //    /// Gets the IdentityRole given the role Id
    //    /// </summary>
    //    /// <param name="roleId"></param>
    //    /// <returns></returns>
    //    public FaIdentityRole GetRoleById(string roleId)
    //    {
    //        var roleName = this.GetRoleName(roleId);
    //        FaIdentityRole role = null;

    //        if (roleName != null)
    //        {
    //            role = new FaIdentityRole(roleName, roleId);
    //        }

    //        return role;

    //    }

    //    /// <summary>
    //    /// Gets the IdentityRole given the role Id
    //    /// </summary>
    //    /// <param name="roleId"></param>
    //    /// <returns></returns>
    //    public FaIdentityRole GetRoleById(int roleId)
    //    {
    //        var roleName = this.GetRoleName(roleId);
    //        FaIdentityRole role = null;

    //        if (roleName != null)
    //        {
    //            role = new FaIdentityRole(roleName);
    //        }

    //        return role;

    //    }

    //    /// <summary>
    //    /// Gets the IdentityRole given the role name
    //    /// </summary>
    //    /// <param name="roleName"></param>
    //    /// <returns></returns>
    //    public FaIdentityRole GetRoleByName(string roleName)
    //    {
    //        var roleId = GetRoleId(roleName);
    //        FaIdentityRole role = null;

    //        if (roleId != null)
    //        {
    //            role = new FaIdentityRole(roleName, roleId);
    //        }

    //        return role;
    //    }

    //    public int Update(FaIdentityRole role)
    //    {
    //        string commandText = SqlResource.SqlRolesUpdate;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>();
    //        parameters.Add("@id", role.Id);

    //        return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
    //    }
    //}

}
