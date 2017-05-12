using System;
using System.ComponentModel;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        void setValidacionesDocumentos()
        {
            base.ClearRules();
            base.AddRule(() => FechaD, () => FechaCeresoIngreso != null, "FECHA DE TRASLADO ES REQUERIDA!");
            base.AddRule(() => ActividadD, () => !string.IsNullOrEmpty(ActividadD), "ACTIVIDAD ES REQUERIDA!");
            base.AddRule(() => SelectedTipoDocumento, () => SelectedTipoDocumento != null, "TIPO DE DOCUMENTO ES REQUERIDO!");
        }
    }
}