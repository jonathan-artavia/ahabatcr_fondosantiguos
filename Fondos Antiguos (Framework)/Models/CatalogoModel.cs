using Fondos_Antiguos.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fondos_Antiguos.Models
{
    /// <summary>
    /// Object representation of the table 'tblCatalogo'
    /// </summary>
    public class CatalogoModel : IDisposable
    {
        #region Constructor
        public CatalogoModel()
        {
            this.FechaIngreso = DateTime.Now;
        }
        #endregion

        #region Public
        public virtual void Fill(IDataReader dr, bool limitarStrings = false)
        {
            if (!dr.IsClosed && dr.FieldCount > 0)
            {
                if(!limitarStrings)
                    this.Contenido = Convert.IsDBNull(dr["Contenido"]) ? null : dr["Contenido"].ToString();
                else if (!Convert.IsDBNull(dr["Contenido"]) && dr["Contenido"].ToString().Length > FaViewOptions.TamanoStringsVista)
                    this.Contenido = Convert.IsDBNull(dr["Contenido"]) ? null : dr["Contenido"].ToString().Substring(0, FaViewOptions.TamanoStringsVista).Trim() + "...";
                else
                    this.Contenido = Convert.IsDBNull(dr["Contenido"]) ? null : dr["Contenido"].ToString();
                this.Año = Convert.IsDBNull(dr["AÃ±o"]) ? (short?)null : Convert.ToInt16(dr["AÃ±o"]); //Año
                this.Fecha = Convert.IsDBNull(dr["Fecha"]) ? (DateTime?)null : (DateTime)dr["Fecha"];
                this.Fichero = Convert.IsDBNull(dr["Fichero"]) ? null : dr["Fichero"].ToString();
                this.Folio = Convert.IsDBNull(dr["Folio"]) ? null : dr["Folio"].ToString();
                this.ID = Convert.IsDBNull(dr["ID"]) ? 0 : Convert.ToInt64(dr["ID"]);
                this.IdSerie = Convert.IsDBNull(dr["IdSerie"]) ? (long?)null : Convert.ToInt64(dr["IdSerie"]);
                this.Libro = Convert.IsDBNull(dr["Libro"]) ? (int?)null : Convert.ToInt32(dr["Libro"]);
                this.Lugar = Convert.IsDBNull(dr["Lugar"]) ? null : dr["Lugar"].ToString();
                this.Mes = Convert.IsDBNull(dr["Mes"]) ? (byte?)null : Convert.ToByte(dr["Mes"]);
                this.NumCaja = Convert.IsDBNull(dr["NumCaja"]) ? null : dr["NumCaja"].ToString();
                this.NumCarpeta = Convert.IsDBNull(dr["NumCarpeta"]) ? (int?)null : Convert.ToInt32(dr["NumCarpeta"]);
                this.NumExpediente = Convert.IsDBNull(dr["NumExpediente"]) ? null : dr["NumExpediente"].ToString();
                this.NumTomo = Convert.IsDBNull(dr["NumTomo"]) ? (int?)null : Convert.ToInt32(dr["NumTomo"]);
                this.Observaciones = Convert.IsDBNull(dr["Observaciones"]) ? null : dr["Observaciones"].ToString();
                this.Signatura = Convert.IsDBNull(dr["Signatura"]) ? null : dr["Signatura"].ToString();
                this.Materias = Convert.IsDBNull(dr["Materias"]) ? null : dr["Materias"].ToString();
                this.Origen = 1;
            }
        }

        public virtual void EstablecerSignaturaPorDefecto()
        {
            List<string> parts = new List<string>();
            if(!string.IsNullOrEmpty(this.NumCaja))
                parts.Add(string.Format(CatalogoRes.FormatoCajaSignatura, this.NumCaja));
            if(this.NumTomo.HasValue)
                parts.Add(string.Format(CatalogoRes.FormatoTomoSignatura, this.NumTomo));
            if(!string.IsNullOrEmpty(this.Folio))
                parts.Add(string.Format(CatalogoRes.FormatoFolioSignatura, this.Folio));
            if(!string.IsNullOrEmpty(this.NumExpediente))
                parts.Add(string.Format(CatalogoRes.FormatoExpedienteSignatura, this.NumExpediente));

            if(parts.Count > 0)
            {
                this.Signatura = string.Join(", ", parts);
            }
        }
        #endregion

        #region Protected
        #endregion

        #region IDisposable members
        public virtual void Dispose()
        {
            this.Fecha = null;
            this.IdSerie = null;
        }
        #endregion

        #region Properties
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Contenido))]
        public string Contenido { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(ID))]
        public long ID { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Fecha))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = @"{0:yyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Fecha { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Signatura))]
        public string Signatura { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Observaciones))]
        public string Observaciones { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(IdSerie))]
        public long? IdSerie { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Fichero))]
        public string Fichero { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(NumCaja))]
        public string NumCaja { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(NumTomo))]
        public int? NumTomo { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Folio))]
        public string Folio { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Libro))]
        public int? Libro { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(NumExpediente))]
        public string NumExpediente { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(NumCarpeta))]
        public int? NumCarpeta { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Lugar))]
        public string Lugar { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Año))]
        public short? Año { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Mes))]
        public byte? Mes { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(FechaIngreso))]
        public DateTime FechaIngreso { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(SeriesNombre))]
        public string SeriesNombre { get; set; }
        [Display(ResourceType = typeof(CatalogoRes), Name = nameof(Materias))]
        public string Materias { get; set; }
        public byte Origen { get; set; }
        public string Exception { get; set; }
        public List<SerieModel> Series { get; set; }
        #endregion
    }
}
