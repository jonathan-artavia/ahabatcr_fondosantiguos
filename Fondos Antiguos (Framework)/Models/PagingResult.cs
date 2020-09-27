using Fondos_Antiguos.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fondos_Antiguos.Models
{
    public class PagingResult<T> : IDisposable, IEnumerable<T>
        where T : class, new()
    {

        #region Properties
        public long PageTotal { get; set; }
        public long SelectedPage { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> PagedResult { get; set; }
        public long RecordTotal { get; set; }
        

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
        public DateTime? FechaInicial { get; set; }
        [DataType(DataType.Date)]
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
        #endregion



        public byte? OrigenIncluido { get; set; }

        public byte? MessageType { get; set; }
        public string Message { get; set; }

        public string Exception { get; set; }
        #endregion

        #region Constructor
        public PagingResult()
        {

        } 

        public PagingResult(long pageTotal, long selectedPage, IEnumerable<T> value, int pageSize = 20)
        {
            this.PagedResult = value;
            this.PageTotal = pageTotal;
            this.SelectedPage = selectedPage;
            this.PageSize = pageSize;
        }
        #endregion

        #region IDisposable members
        public void Dispose()
        {
            this.PagedResult = null;
        } 
        #endregion

        #region IEnumerable<T> members
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)this.PagedResult).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.PagedResult).GetEnumerator();
        }
        #endregion
    }
    #region Enums
    public enum OperacionEnum : byte
    {
        DiferenteA = 0,
        IgualA = 1,
        MayorQueExc = 2,
        MenorQueExc = 4,
        MayorQueInc = 8,
        MenorQueInc = 16,
        EntreExc = 32,
        EntreInc = 64
    }
    #endregion
}
