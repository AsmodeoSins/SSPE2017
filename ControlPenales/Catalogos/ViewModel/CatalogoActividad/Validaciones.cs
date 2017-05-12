using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoActividadViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => Activo, () => !string.IsNullOrEmpty(Activo), "ACTIVO ES REQUERIDA!");
            base.AddRule(() => Objetivo, () => !string.IsNullOrEmpty(Objetivo), "OBJETIVO ES REQUERIDA!");
            base.AddRule(() => IdTipoP, () => IdTipoP != -1, "TIPO DE PROGRAMA ES REQUERIDA!");
            base.AddRule(() => Prioridad, () => Prioridad.HasValue, "PRIORIDAD ES REQUERIDO!");
            base.AddRule(() => Orden, () => Orden.HasValue, "ORDEN ES REQUERIDO!");
            base.AddRule(() => OcupanteMin, () => OcupanteMin.HasValue, "MINIMO DE OCUPANTES ES REQUERIDO!");
            base.AddRule(() => OcupanteMax, () => OcupanteMax.HasValue, "MAXIMO ES REQUERIDO!");
        }
    }
}