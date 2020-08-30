using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Base;
using Fondos_Antiguos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Fondos_Antiguos.DataService
{
    public class CatalogoDataService : BaseDataService
    {

        #region Constructor
        public CatalogoDataService()
        {

        }
        #endregion

        #region Public

        public virtual CatalogoModel GetCatalogo(QueryExpresion expr, Dictionary<string, object> parameters, byte includeHist, HttpContextBase context)
        {
            SeriesDataService ds = new SeriesDataService();
            var seriesList = ds.GetSeries(null, null, context);
            IDataReader read;
            switch (includeHist)
            {
                case 0: //sin hist
                    read = this.GetOrCreateValue<IDataReader>(
                        this.GetOrCreateKey(context),
                        () => DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlCatalogoResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context)
                        , "GetCatalogo", expr?.ToString(), 0
                        );
                    if(read.Read())
                    {
                        CatalogoModel nc = new CatalogoModel();
                        nc.Fill(read);
                        if (seriesList.Count() > 0)
                            nc.SeriesNombre = seriesList.FirstOrDefault(sm => sm.ID == nc.IdSerie.GetValueOrDefault(0))?.Nombre;
                        nc.EstablecerSignaturaPorDefecto();
                        return nc;
                    }
                    break;
                case 1:
                    read = this.GetOrCreateValue<IDataReader>(
                        this.GetOrCreateKey(context),
                        () => DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlCatalogoAmbosResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context)
                        , "GetCatalogo", expr?.ToString(), 1
                        );
                    if (read.Read())
                    {
                        CatalogoModel nc = new CatalogoModel();
                        nc.Fill(read);
                        if (seriesList.Count() > 0)
                            nc.SeriesNombre = seriesList.FirstOrDefault(sm => sm.ID == nc.IdSerie.GetValueOrDefault(0))?.Nombre;
                        nc.EstablecerSignaturaPorDefecto();
                        return nc;
                    }
                    break;
                case 2:
                    read = this.GetOrCreateValue<IDataReader>(
                        this.GetOrCreateKey(context),
                        () => DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlHistCatalogoResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context),
                        "GetCatalogo", expr?.ToString(), 2
                        );
                    
                    if (read.Read())
                    {
                        HistCatalogoModel nc = new HistCatalogoModel();
                        nc.Fill(read);
                        if (seriesList.Count() > 0)
                            nc.SeriesNombre = seriesList.FirstOrDefault(sm => sm.ID == nc.IdSerie.GetValueOrDefault(0))?.Nombre;
                        nc.EstablecerSignaturaPorDefecto();
                        return nc;
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="parameters"></param>
        /// <param name="page"></param>
        /// <param name="includeHist">0 = without Hist; 1 = with Hist; 2 = only Hist</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IEnumerable<CatalogoModel> GetCatalogos(QueryExpresion expr, Dictionary<string,object> parameters, long? page, byte includeHist, bool limitarStrings, HttpContextBase context)
        {
            SeriesDataService ds = new SeriesDataService();
            IEnumerable<SerieModel> seriesList = ds.GetSeries(null, null, context);
            List<CatalogoModel> read = new List<CatalogoModel>();
            switch (includeHist)
            {
                case 0: //sin hist
                    read = this.GetOrCreateValue<List<CatalogoModel>>(
                        this.GetOrCreateKey(context),
                        () => this.FillCatalogoList(DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlCatalogoResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context,
                            page.GetValueOrDefault(1)), seriesList, limitarStrings)
                        , "GetCatalogos", expr?.ToString(), page.GetValueOrDefault(1), 0
                        );
                    break;
                case 1: //con hist
                    read = this.GetOrCreateValue(
                        this.GetOrCreateKey(context),
                        () => this.FillCatalogoList(DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlCatalogoAmbosResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context,
                            page.GetValueOrDefault(1)), seriesList, limitarStrings)
                        , "GetCatalogos", expr?.ToString(), page.GetValueOrDefault(1), 1
                        );
                    break;
                case 2: //solo hist
                    read = this.GetOrCreateValue<List<HistCatalogoModel>>(
                        this.GetOrCreateKey(context),
                        () => this.FillHistCatalogoList(DataConnection.Instance.ExecuteQuery( string.Format(SqlResource.SqlHistCatalogoResource, expr?.ToString() ?? "1=1"), parameters, context, page.GetValueOrDefault(1)), seriesList, limitarStrings)
                        , "GetCatalogos", expr?.ToString(), page.GetValueOrDefault(1), 2
                        ).Cast<CatalogoModel>().ToList();
                    break;
            }
            return read;
        }

        public virtual PagingResult<CatalogoModel> CrearPagingResult(QueryExpresion expr, Dictionary<string, object> parameters, long? page, byte includeHist, HttpContextBase context)
        {
            long count = 0;
            //long count = 0; this.GetOrCreateValue<long>(
            //    this.GetOrCreateKey(context, "GetCatalogosPageCount", expr?.ToString()),
            //    () => (long)DataConnection.Instance.ExecuteScalar(
            //        string.Format(Fondos_Antiguos.AddResources.SqlResource.SqlCatalogoAmbosCountResource, expr?.ToString() ?? "1=1"),
            //        parameters,
            //        context)
            //    );
            switch (includeHist)
            {
                case 0: //sin hist
                    count = this.GetOrCreateValue<long>(
                        this.GetOrCreateKey(context),
                        () => Convert.ToInt64(DataConnection.Instance.ExecuteScalar(string.Format(SqlResource.SqlCatalogoCountResource, expr?.ToString() ?? "1=1"), parameters, context))
                        , "GetCatalogosCount", expr?.ToString(), page.GetValueOrDefault(1), 0
                        );
                    break;
                case 1:
                    count = this.GetOrCreateValue<long>(
                        this.GetOrCreateKey(context),
                        () => Convert.ToInt64(DataConnection.Instance.ExecuteScalar(
                            string.Format(SqlResource.SqlCatalogoAmbosCountResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context))
                        , "GetCatalogosCount", expr?.ToString(), page.GetValueOrDefault(1), 1
                        );
                    break;
                case 2:
                    count = this.GetOrCreateValue<long>(
                        this.GetOrCreateKey(context),
                        () => Convert.ToInt64(DataConnection.Instance.ExecuteScalar(
                            string.Format(SqlResource.SqlHistCatalogoCountResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context))
                        , "GetCatalogosCount", expr?.ToString(), page.GetValueOrDefault(1), 2
                        );
                    break;
            }
            return new PagingResult<CatalogoModel>()
            {
                PageTotal = (count + (FaViewOptions.RegistrosPorPagina - 3)) / FaViewOptions.RegistrosPorPagina,
                RecordTotal = count
            };
        }

        public virtual void Insertar(CatalogoModel model, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@contenido", model.Contenido);
            parameters.Add("@Fecha", model.Fecha);
            parameters.Add("@Signatura", model.Signatura);
            parameters.Add("@Observaciones", model.Observaciones);
            parameters.Add("@IdSerie", model.IdSerie);
            parameters.Add("@Fichero", model.Fichero);
            parameters.Add("@NumCaja", model.NumCaja);
            parameters.Add("@NumTomo", model.NumTomo);
            parameters.Add("@Folio", model.Folio);
            parameters.Add("@Libro", model.Libro);
            parameters.Add("@NumExpediente", model.NumExpediente);
            parameters.Add("@NumCarpeta", model.NumCarpeta);
            parameters.Add("@Lugar", model.Lugar);
            parameters.Add("@Año", model.Año);
            parameters.Add("@Mes", model.Mes);
            parameters.Add("@FechaIngreso", model.FechaIngreso);
            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoInsert, parameters, context);
                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(GetCatalogos));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Eliminar(long id, byte origen, HttpContextBase context)
        {
            if (origen == 1)
                this.EliminarNuevas(id, context);
            else
                this.EliminarHist(id, context);
            this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(GetCatalogos));
        }

        public virtual void Actualizar(CatalogoModel model, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@contenido", model.Contenido);
            parameters.Add("@Fecha", model.Fecha);
            parameters.Add("@Signatura", model.Signatura);
            parameters.Add("@Observaciones", model.Observaciones);
            parameters.Add("@IdSerie", model.IdSerie);
            parameters.Add("@Fichero", model.Fichero);
            parameters.Add("@NumCaja", model.NumCaja);
            parameters.Add("@NumTomo", model.NumTomo);
            parameters.Add("@Folio", model.Folio);
            parameters.Add("@Libro", model.Libro);
            parameters.Add("@NumExpediente", model.NumExpediente);
            parameters.Add("@NumCarpeta", model.NumCarpeta);
            parameters.Add("@Lugar", model.Lugar);
            parameters.Add("@Año", model.Año);
            parameters.Add("@Mes", model.Mes);
            parameters.Add("@FechaIngreso", model.FechaIngreso);
            parameters.Add("@id", model.ID);
            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoActualizarResource, parameters, context);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Protected
        protected virtual List<CatalogoModel> FillCatalogoList(IDataReader reader, IEnumerable<SerieModel> seriesList, bool limitarStrings)
        {
            List<CatalogoModel> result = new List<CatalogoModel>();
            while (reader.Read())
            {
                CatalogoModel nc = new CatalogoModel();
                nc.Fill(reader, limitarStrings);
                if (seriesList.Count() > 0)
                    nc.SeriesNombre = seriesList.FirstOrDefault(sm => sm.ID == nc.IdSerie.GetValueOrDefault(0))?.Nombre;
                result.Add(nc);
            }
            return result;
        }

        protected virtual List<HistCatalogoModel> FillHistCatalogoList(IDataReader reader, IEnumerable<SerieModel> seriesList, bool limitarStrings)
        {
            List<HistCatalogoModel> result = new List<HistCatalogoModel>();
            while (reader.Read())
            {
                HistCatalogoModel nc = new HistCatalogoModel();
                nc.Fill(reader, limitarStrings);
                if (seriesList.Count() > 0)
                    nc.SeriesNombre = seriesList.FirstOrDefault(sm => sm.ID == nc.IdSerie.GetValueOrDefault(0))?.Nombre;
                result.Add(nc);
            }
            return result;
        }

        protected virtual void EliminarNuevas(long id, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", id);

            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoEliminar, parameters, context);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual void EliminarHist(long id, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", id);

            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlHistCatalogoEliminar, parameters, context);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Properties

        #endregion
    }
}
