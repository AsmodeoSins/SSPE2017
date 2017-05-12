using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoServiciosAuxiliaresViewModel
    {
        void setValidaciones()
        {
            base.ClearRules();
            AddRule(()=>SelectedSubtipoServAux, ()=>SelectedSubtipoServAux!=-1,"SELECCIONE UN SUBTIPO DE SERVICIO AUXILIAR DE DIAGNOSTICO");
            AddRule(() => SelectedTipoServAux, () => SelectedTipoServAux != -1, "SELECCIONE UN TIPO DE SERVICIO AUXILIAR DE DIAGNOSTICO");
            AddRule(() => TextServAux, () => !string.IsNullOrWhiteSpace(TextServAux), "CAPTURE UN SERVICIO AUXILIAR DE DIAGNOSTICO");
            AddRule(() => SelectedEstatus, () => SelectedEstatus != null && SelectedEstatus.CLAVE != "-1", "SELECCIONE UN ESTATUS!");
            RaisePropertyChanged("TextServAux");
            RaisePropertyChanged("SelectedEstatus");
            RaisePropertyChanged("SelectedTipoServAux");
            RaisePropertyChanged("SelectedSubtipoServAux");
        }
    }
}
