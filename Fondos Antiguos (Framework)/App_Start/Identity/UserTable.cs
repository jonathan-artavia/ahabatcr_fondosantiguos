using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fondos_Antiguos
{
    /// <summary>
    /// Class that represents the Users table in the MySQL Database
    /// </summary>
    public class UserTable
    {
        IUser _user;
        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UserTable(IUser user)
        {
            _user = user;
        }

        /// <summary>
        /// Returns the user's name given a user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(string userId)
        {
            string commandText = SqlResource.SqlUsuariosUsuarioByIdUsuario;
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            return DataConnection.Instance.ExecuteQuery(commandText, parameters, this._user).ToString();
        }

        /// <summary>
        /// Returns a User ID given a user name
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public string GetUserId(string userName)
        {
            string commandText = SqlResource.SqlUsuariosIdUsuarioByUsuario;
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            return DataConnection.Instance.ExecuteQuery(commandText, parameters, this._user).ToString();
        }

        /// <summary>
        /// Returns an TUser given the user's id
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public ApplicationUser GetUserById(string userId)
        {
            ApplicationUser user = null;
            string commandText = SqlResource.SqlUsuariosByIdUsuario;
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            var rows = DataConnection.Instance.ExecuteQuery(commandText, parameters,this._user);
            if (rows != null && rows.Read())
            {
                
                user = (ApplicationUser)Activator.CreateInstance(typeof(ApplicationUser));
                this.Fill(user, rows);
            }

            return user;
        }

        /// <summary>
        /// Returns a list of TUser instances given a user name
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <returns></returns>
        public List<ApplicationUser> GetUserByName(string userName)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            string commandText = SqlResource.SqlUsuariosByUsuario;
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            var rows = DataConnection.Instance.ExecuteQuery(commandText, parameters, this._user);
            while(rows.Read())
            {
                ApplicationUser user = (ApplicationUser)Activator.CreateInstance(typeof(ApplicationUser));
                this.Fill(user, rows);
                users.Add(user);
            }

            return users;
        }

        public List<ApplicationUser> GetUserByEmail(string email)
        {
            return null;
        }

        /// <summary>
        /// Return the user's password hash
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public string GetPasswordHash(string userId)
        {
            return null;
        }

        /// <summary>
        /// Sets the user's password hash
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public int SetPasswordHash(string userId, byte[] passwordHash, bool reqCambioContra)
        {
            string commandText = SqlResource.SqlUsuariosUpdatePasswordHash;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@pwdHash", passwordHash);
            parameters.Add("@req", reqCambioContra);
            parameters.Add("@id", userId);
            return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
        }

        /// <summary>
        /// Returns the user's security stamp
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetSecurityStamp(string userId)
        {
            return null;
        }

        /// <summary>
        /// Inserts a new user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Insert(ApplicationUser user)
        {
            string commandText = SqlResource.SqlUsuariosInsert;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", user.UserName);
            parameters.Add("@id", user.Id);
            parameters.Add("@pwdHash", Encoding.UTF8.GetBytes(user.PasswordHash));
            parameters.Add("@fechaIngreso", user.FechaIngreso);
            parameters.Add("@intentos", user.AccessFailedCount);
            parameters.Add("@bloqueado", user.LockoutEnabled);
            parameters.Add("@fechaDesb", user.LockoutEndDateUtc);
            parameters.Add("@reqContra", user.ReqCambioContraseña);
            //parameters.Add("@SecStamp", user.SecurityStamp);
            //parameters.Add("@email", user.Email);
            //parameters.Add("@emailconfirmed", user.EmailConfirmed);
            //parameters.Add("@phonenumber", user.PhoneNumber);
            //parameters.Add("@phonenumberconfirmed", user.PhoneNumberConfirmed);
            //parameters.Add("@accesscount", user.AccessFailedCount);
            //parameters.Add("@lockoutenabled", user.LockoutEnabled);
            //parameters.Add("@lockoutenddate", user.LockoutEndDateUtc);
            //parameters.Add("@twofactorenabled", user.TwoFactorEnabled);

            return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        private int Delete(string userId)
        {
            string commandText = SqlResource.SqlUsuariosDeleteByIdUsuario;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Delete(ApplicationUser user)
        {
            return Delete(user.Id);
        }

        /// <summary>
        /// Updates a user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(ApplicationUser user)
        {
            string commandText = SqlResource.SqlUsuariosUpdateByIdUsuario;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userName", user.UserName);
            parameters.Add("@pswHash", user.PasswordHash);
            parameters.Add("@userId", user.Id);
            parameters.Add("@intentos", user.AccessFailedCount);
            parameters.Add("@bloqueado", user.LockoutEnabled);
            parameters.Add("@fechaDesb", user.LockoutEndDateUtc);
            return DataConnection.Instance.ExecuteNonQuery(commandText, parameters, this._user);
        }

        public IEnumerable<ApplicationUser> GetUsers(string filter, Dictionary<string,object> parameters)
        {
            string commandText = string.Format(SqlResource.SqlUsuariosResource, string.IsNullOrEmpty(filter) ? "1=1" : filter);
            IDataReader dr = DataConnection.Instance.ExecuteQuery(commandText, parameters, this._user);
            while (dr.Read())
            {
                ApplicationUser user = new ApplicationUser();
                this.Fill(user, dr);
                yield return user;
            }
        }

        protected virtual void Fill(ApplicationUser user, IDataReader reader)
        {
            user.IdUsuario = Convert.ToInt64(reader["Id"]);
            user.UserName = reader["Usuario"].ToString();
            user.Id = reader["IdUsuario"].ToString();
            user.IdRol = Convert.ToString(reader["IdRol"]);
            user.FechaIngreso = (DateTime)reader["FechaIngreso"];
            user.LockoutEnabled = Convert.ToBoolean(reader["EstaBloqueado"]);
            user.LockoutEndDateUtc = Convert.IsDBNull(reader["FechaDesbloqueo"]) ? null : (DateTime?)reader["FechaDesbloqueo"];
            user.AccessFailedCount = Convert.ToInt32(reader["IntentosFallidos"]);
            user.ReqCambioContraseña = Convert.ToBoolean(reader[6]);
            //user.SecurityStamp = string.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
            //user.Email = string.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
            //user.EmailConfirmed = row["EmailConfirmed"] == "1" ? true : false;
            //user.PhoneNumber = string.IsNullOrEmpty(row["PhoneNumber"]) ? null : row["PhoneNumber"];
            //user.PhoneNumberConfirmed = row["PhoneNumberConfirmed"] == "1" ? true : false;
            //user.LockoutEnabled = row["LockoutEnabled"] == "1" ? true : false;
            //user.LockoutEndDateUtc = string.IsNullOrEmpty(row["LockoutEndDateUtc"]) ? DateTime.Now : DateTime.Parse(row["LockoutEndDateUtc"]);
            //user.AccessFailedCount = string.IsNullOrEmpty(row["AccessFailedCount"]) ? 0 : int.Parse(row["AccessFailedCount"]);
        }
    }

}
