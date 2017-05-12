using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ReporteTrasladosEstatalesViewModel
    {
        private void setValidaciones()
        {
            ClearRules();
            AddRule(() => FechaInicio, () => FechaInicio.HasValue, "SELECCIONAR UNA FECHA DE INICIO");
            AddRule(()=> FechaFin,()=>!FechaFin.HasValue || (FechaFin.HasValue && FechaFin.Value>=FechaInicio.Value),"SELECCIONAR UNA FECHA FINAL VALIDA");
            AddRule(() => SelectedTipoTrasladoValue, () => SelectedTipoTrasladoValue != "-1", "SELECCIONA UN TIPO DE TRASLADO");
            AddRule(() => IsTipoMovimientoValid, () => IsTipoMovimientoValid,"SELECCIONA EGRESO O INGRESO");
        }
    }
}
