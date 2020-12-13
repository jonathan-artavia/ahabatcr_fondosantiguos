using Fondos_Antiguos.Base;
using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Fondos_Antiguos.DataService
{
    public class LugaresDataService : BaseDataService
    {
        #region Constructor
        public LugaresDataService() : base()
        {

        }
        #endregion

        #region Public
        public virtual IEnumerable<LugarModel> ObtenerLugares(QueryExpresion expr, Dictionary<string, object> parameters, HttpContextBase context)
        {
            List<LugarModel> result = this.GetOrCreateValue<List<LugarModel>>(
                this.GetOrCreateKey(context),
                () => this.LlenarModelos(DataConnection.Instance.ExecuteQuery(
                    string.Format(SqlResource.SqlLugarResource, expr?.ToString() ?? "1=1"),
                    parameters,
                    context, 0)),
                nameof(ObtenerLugares), expr?.ToString()
                );

            return result;
        }

        public virtual void Insertar(LugarModel model, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@nombre", model.Nombre);
            try
            {
                LugarModel dup = this.ObtenerLugares(new QueryExpresion($"lcase({SqlUtil.SurroundColumn("Nombre")}) = @nombre"), new Dictionary<string, object>() { { "@nombre", model.Nombre.ToLower() } }, context)?.FirstOrDefault();

                //buscar por duplicados
                if (dup != null)
                    throw new Exception(string.Format(CatalogoRes.MateriaYaExisteFmt, dup.Nombre));

                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlLugarInsert, parameters, context);
                ulong id = (ulong)DataConnection.Instance.ExecuteScalar(SqlResource.SqlLastInsertedId, default(Dictionary<string, object>), context, transaction: null);
                model.ID = (long)id;
                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerLugares));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Borrar(long id, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", id);
            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlLugarDelete, parameters, context);

                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerLugares));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Protected
        protected virtual List<LugarModel> LlenarModelos(IDataReader reader)
        {
            List<LugarModel> result = new List<LugarModel>();
            while (reader.Read())
            {
                LugarModel nc = new LugarModel();
                nc.Fill(reader);
                result.Add(nc);
            }
            return result;
        }
        #endregion
    }
}