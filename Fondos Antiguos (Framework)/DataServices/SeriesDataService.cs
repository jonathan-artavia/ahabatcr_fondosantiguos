using Fondos_Antiguos.Base;
using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Fondos_Antiguos.DataService
{
    public class SeriesDataService : BaseDataService
    {
        #region Constructor
        public SeriesDataService() : base()
        {

        }
        #endregion

        #region Public
        public virtual IEnumerable<SerieModel> ObtenerSeries(QueryExpresion expr, Dictionary<string, object> parameters, HttpContextBase context)
        {
            List<SerieModel> result = this.GetOrCreateValue<List<SerieModel>>(
                this.GetOrCreateKey(context),
                () => this.LlenarModels(DataConnection.Instance.ExecuteQuery(
                    string.Format(SqlResource.SqlSeriesResource, expr?.ToString() ?? "1=1"),
                    parameters,
                    context,0)),
                nameof(ObtenerSeries), expr?.ToString()
                );

            return result;
        }

        public virtual void Insertar(SerieModel model, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@series", model.Nombre);
            try
            {
                SerieModel dup = this.ObtenerSeries(new QueryExpresion($"lcase({SqlUtil.SurroundColumn("Nombre")}) = @nombre"), new Dictionary<string, object>() { { "@nombre", model.Nombre.ToLower() } }, context)?.FirstOrDefault();

                //buscar por duplicados
                if (dup != null)
                    throw new Exception(string.Format(CatalogoRes.SerieYaExisteFmt, dup.Nombre));

                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlSeriesInsert, parameters, context);
                ulong id = (ulong)DataConnection.Instance.ExecuteScalar(SqlResource.SqlLastInsertedId, default(Dictionary<string,object>), context, transaction: null);
                model.ID = (long)id;
                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerSeries));
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
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlSeriesDelete, parameters, context);

                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerSeries));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Protected
        protected virtual List<SerieModel> LlenarModels(IDataReader reader)
        {
            List<SerieModel> result = new List<SerieModel>();
            while (reader.Read())
            {
                SerieModel nc = new SerieModel();
                nc.Fill(reader);
                result.Add(nc);
            }
            return result;
        }
        #endregion
    }
}
