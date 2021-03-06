using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fondos_Antiguos.Models
{
    public class HistCatalogoModel : CatalogoModel
    {
        public HistCatalogoModel() : base()
        {

        }
        public override void Fill(IDataReader dr, bool limitarStrings = false)
        {
            if (!dr.IsClosed && dr.FieldCount > 0)
            {
                DataTableReader read = (DataTableReader)dr;
                DateTime? fecha = Convert.IsDBNull(dr["Fecha"]) ? (DateTime?)null : (DateTime)read["Fecha"];
                string signatura = Convert.IsDBNull(dr["Signatura"]) ? null : dr["Signatura"].ToString();
                Match rg = Regex.Match(signatura, "Caja ([0-9]{2}),\\s?([^,]*[,]? )(F[s]?\\..*)");
                
                if(!limitarStrings)
                    this.Contenido = Convert.IsDBNull(dr["Descripcion"]) ? null : dr["Descripcion"].ToString();
                else if(!Convert.IsDBNull(dr["Descripcion"]) && dr["Descripcion"].ToString().Length > FaViewOptions.TamanoStringsVista)
                    this.Contenido = Convert.IsDBNull(dr["Descripcion"]) ? null : dr["Descripcion"].ToString().Substring(0, FaViewOptions.TamanoStringsVista).Trim() + "...";
                else
                    this.Contenido = Convert.IsDBNull(dr["Descripcion"]) ? null : dr["Descripcion"].ToString();
                this.Año = !fecha.HasValue ? (short?)null : (short)fecha.Value.Year;
                this.Fecha = fecha;
                this.Fichero = Convert.IsDBNull(dr["Fichero"]) ? null : dr["Fichero"].ToString();
                this.Folio = string.IsNullOrEmpty(rg.Groups[3].Value) ? null : rg.Groups[3].Value;
                this.ID = Convert.IsDBNull(dr["ID"]) ? 0 : Convert.ToInt64(dr["ID"]);
                this.IdSerie = null;
                this.HistMaterias = Convert.IsDBNull(dr["Materias"]) ? null : dr["Materias"].ToString();
                this.Libro = null;
                this.Lugar = Convert.IsDBNull(dr["Lugar"]) ? null : dr["Lugar"].ToString();
                this.Mes = !fecha.HasValue ? (byte?)null : (byte)fecha.Value.Month;
                this.NumCaja = string.IsNullOrEmpty(rg.Groups[1].Value) ? null : rg.Groups[1].Value;
                this.NumCarpeta = null;
                this.NumExpediente = null;
                this.NumTomo = null;
                this.Observaciones = Convert.IsDBNull(dr["Datos"]) ? null : dr["Datos"].ToString();
                this.Signatura = Convert.IsDBNull(dr["Signatura"]) ? null : dr["Signatura"].ToString();
                this.Origen = Convert.IsDBNull(dr["Hist"]) ? (byte)0 : (byte)Convert.ToInt32(dr["Hist"]);
                if(this.Origen == 1)
                {
                    this.Origen = 2;
                }
                else
                {
                    this.Origen = 0;
                }
            }
        }
    }
}
