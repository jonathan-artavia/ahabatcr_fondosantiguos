using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Fondos_Antiguos.Models
{
    public class SerieModel
    {

        #region Properties
        public long ID { get; set; }
        public string Nombre { get; set; }
        #endregion

        #region public
        public void Fill(IDataReader dr)
        {
            if (!dr.IsClosed && dr.FieldCount > 0)
            {
                this.ID = Convert.IsDBNull(dr["ID"]) ? 0 : Convert.ToInt64(dr["ID"]);
                
                this.Nombre = Convert.IsDBNull(dr["Nombre"]) ? null : dr["Nombre"].ToString();
            }
        }
        #endregion
    }
}
