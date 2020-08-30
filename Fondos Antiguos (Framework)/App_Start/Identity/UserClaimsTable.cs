using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fondos_Antiguos
{
    /// <summary>
    /// Class that represents the UserClaims table in the MySQL Database
    /// </summary>
    public class UserClaimsTable
    {
        IUser _user;
        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UserClaimsTable(IUser user)
        {
            this._user = user;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(string userId)
        {
            ClaimsIdentity claims = new ClaimsIdentity();
            string commandText = SqlResource.SqlUsuarioClaimSelect;
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@UserId", userId } };

            var rows = DataConnection.Instance.ExecuteQuery(commandText, parameters, this._user);
            while(rows.Read())
            {
                Claim claim = new Claim(rows["ClaimType"].ToString(), rows["ClaimValue"].ToString());
                claims.AddClaim(claim);
            }

            return claims;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = SqlResource.SqlUsuarioClaimDelete;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
        }

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim">User's claim to be added</param>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        public int Insert(Claim userClaim, string userId)
        {
            string commandText = SqlResource.SqlUsuarioClaimInsert;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@value", userClaim.Value);
            parameters.Add("@type", userClaim.Type);
            parameters.Add("@userId", userId);

            return DataConnection.Instance.ExecuteNonQuery(commandText, parameters,this._user);
        }

        /// <summary>
        /// Deletes a claim from a user 
        /// </summary>
        /// <param name="user">The user to have a claim deleted</param>
        /// <param name="claim">A claim to be deleted from user</param>
        /// <returns></returns>
        public int Delete(ApplicationUser user, Claim claim)
        {
            string commandText = SqlResource.SqlUsuarioClaimDeleteSingle;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", user.Id);
            parameters.Add("@value", claim.Value);
            parameters.Add("@type", claim.Type);

            return DataConnection.Instance.ExecuteNonQuery(commandText, parameters,this._user);
        }
    }

}
