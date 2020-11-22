using Fondos_Antiguos.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fondos_Antiguos.Models
{
    public class RevisionCatalogoPaginadoModel : PagingResult<SeleccionableCatalogoModel>
    {
        #region Properties
        [Display(ResourceType = typeof(CatalogoRes), Name = "Revision_FechaVencimiento")]
        public virtual DateTime FechaVencimiento { get; set; }

        public virtual List<SeleccionableCatalogoModel> Registros
        {
            get
            {
                return this.PagedResult as List<SeleccionableCatalogoModel>;
            }
            set
            {
                this.PagedResult = value;
            }
        }

        public virtual string ArchivoId { get; set; }

        [HiddenInput]
        public virtual long PaginaActual { get; set; }

        [HiddenInput]
        public virtual bool DescartarSN { get; set; }
        #endregion

        #region Constructor
        public RevisionCatalogoPaginadoModel() : base()
        {
            this.DescartarSN = false;
        }
        #endregion
    }
}