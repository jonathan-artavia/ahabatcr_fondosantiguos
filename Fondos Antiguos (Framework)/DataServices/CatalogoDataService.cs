using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Base;
using Fondos_Antiguos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExcelDataReader;
using System.Management.Instrumentation;

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

        public virtual CatalogoModel GetCatalogo(QueryExpresion expr, Dictionary<string, object> parameters, bool paraEditar, byte includeHist, HttpContextBase context)
        {
            SeriesDataService ds = new SeriesDataService();
            var seriesList = ds.ObtenerSeries(null, null, context);
            IDataReader read;
            switch (includeHist)
            {
                case 0: //sin hist
                    read = DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlCatalogoResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context, 0);
                    if(read.Read())
                    {
                        CatalogoModel nc = new CatalogoModel();
                        nc.Fill(read);
                        if (seriesList.Count() > 0)
                            nc.SeriesNombre = seriesList.FirstOrDefault(sm => sm.ID == nc.IdSerie.GetValueOrDefault(0))?.Nombre;
                        nc.EstablecerSignaturaPorDefecto();
                        nc.ListaMaterias = !paraEditar ? this.ObtenerMateriasDeCatalogo(nc.ID, context) : null;
                        nc.ListaMateriasSeleccionables = paraEditar ? this.ObtenerMateriasSeleccionablesDeCatalogo(nc.ID, context) : null;
                        return nc;
                    }
                    break;
                case 1:
                    read = DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlCatalogoAmbosResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context,0);
                    if (read.Read())
                    {
                        int origen = read.GetInt32(read.GetOrdinal("Hist"));
                        CatalogoModel nc = new CatalogoModel();
                        nc.Fill(read);
                        if (seriesList.Count() > 0)
                            nc.SeriesNombre = seriesList.FirstOrDefault(sm => sm.ID == nc.IdSerie.GetValueOrDefault(0))?.Nombre;
                        nc.EstablecerSignaturaPorDefecto();
                        if(origen == 0)
                        {
                            nc.ListaMaterias = !paraEditar ? this.ObtenerMateriasDeCatalogo(nc.ID, context) : null;
                            nc.ListaMateriasSeleccionables = paraEditar ? this.ObtenerMateriasSeleccionablesDeCatalogo(nc.ID, context) : null;
                        }
                        return nc;
                    }
                    break;
                case 2:
                    read = DataConnection.Instance.ExecuteQuery(
                            string.Format(SqlResource.SqlHistCatalogoResource, expr?.ToString() ?? "1=1"),
                            parameters,
                            context,0);
                    
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
            IEnumerable<SerieModel> seriesList = ds.ObtenerSeries(null, null, context);
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

        public virtual CatalogoPaginadoModel<CatalogoModel> CrearModelosPaginados(QueryExpresion expr, Dictionary<string, object> parameters, long? page, byte includeHist, HttpContextBase context)
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
            return new CatalogoPaginadoModel<CatalogoModel>()
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
            parameters.Add("@Año", model.Año ?? model.Fecha?.Year);
            parameters.Add("@Mes", model.Mes ?? model.Fecha?.Month);
            parameters.Add("@FechaIngreso", model.FechaIngreso);
            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoInsert, parameters, context);
                model.ID = (long)((ulong)DataConnection.Instance.ExecuteScalar(SqlResource.SqlLastInsertedId, default(Dictionary<string, object>), context));
                foreach (var item in model.ListaMaterias)
                {
                    if (item.Estado == 3)
                        this.AsignarMateriaDeCatalogo(model.ID, item.ID.Value, context);
                }
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
            this.RemoveValueIfExists(this.GetOrCreateKey(context), "GetCatalogosCount");
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
            parameters.Add("@Año", model.Año ?? model.Fecha?.Year);
            parameters.Add("@Mes", model.Mes ?? model.Fecha?.Month);
            parameters.Add("@FechaIngreso", model.FechaIngreso);
            parameters.Add("@id", model.ID);
            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoActualizarResource, parameters, context);
                foreach (var item in model.ListaMateriasSeleccionables)
                {
                    if (item.Estado == 3)
                        this.AsignarMateriaDeCatalogo(model.ID, item.ID.Value, context);
                    if (item.Estado == 2)
                        this.EliminarMateriasDeCatalogo(model.ID, item.ID.Value, context);
                }
                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(GetCatalogos));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Materias
        public virtual List<MateriaModel> ObtenerMateriasDeCatalogo(long catalogo_id, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@catalogoId", catalogo_id);
            List<MateriaModel> result = this.GetOrCreateValue<List<MateriaModel>>(
                this.GetOrCreateKey(context),
                () => this.LlenarMateriasDelCatalogo(DataConnection.Instance.ExecuteQuery(
                    SqlResource.SqlCatalogoMateriasResource,
                    parameters,
                    context, 0), catalogo_id),
                nameof(ObtenerMateriasDeCatalogo), catalogo_id
                );

            return result;
        }

        public virtual List<SeleccionableMateriaModel> ObtenerMateriasSeleccionablesDeCatalogo(long catalogo_id, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@catalogoId", catalogo_id);
            List<SeleccionableMateriaModel> result = this.GetOrCreateValue<List<SeleccionableMateriaModel>>(
                this.GetOrCreateKey(context),
                () => this.LlenarMateriasSeleccionablesDelCatalogo(DataConnection.Instance.ExecuteQuery(
                    SqlResource.SqlCatalogoMateriasResource,
                    parameters,
                    context, 0), catalogo_id),
                nameof(ObtenerMateriasDeCatalogo), catalogo_id
                );

            return result;
        }

        public virtual void EliminarMateriasDeCatalogo(long catalogo_id, long materia_id, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@catalogoId", catalogo_id);
            parameters.Add("@materiaId", materia_id);
            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoMateriasEliminar, parameters, context);

                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerMateriasDeCatalogo), catalogo_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void EliminarMateriasDeCatalogo(long catalogo_id, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@catalogoId", catalogo_id);
            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoMateriasEliminarTodo, parameters, context);

                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerMateriasDeCatalogo), catalogo_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void AsignarMateriaDeCatalogo(long catalogo_id, long materia_id, HttpContextBase context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@catalogoId", catalogo_id);
            parameters.Add("@materiaId", materia_id);
            try
            {
                DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoMateriasInsertar, parameters, context);
                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(ObtenerMateriasDeCatalogo), catalogo_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Lotes
        /// <summary>
        /// Convertir el DataReader y la convierte en una lista del tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> ConvertirLoteDataReader<T>(IDataReader reader, HttpContextBase context) where T : CatalogoModel, new()
        {
            IEnumerable<SerieModel> seriesList = new SeriesDataService().ObtenerSeries(null, null, context);
            IEnumerable<MateriaModel> materiasList = new MateriasDataService().ObtenerMaterias(null, null, context);
            List<T> modelos = new List<T>();
            DataSet ds = ((ExcelDataReader.IExcelDataReader)reader).AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (rowReader) =>
                    new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
            });
            using (DataTableReader dsReader = ds.Tables[0].CreateDataReader())
                modelos = this.FillCatalogoListExcel<T>(dsReader, new SeriesDataService(), seriesList, new MateriasDataService(), materiasList, false, context);
            //this.InsertarLote(modelos, context);
            return modelos;
        }

        /// <summary>
        /// Inserta la lista de <paramref name="modelos"/> en memoria o en la base de datos, dependiendo del valor en <paramref name="persistir"/>.
        /// </summary>
        /// <param name="modelos"></param>
        /// <param name="persistir"></param>
        /// <param name="context"></param>
        public virtual void InsertarLote(string archivoId, IEnumerable<SeleccionableCatalogoModel> modelos, bool persistir, HttpContextBase context)
        {
            if (persistir)
            {
                MySql.Data.MySqlClient.MySqlTransaction trans = DataConnection.Instance.BeginTransaction(context);
                foreach (SeleccionableCatalogoModel model in modelos)
                {
                    if (model.Seleccionado)
                        continue;

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
                        DataConnection.Instance.ExecuteNonQuery(SqlResource.SqlCatalogoInsert, parameters, context, trans);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                DataConnection.Instance.CommitTransaction(trans);
                this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(GetCatalogos));
            }
            else
            {
                base.GetOrCreateValue<IEnumerable<SeleccionableCatalogoModel>>(base.GetOrCreateKey(context), () => modelos, nameof(GetCatalogoSinRevisar), archivoId);
            }
        }

        /// <summary>
        /// Actualiza la lista de catalogos insertados por lote que estan en memoria.
        /// </summary>
        /// <param name="modelos"></param>
        /// <param name="persistir"></param>
        /// <param name="context"></param>
        public virtual void ActualizarLote(string archivoId, IEnumerable<SeleccionableCatalogoModel> modelos, HttpContextBase context)
        {
            base.RemoveValueIfExists(base.GetOrCreateKey(context), nameof(GetCatalogoSinRevisar));
            base.GetOrCreateValue<IEnumerable<SeleccionableCatalogoModel>>(base.GetOrCreateKey(context), () => modelos, nameof(GetCatalogoSinRevisar), archivoId);
        }

        public virtual IEnumerable<SeleccionableCatalogoModel> GetCatalogoSinRevisar(string archivoId, int? page, int total, HttpContextBase context)
        {
            return base.GetOrCreateValue<IEnumerable<SeleccionableCatalogoModel>>(base.GetOrCreateKey(context), nameof(GetCatalogoSinRevisar), archivoId)?
                .Skip(1 + (FaViewOptions.RegistrosPorPagina * page.GetValueOrDefault(0)) - FaViewOptions.RegistrosPorPagina)
                .Take(FaViewOptions.RegistrosPorPagina).ToList();
        }

        public virtual IEnumerable<SeleccionableCatalogoModel> GetCatalogoSinRevisar(string archivoId, HttpContextBase context)
        {
            return base.GetOrCreateValue<IEnumerable<SeleccionableCatalogoModel>>(base.GetOrCreateKey(context), nameof(GetCatalogoSinRevisar), archivoId);
        }

        public virtual int GetCantidadCatalogoSinRevisar(string archivoId, HttpContextBase context)
        {
            return base.GetOrCreateValue<IEnumerable<SeleccionableCatalogoModel>>(base.GetOrCreateKey(context), nameof(GetCatalogoSinRevisar), archivoId)?.Count() ?? 0;
        }

        public virtual RevisionCatalogoPaginadoModel CrearModelosRevisionPaginados(string archivoId, int? page, int conteoPagina, int totalRegistros, HttpContextBase context)
        {
            long count = totalRegistros;

            return new RevisionCatalogoPaginadoModel()
            {
                PageTotal = (count + (FaViewOptions.RegistrosPorPagina - 3)) / FaViewOptions.RegistrosPorPagina,
                RecordTotal = count,
                PageSize = conteoPagina < FaViewOptions.RegistrosPorPagina ? conteoPagina : FaViewOptions.RegistrosPorPagina
            };
        }

        public virtual void DescartarRevision(string archivoId, HttpContextBase context)
        {
            this.RemoveValueIfExists(this.GetOrCreateKey(context), nameof(GetCatalogoSinRevisar), archivoId);
        }
        #endregion
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

        protected virtual List<T> FillCatalogoListExcel<T>(IDataReader reader, SeriesDataService serieDs , IEnumerable<SerieModel> seriesList, MateriasDataService materiaDs, IEnumerable<MateriaModel> materiasList, bool limitarStrings, HttpContextBase context) where T : CatalogoModel, new()
        {
            List<T> result = new List<T>();
            long count = 0;
            while (reader.Read())
            {
                T nc = new T();
                //--------------------------\\
                nc.Contenido = Convert.IsDBNull(reader["Contenido"]) ? null : reader["Contenido"].ToString();
                nc.Año = Convert.IsDBNull(reader["Año"]) ? (short?)null : Convert.ToInt16(reader["Año"]); //Año
                nc.Fecha = Convert.IsDBNull(reader["Fecha"]) ? (DateTime?)null : (DateTime)reader["Fecha"];
                nc.Folio = Convert.IsDBNull(reader["Folio"]) ? null : reader["Folio"].ToString();

                if (seriesList.Count() > 0)
                {
                    SerieModel serie = seriesList.FirstOrDefault(sm => sm.Nombre.Trim().Equals(reader["Series"].ToString().Trim()));
                    if (serie == null)
                    {
                        serie = new SerieModel() { Nombre = reader["Series"].ToString() };
                        serieDs.Insertar(serie, context);
                    }
                    nc.IdSerie = serie?.ID;
                    nc.SeriesNombre = serie?.Nombre;
                }

                if(materiasList.Count() > 0)
                {
                    string materiaExt = Convert.IsDBNull(reader["Materias"]) ? null : reader["Materias"].ToString();
                    if (!string.IsNullOrEmpty(materiaExt))
                    {
                        nc.ListaMaterias = new List<MateriaModel>();
                        string[] valoresSeparados = null;
                        if(materiaExt.Contains(" / "))
                        {
                            valoresSeparados = materiaExt.Split(new string[] { " / " }, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                        {
                            valoresSeparados = new string[] { materiaExt };
                        }
                        foreach (string item in valoresSeparados)
                        {
                            MateriaModel materia = materiasList.FirstOrDefault(m => m.Nombre.Trim().Equals(item.Trim()));
                            if(materia == null)
                            {
                                materia = new MateriaModel() { Nombre = item };
                                materiaDs.Insertar(materia, context);
                            }
                            nc.ListaMaterias.Add(materia);
                        }
                    }
                }

                nc.Libro = Convert.IsDBNull(reader["Libro"]) ? (int?)null : Convert.ToInt32(reader["Libro"]);
                nc.Lugar = Convert.IsDBNull(reader["Lugar"]) ? null : reader["Lugar"].ToString();
                nc.NumCaja = Convert.IsDBNull(reader["Caja"]) ? null : reader["Caja"].ToString();
                nc.NumCarpeta = Convert.IsDBNull(reader["Carpeta"]) ? (int?)null : Convert.ToInt32(reader["Carpeta"]);
                nc.NumExpediente = Convert.IsDBNull(reader["Expediente"]) ? null : reader["Expediente"].ToString();
                nc.NumTomo = Convert.IsDBNull(reader["Tomo"]) ? (int?)null : Convert.ToInt32(reader["Tomo"]);
                nc.Observaciones = Convert.IsDBNull(reader["Observaciones"]) ? null : reader["Observaciones"].ToString();
                //nc.Materias = Convert.IsDBNull(reader["Materias"]) ? null : reader["Materias"].ToString();
                nc.Origen = 1;
                nc.ID = count++;
                //--------------------------\\
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

        protected virtual List<MateriaModel> LlenarMateriasDelCatalogo(IDataReader reader, long catalogo_id)
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

        protected virtual List<SeleccionableMateriaModel> LlenarMateriasSeleccionablesDelCatalogo(IDataReader reader, long catalogo_id)
        {
            List<SeleccionableMateriaModel> result = new List<SeleccionableMateriaModel>();
            while (reader.Read())
            {
                SeleccionableMateriaModel nc = new SeleccionableMateriaModel();
                nc.Fill(reader);
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
                this.EliminarMateriasDeCatalogo(id, context);
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
