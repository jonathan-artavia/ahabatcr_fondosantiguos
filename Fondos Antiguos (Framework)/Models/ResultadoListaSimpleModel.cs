using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fondos_Antiguos.Models
{
    public class ResultadoListaSimpleModel<T> : IEnumerable<T>
    {
        #region Propiedades
        public virtual IEnumerable<T> ListaBase { get; set; }
        [HiddenInput]
        public virtual string Mensaje { get; set; }

        /// <summary>
        /// Tipo de Mensaje. 0 = Success, 1 = Danger, null = sin mensaje.
        /// </summary>
        [HiddenInput]
        public virtual byte? TipoMensaje { get; set; }
        #endregion

        #region Constructores
        public ResultadoListaSimpleModel(IEnumerable<T> listaBase)
        {
            this.ListaBase = listaBase;
        }
        #endregion

        #region IEnumerable<T>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.ListaBase?.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.ListaBase as IEnumerable)?.GetEnumerator();
        } 
        #endregion
    }
}