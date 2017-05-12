using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases.ConfigDepartamentos
{
    public class cCOORDINADORESASIGNADOS : ValidationViewModelBase
    {
        private DEPARTAMENTO_ACCESO _OBJETO_DEPARTAMENTO_ACCESO;

        public DEPARTAMENTO_ACCESO OBJETO_DEPARTAMENTO_ACCESO
        {
            get { return _OBJETO_DEPARTAMENTO_ACCESO; }
            set { _OBJETO_DEPARTAMENTO_ACCESO = value; OnPropertyChanged("OBJETO_DEPARTAMENTO_ACCESO"); }
        }


        private string _NOMBRE_COORDINADOR;

        public string NOMBRE_COORDINADOR
        {
            get { return _NOMBRE_COORDINADOR; }
            set { _NOMBRE_COORDINADOR = value; OnPropertyChanged("NOMBRE_COORDINADOR"); }
        }

    }
}
