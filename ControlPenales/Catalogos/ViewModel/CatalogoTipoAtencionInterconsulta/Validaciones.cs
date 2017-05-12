using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoTipoAtencionInterconsultaViewModel
    {
        private void setValidaciones()
        {
            ClearRules();
            AddRule(() => TextTipoAtencion, () => !string.IsNullOrWhiteSpace(TextTipoAtencion), "CAPTURA UN TIPO DE ATENCION DE INTERCONSULTA");
            AddRule(() => SelectedEstatus, () => SelectedEstatus != null && SelectedEstatus.CLAVE != "-1", "SELECCIONE UN ESTATUS!");
            RaisePropertyChanged("TextTipoAtencion");
            RaisePropertyChanged("SelectedEstatus");
        }
    }
}
