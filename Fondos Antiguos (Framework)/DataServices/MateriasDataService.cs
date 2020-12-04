using Fondos_Antiguos.Base;
using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Fondos_Antiguos.DataServices
{
    public class MateriasDataService : BaseDataService
    {
        #region Constructor
        public MateriasDataService() : base()
        {

        }
        #endregion

        #region Public
        public virtual IEnumerable<MateriaModel> ObtenerMaterias(QueryExpresion expr, Dictionary<string, object> parameters, HttpContextBase context)
        {
            List<MateriaModel> result = this.GetOrCreateValue<List<MateriaModel>>(
                this.GetOrCreateKey(context),
                () => this.LlenarModelos(DataConnection.Instance.ExecuteQuery(
                    string.Format(SqlResource.SqlMateriasResource, expr?.ToString() ?? "1=1"),
                    parameters,
                    context, 0)),
                nameof(ObtenerMaterias), expr?.ToString()
                );

            return result;
        }

        public virtual void Insertar(MateriaModel model, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@nombre", model.Nombre);
            try
            {
                MateriaModel dup = this.ObtenerMaterias(new QueryExpresion($"lcase({SqlUtil.SurroundColumn("Nombre")}) = @nombre"), new Dictionary<string, object>() { { "@nombre", model.Nombre.ToLower() } }, context)?.FirstOrDefault();

                //buscar por duplicados
                if (dup != null)
                    throw new Exception(string.Format(CatalogoRes.MateriaYaExisteFmt, dup.Nombre));

                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlMateriasInsert, parameters, context);
                ulong id = (ulong)DataConnection.Instance.ExecuteScalar(SqlResource.SqlLastInsertedId, default(Dictionary<string, object>), context, transaction: null);
                model.ID = (long)id;
                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerMaterias));
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
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlMateriasDelete, parameters, context);

                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerMaterias));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Protected
        protected virtual List<MateriaModel> LlenarModelos(IDataReader reader)
        {
            List<MateriaModel> result = new List<MateriaModel>();
            while (reader.Read())
            {
                MateriaModel nc = new MateriaModel();
                nc.Fill(reader);
                result.Add(nc);
            }
            return result;
        }
        #endregion
    }
}