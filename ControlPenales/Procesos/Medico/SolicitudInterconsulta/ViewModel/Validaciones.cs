using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class SolicitudInterconsultaViewModel
    {
        private void setValidaciones()
        {
            ClearRules();
            AddRule(()=>IsEspecialidadesValid,()=>IsEspecialidadesValid || IsServAuxSeleccionadosValid,"Agregar por lo menos una especialidad/servicio auxiliar al plan de tratamiento");
            RaisePropertyChanged("IsEspecialidadesValid");
        }


    }
}
