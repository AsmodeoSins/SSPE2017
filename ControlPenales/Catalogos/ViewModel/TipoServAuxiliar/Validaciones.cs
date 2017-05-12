using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoTipoServiciosAuxViewModel
    {
        void setValidaciones()
        {
            base.ClearRules();
            AddRule(() => TextServAux, () => !string.IsNullOrWhiteSpace(TextServAux), "CAPTURE UN TIPO DE SERVICIO AUXILIAR DE DIAGNOSTICO");
            AddRule(() => SelectedEstatus, () => SelectedEstatus != null && SelectedEstatus.CLAVE != "-1", "SELECCIONE UN ESTATUS!");
            RaisePropertyChanged("TextServAux");
            RaisePropertyChanged("SelectedEstatus");
        }
    }
}
