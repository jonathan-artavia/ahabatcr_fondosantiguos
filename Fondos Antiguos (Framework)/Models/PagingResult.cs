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
        //public string Busqueda { get; set; }

        /// <summary>
        /// Operacion a aplicar a la fecha. 0 = diferente a, 1 = igual a, 2 = mayor que exclusivo, 4 = menor que exclusivo, 8 = mayor que inclusivo, 16 = menor que inclusivo, 32 = entre exclusivo, 64 = entre inclusivo
        /// </summary>
        [Display(ResourceType = typeof(ComunResource), Name = nameof(Operacion))]
        [Required(AllowEmptyStrings = true)]
        public byte? Operacion { get; set; }
        public OperacionEnum OperacionType
        {
            get
            {
                return (OperacionEnum)this.Operacion;
            }
        }
        [DataType(DataType.Date)]
        public DateTime? FechaInicial { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FechaFinal { get; set; }
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
