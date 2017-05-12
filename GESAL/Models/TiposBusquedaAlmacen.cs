using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GESAL.Models
{
    public class TiposBusquedaAlmacen
    {
        private List<TipoBusquedaAlmacen> lista_tipobusquedaalmacen;
        public TiposBusquedaAlmacen()
        {
            lista_tipobusquedaalmacen = new List<TipoBusquedaAlmacen>();
            lista_tipobusquedaalmacen.Add(new TipoBusquedaAlmacen
            {
                CLAVE = 0,
                DESCRIPCION = "TODOS"
            });
            lista_tipobusquedaalmacen.Add(new TipoBusquedaAlmacen
            {
                CLAVE = 1,
                DESCRIPCION = "INDIVIDUAL"
            });
        }

        public List<TipoBusquedaAlmacen> LISTA_TIPOBUSQUEDAALMACEN
        {
            get { return lista_tipobusquedaalmacen; }
        }
    }

    public class TipoBusquedaAlmacen
    {
        public short CLAVE { get; set; }
        public string DESCRIPCION { get; set; }
    }
}
