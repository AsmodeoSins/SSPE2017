using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class Tipo_Traslado
    {
        public Tipo_Traslado(string _id, string _nombre)
        {
            id = _id;
            nombre = _nombre;
        }

        private string id;
        public string ID {
            get { return id; }
            set { id = value; }
        }
        private string nombre;
        public string NOMBRE {
            get { return nombre; }
            set { nombre = value; }
        }
    }
}
