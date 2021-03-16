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
    public class EditorDataService : BaseDataService
    {
        #region Constructor
        public EditorDataService() : base()
        {

        }
        #endregion

        #region Public
        public virtual IEnumerable<EditorEnunciadoModel> ObtenerTextos(QueryExpresion expr, Dictionary<string, object> parameters, HttpContextBase context)
        {
            return this.LlenarModels(DataConnection.Instance.ExecuteQuery(
                    string.Format(SqlResource.SqlSeriesResource, expr?.ToString() ?? "1=1"),
                    parameters,
                    context, 0));
        }

        public virtual EditorEnunciadoModel ObtenerTexto(string nombre, HttpContextBase context, string key = null)
        {
            if(string.IsNullOrEmpty(key))
                return this.GetOrCreateValue<EditorEnunciadoModel>(key: "comun", 
                    () => this.LlenarModels(DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlEditorMensajesResource, $"lcase({SqlUtil.SurroundColumn("Nombre")}) = @nombre"),
                            new Dictionary<string, object>() { { "@nombre", nombre.ToLower() } },
                            context, 0))?.FirstOrDefault()
                    , nameof(ObtenerTexto), default(object[]));
            return this.GetOrCreateValue<EditorEnunciadoModel>(key: key, 
                () => this.LlenarModels(DataConnection.Instance.ExecuteQuery(
                        string.Format(SqlResource.SqlEditorMensajesResource, $"lcase({SqlUtil.SurroundColumn("Nombre")}) = @nombre"),
                        new Dictionary<string, object>() { { "@nombre", nombre.ToLower() } },
                        context, 0))?.FirstOrDefault()
                , nameof(ObtenerTexto), default(object[]));
        }

        public virtual void CrearOEditar(EditorEnunciadoModel modelo, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@nombre", modelo.Nombre);
            try
            {
                EditorEnunciadoModel dup = this.ObtenerTexto(modelo.Nombre, context);

                parameters.Add("@mensaje", modelo.Mensaje);
                modelo.UltimaModificacion = DateTime.Now;
                parameters.Add("@ultimaModificacion", modelo.UltimaModificacion);
                if (dup == null)
                {
                    DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlEditorMensajesInsert, parameters, context);

                    ulong id = (ulong)DataConnection.Instance.ExecuteScalar(SqlResource.SqlLastInsertedId, default(Dictionary<string, object>), context, transaction: null);
                    modelo.ID = (long)id;

                }
                else
                {
                    parameters.Add("@id", modelo.ID);
                    DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlEditorMensajesUpdate, parameters, context);
                }
                this.RemoveValueIfExists("comun", nameof(ObtenerTexto));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Protected
        protected virtual List<EditorEnunciadoModel> LlenarModels(IDataReader reader)
        {
            List<EditorEnunciadoModel> result = new List<EditorEnunciadoModel>();
            while (reader.Read())
            {
                EditorEnunciadoModel nc = new EditorEnunciadoModel();
                nc.Fill(reader);
                result.Add(nc);
            }
            return result;
        }
        #endregion
    }
}