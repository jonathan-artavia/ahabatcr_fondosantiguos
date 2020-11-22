using Fondos_Antiguos.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fondos_Antiguos.Models
{
    public class CatalogoPaginadoModel<T> : PagingResult<T> where T : CatalogoModel, new ()
    {
        #region Filtros
        /// <summary>
        /// Operacion a aplicar a la fecha. 0 = diferente a, 1 = igual a, 2 = mayor que exclusivo, 4 = menor que exclusivo, 8 = mayor que inclusivo, 16 = menor que inclusivo, 32 = entre exclusivo, 64 = entre inclusivo
        /// </summary>
        [Display(ResourceType = typeof(ComunResource), Name = nameof(OperacionFecha))]
        [Required(AllowEmptyStrings = true)]
        public byte? OperacionFecha { get; set; }
        public OperacionEnum OperacionFechaType
        {
            get
            {
                return (OperacionEnum)this.OperacionFecha;
            }
        }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = @"{0:yyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicial { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = @"{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFinal { get; set; }


        /// <summary>
        /// Operacion a aplicar a la fecha. 0 = diferente a, 1 = igual a, 64 = contiene (entre inclusivo)
        /// </summary>
        [Display(ResourceType = typeof(ComunResource), Name = nameof(OperacionMateria))]
        [Required(AllowEmptyStrings = true)]
        public byte? OperacionMateria { get; set; }
        public OperacionEnum OperacionMateriaType
        {
            get
            {
                return (OperacionEnum)this.OperacionMateria;
            }
        }
        [DataType(DataType.Text)]
        public string FiltroMateria { get; set; }


        /// <summary>
        /// Operacion a aplicar a la fecha. 0 = diferente a, 1 = igual a, 64 = contiene (entre inclusivo)
        /// </summary>
        [Display(ResourceType = typeof(ComunResource), Name = nameof(OperacionLugar))]
        [Required(AllowEmptyStrings = true)]
        public byte? OperacionLugar { get; set; }
        public OperacionEnum OperacionLugarType
        {
            get
            {
                return (OperacionEnum)this.OperacionLugar;
            }
        }
        [DataType(DataType.Text)]
        public string FiltroLugar { get; set; }


        /// <summary>
        /// Operacion a aplicar a la fecha. 0 = diferente a, 1 = igual a
        /// </summary>
        [Display(ResourceType = typeof(ComunResource), Name = nameof(OperacionSerie))]
        [Required(AllowEmptyStrings = true)]
        public byte? OperacionSerie { get; set; }
        public OperacionEnum OperacionSerieType
        {
            get
            {
                return (OperacionEnum)this.OperacionSerie;
            }
        }
        [DataType(DataType.Text)]
        public long? FiltroSerie { get; set; }


        /// <summary>
        /// Operacion a aplicar a la fecha. 0 = diferente a, 1 = igual a, 64 = contiene(entre inclusivo)
        /// </summary>
        [Display(ResourceType = typeof(ComunResource), Name = nameof(OperacionFecha))]
        [Required(AllowEmptyStrings = true)]
        public byte? OperacionCaja { get; set; }
        public OperacionEnum OperacionCajaType
        {
            get
            {
                return (OperacionEnum)this.OperacionCaja;
            }
        }
        [DataType(DataType.Text)]
        public string FiltroCaja { get; set; }


        public string TextoPlano { get; set; }
        #endregion



        public byte? OrigenIncluido { get; set; }

        public byte? MessageType { get; set; }
        public string Message { get; set; }

        public string Exception { get; set; }
    }
}