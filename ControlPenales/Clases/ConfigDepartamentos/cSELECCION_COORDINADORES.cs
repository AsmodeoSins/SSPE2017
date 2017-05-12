using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases.ConfigDepartamentos
{
    public class cSELECCION_COORDINADORES : ValidationViewModelBase
    {
        private int _ID_EMPLEADO;

        public int ID_EMPLEADO
        {
            get { return _ID_EMPLEADO; }
            set { _ID_EMPLEADO = value; OnPropertyChanged("ID_EMPLEADO"); }
        }
        private string _COORDINADOR_NOMBRE;

        public string COORDINADOR_NOMBRE
        {
            get { return _COORDINADOR_NOMBRE; }
            set { _COORDINADOR_NOMBRE = value; OnPropertyChanged("COORDINADOR_NOMBRE"); }
        }

    }
}
