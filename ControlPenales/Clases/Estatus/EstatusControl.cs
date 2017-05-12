using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases.Estatus
{
    public class EstatusControl
    {
        private List<Estatus> lista_estatus;
        public EstatusControl()
        {
            lista_estatus = new List<Estatus>();
            lista_estatus.Add(new Estatus
            {
                CLAVE = "S",
                DESCRIPCION = "ACTIVO"
            });
            lista_estatus.Add(new Estatus
            {
                CLAVE = "N",
                DESCRIPCION = "INACTIVO"
            });
        }

        public List<Estatus> LISTA_ESTATUS
        {
            get { return lista_estatus; }
        }
    }

    public class Estatus
    {
        public string CLAVE { get; set; }
        public string DESCRIPCION { get; set; }
    } 
}
