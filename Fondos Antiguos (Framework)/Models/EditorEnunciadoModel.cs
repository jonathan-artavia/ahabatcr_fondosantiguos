using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Fondos_Antiguos.Localization;

namespace Fondos_Antiguos.Models
{
    public class EditorEnunciadoModel
    {
        #region Constructor
        public EditorEnunciadoModel()
        {

        }
        #endregion

        #region Public
        public virtual void Fill(IDataReader reader)
        {
            if (!reader.IsClosed && reader.FieldCount > 0)
            {
                this.ID = Convert.IsDBNull(reader["ID"]) ? 0 : Convert.ToInt64(reader["ID"]);

                this.Nombre = Convert.IsDBNull(reader["Nombre"]) ? null : reader["Nombre"].ToString();

                this.Mensaje = Convert.IsDBNull(reader["Mensaje"]) ? null : reader["Mensaje"].ToString();

                this.UltimaModificacion = (DateTime)reader["UltimaModificacion"];
            }
        }
        #endregion

        #region Propiedades
        
        public long ID { get; set; }
        public string Nombre { get; set; }

        [AllowHtml]
        public string Mensaje { get; set; }

        [Display(ResourceType = typeof(EditorResource), Name = nameof(UltimaModificacion))]
        [DisplayFormat(DataFormatString = @"{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UltimaModificacion { get; set; }
        #endregion
    }
}