using Fondos_Antiguos.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fondos_Antiguos.Models
{
    public class LugarModel
    {
		#region Propiedades
		[Required]
		[HiddenInput]
		public long? ID { get; set; }

		[System.ComponentModel.DataAnnotations.DataType(DataType.Text)]
		[System.ComponentModel.DataAnnotations.MaxLength(512)]
		[Display(ResourceType = typeof(CatalogoRes), Name = "MateriaNombre")]
		public string Nombre { get; set; }

		/// <summary>
		/// 0 = Sin Cambios, 1 = Cambiado, 2 = Borrado, 3 = Nuevo
		/// </summary>
		[HiddenInput]
		public byte Estado { get; set; }
		#endregion


		#region "Constructors"
		public LugarModel()
		{

		}

		public LugarModel(long id, string nombre)
		{
			this.ID = id;
			this.Nombre = nombre;
		}
		#endregion "Constructors"

		#region Metodos
		public virtual void Fill(IDataReader dr)
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