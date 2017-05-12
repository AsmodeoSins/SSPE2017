using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoSubTipoServiciosAuxViewModel
    {
        void setValidaciones()
        {
            base.ClearRules();
            AddRule(() => SelectedTipoServAux,()=>SelectedTipoServAux!=-1,"SELECCIONE UN TIPO DE SERVICIO AUXILIAR DE DIAGNOSTICO");
            AddRule(() => TextSubTipoServAux, () => !string.IsNullOrWhiteSpace(TextSubTipoServAux), "CAPTURE UN SUBTIPO DE SERVICIO AUXILIAR DE DIAGNOSTICO");
            AddRule(() => SelectedEstatus, () => SelectedEstatus != null && SelectedEstatus.CLAVE != "-1", "SELECCIONE UN ESTATUS!");
            RaisePropertyChanged("TextSubTipoServAux");
            RaisePropertyChanged("SelectedEstatus");
            RaisePropertyChanged("SelectedTipoServAux");
        }
    }
}
