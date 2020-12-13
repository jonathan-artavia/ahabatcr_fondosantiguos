using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Fondos_Antiguos.Models
{
    public class SeleccionableMateriaModel : MateriaModel
    {
        public bool Seleccionado { get; set; }

        public override void Fill(IDataReader dr)
        {
            if (!dr.IsClosed && dr.FieldCount > 0)
            {
                this.ID = Convert.IsDBNull(dr["Materia_ID"]) ? 0 : Convert.ToInt64(dr["Materia_ID"]);

                this.Nombre = Convert.IsDBNull(dr["Nombre"]) ? null : dr["Nombre"].ToString();
            }
        }
    }
}