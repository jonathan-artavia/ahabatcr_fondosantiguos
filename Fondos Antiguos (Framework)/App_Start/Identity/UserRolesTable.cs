using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Fondos_Antiguos
{
    /// <summary>
    ///// Class that represents the UserRoles table in the MySQL Database
    ///// </summary>
    //public class UserRolesTable
    //{
    //    IUser _user;
    //    /// <summary>
    //    /// Constructor that takes a MySQLDatabase instance 
    //    /// </summary>
    //    /// <param name="database"></param>
    //    public UserRolesTable(IUser user)
    //    {
    //        this._user = user;
    //    }

    //    /// <summary>
    //    /// Returns a list of user's roles
    //    /// </summary>
    //    /// <param name="userId">The user's id</param>
    //    /// <returns></returns>
    //    public List<string> FindByUserId(string userId)
    //    {
    //        FaRoleStore role = new FaRoleStore(this._user);
            
    //        List<string> roles = new List<string>();
    //        string commandText = SqlResource.SqlUsuarioRolesByUserId;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>();
    //        parameters.Add("@userId", userId);

    //        var rows = DataConnection.Instance.ExecuteQuery(commandText, parameters, this._user);
    //        while(rows.Read())
    //        {
    //            roles.Add(rows["Nombre"].ToString());
    //        }

    //        return roles;
    //    }

    //    /// <summary>
    //    /// Deletes all roles from a user in the UserRoles table
    //    /// </summary>
    //    /// <param name="userId">The user's id</param>
    //    /// <returns></returns>
    //    public int Delete(string userId)
    //    {
    //        string commandText = SqlResource.SqlUsuarioRolesDelete;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>();
    //        parameters.Add("@userId", userId);

    //        return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
    //    }

    //    /// <summary>
    //    /// Inserts a new role for a user in the UserRoles table
    //    /// </summary>
    //    /// <param name="user">The User</param>
    //    /// <param name="roleId">The Role's id</param>
    //    /// <returns></returns>
    //    public async Task<int> Insert(FaIdentityUser user, string roleId)
    //    {
    //        FaRoleStore st = new FaRoleStore(this._user);
    //        FaIdentityRole rol = await st.FindByIdAsync(roleId);

    //        string commandText = SqlResource.SqlUsuarioRolesInsert;
    //        Dictionary<string, object> parameters = new Dictionary<string, object>();
    //        parameters.Add("@userId", user.Id);
    //        parameters.Add("@rolId", roleId);

    //        return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
    //    }
    //}

}
