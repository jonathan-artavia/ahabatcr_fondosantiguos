using Fondos_Antiguos.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.CodeDom;

namespace Fondos_Antiguos.DataService
{
    public class CuentaDataService
    {
        public ApplicationUserManager UserManager { get; }
        #region Fields
        private IUser _user;
        #endregion

        #region Constructors
        public CuentaDataService(IUser user)
        {
            
            this._user = user;
        }

        public CuentaDataService(IUser user, ApplicationUserManager man)
        {
            this.UserManager = man;
            this._user = user;
        }
        #endregion

        #region Public
        public async virtual Task<IEnumerable<CuentaModel>> GetCuentas()
        {
            List<CuentaModel> result = new List<CuentaModel>();
            foreach (ApplicationUser user in this.UserManager.Users.ToList())
            {
                if (user.Id == "BE75D823412D4DA9AEC6236C6CD73BF8" || user.UserName.Equals("anon", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                CuentaModel model = await this.ToLoginModel(user);
                result.Add(model);
            }
            return result;
        }

        public async virtual Task<CuentaModel> GetCuenta(string idUsuario)
        {
            ApplicationUser user = await this.UserManager.FindByIdAsync(idUsuario);
            if (user == null)
                throw new Exception("Usuario no encontrado");
            return await this.ToLoginModel(user);
        }

        public virtual bool IsValid(string username, string password)
        {
            return FaUserManager.IsValid(username, Encoding.UTF8.GetBytes(DataSecurity.Hash(password)), this.User);
        }

        public virtual async Task<CuentaModel> CrearNewCtrña(string idUsuario)
        {
            ApplicationUser user = await this.GetIdentityUser(idUsuario);
            if (user != null)
            {
                CuentaModel nLoginModel = await this.ToLoginModel(user);
                nLoginModel.NuevaContraseña = DataSecurity.GenerateRandomPassword();
                
                IdentityResult res = await this.UserManager.ChangePasswordHashedCurrentAsync(idUsuario, user.PasswordHash, nLoginModel.NuevaContraseña);
                //this.UserTable.SetPasswordHash(user.Id, Encoding.UTF8.GetBytes(user.PasswordHash), true);
                return nLoginModel;
            }
            return null;
        }

        public virtual async Task CambiarCtrña(string idUsuario, string newContra)
        {
            ApplicationUser user = await this.GetIdentityUser(idUsuario);
            if (user != null)
            {
                IdentityResult res = await this.UserManager.ChangePasswordHashedCurrentAsync(idUsuario, user.PasswordHash, newContra);
                if (!res.Succeeded)
                {
                    throw this.CraftException(res.Errors);
                }
                else
                {
                    user = await this.GetIdentityUser(idUsuario);
                    user.ReqCambioContraseña = false;
                    IdentityResult res1 = await this.UserManager.UpdateAsync(user);
                    if(!res1.Succeeded)
                    {
                        throw this.CraftException(res1.Errors);
                    }
                }
            }
        }

        public virtual async Task Eliminar(string idUsuario)
        {
            ApplicationUser user = await this.GetIdentityUser(idUsuario);
            if(user != null)
            {
                if (user.Id == this.User.Id)
                    throw new Exception("No se puede borrar a si mismo");

                IdentityResult result = await this.UserManager.DeleteAsync(user);
                if(!result.Succeeded)
                {
                    throw this.CraftException(result.Errors);
                }
            }
            throw new Exception("Usuario no existe");
        }
        #endregion

        #region protected
        protected async virtual Task<ApplicationUser> GetIdentityUser(string idUsuario)
        {
            return await this.UserManager.FindByIdAsync(idUsuario);
        }

        protected virtual async Task<CuentaModel> ToLoginModel(ApplicationUser user)
        {
            CuentaModel nLoginModel = new CuentaModel();
            nLoginModel.FechaIngreso = user.FechaIngreso;
            nLoginModel.Id = user.IdUsuario;
            nLoginModel.IdUsuario = user.Id;
            nLoginModel.Usuario = user.UserName;
            nLoginModel.Roles.AddRange((await this.UserManager.GetRolesAsync(user.Id)).ToList());
            return nLoginModel;
        }

        protected virtual Exception CraftException(IEnumerable<string> errors)
        {
            Exception cursor = null;
            for(int i = errors.Count()-1; i >= 0; i--)
            {
                cursor = new Exception(errors.ElementAt(i), cursor);
            }
            return cursor;
        }
        #endregion

        #region Properties
        protected IUser User => this._user;
        #endregion
    }
}
