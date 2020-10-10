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
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using Fondos_Antiguos.Base;
using System.Data;
using Fondos_Antiguos.Localization;

namespace Fondos_Antiguos.DataService
{
    public class CuentaDataService : BaseDataService
    {
        public ApplicationUserManager UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; set; }
        #region Fields
        private IUser _user;
        #endregion

        #region Constructors
        public CuentaDataService(IUser user)
        {
            
            this._user = user;
        }

        public CuentaDataService(IUser user, ApplicationUserManager man, RoleManager<IdentityRole> rolMan)
        {
            this.UserManager = man;
            this.RoleManager = rolMan;
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


        public virtual async Task EliminarRol(string idRol)
        {
            IdentityRole rol = await this.GetIdentityRole(idRol);

            if (rol != null)
            {
                IdentityResult result = await this.RoleManager.DeleteAsync(rol);
                if (!result.Succeeded)
                {
                    throw this.CraftException(result.Errors);
                }
            }
            else
            {
                throw new Exception("Rol no existe");
            }
        }

        public virtual async Task<IdentityRole> GetRol(string idRol, HttpContextBase httpContext)
        {
            return await this.GetIdentityRole(idRol);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRol"></param>
        /// <param name="dirView"></param>
        /// <param name="todas">0 = NA, 1 = Permitir Todas, 2 = Denegar Todas</param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public virtual async Task CrearRolViewPermit(string idRol, string dirView, byte todas, HttpContextBase httpContext)
        {
            IdentityRole rol = await this.GetIdentityRole(idRol);

            if (rol != null)
            {
                if (string.IsNullOrEmpty(dirView))
                {
                    QueryExpresion expr = new QueryExpresion(SqlUtil.Equals("IdRol", "@idRol")).And($"{SqlUtil.Equals("All", "@all1")} {SqlUtil.OR} {SqlUtil.Equals("All", "@all2")}");
                    DataConnection.Instance.ExecuteNonQuery(string.Format("DELETE FROM `tblRolView` WHERE {0}", expr.ToString()), new Dictionary<string, object>() { { "@idRol", idRol }, { "@all1", 1 }, { "@all2", 2 } }, httpContext);
                }
                if(todas != 0 || !string.IsNullOrEmpty(dirView))
                    DataConnection.Instance.ExecuteNonQuery("INSERT INTO `tblRolView` (`View`, `IdRol`, `All`) VALUES (@view, @idRol, @all)", new Dictionary<string, object>() { { "@view", todas != 0 ? "*" : dirView }, { "@idRol", idRol }, { "@all",todas } }, httpContext);
            }
            else
            {
                throw new Exception("Rol no existe");
            }
        }

        public virtual async Task EliminarRolView(string idRol, long idView, HttpContextBase httpContext)
        {
            IdentityRole rol = await this.GetIdentityRole(idRol);

            if (rol != null)
            {
                QueryExpresion expr = new QueryExpresion(SqlUtil.Equals("IdRol", "@idRol")).And(SqlUtil.Equals("ID", "@idView"));

                DataConnection.Instance.ExecuteNonQuery(string.Format("DELETE FROM `tblRolView` WHERE {0}", expr.ToString()), new Dictionary<string, object>() { { "@idRol", idRol }, { "@idView", idView } }, httpContext);
            }
            else
            {
                throw new Exception("Rol no existe");
            }
        }

        public virtual async Task<List<IdentityRolPermit>> GetViewsPermitidas(string idRol, HttpContextBase httpContext)
        {
            List<IdentityRolPermit> read = new List<IdentityRolPermit>();
            QueryExpresion expr = new QueryExpresion(SqlUtil.Equals("IdRol", idRol, true));
            read = this.GetOrCreateValue<List<IdentityRolPermit>>(
                        this.GetOrCreateKey(httpContext),
                        () => this.FillVistasDeRol(DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlRolesVistasPermitidasByRol, expr.ToString()),
                            new Dictionary<string, object>() { { "@id", idRol } },
                            httpContext,
                            1))
                        , "GetViewsPermitidas", expr?.ToString()
                        );
            return read;
        }
        #endregion

        #region protected
        protected async virtual Task<ApplicationUser> GetIdentityUser(string idUsuario)
        {
            return await this.UserManager.FindByIdAsync(idUsuario);
        }

        protected async virtual Task<IdentityRole> GetIdentityRole(string idRol)
        {
            return await this.RoleManager.FindByIdAsync(idRol);
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

        protected virtual List<IdentityRolPermit> FillVistasDeRol(IDataReader reader)
        {
            List<IdentityRolPermit> result = new List<IdentityRolPermit>();
            while (reader.Read())
            {
                IdentityRolPermit nc = new IdentityRolPermit();
                nc.Fill(reader);
                result.Add(nc);
            }
            return result;
        }
        #endregion

        #region Properties
        protected IUser User => this._user;
        #endregion
    }
}
