using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogosAgenciasViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => Domicilio, () => !string.IsNullOrEmpty(Domicilio), "DOMICILIO ES REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
            base.AddRule(() => SelectAgenciaTipo, () => SelectAgenciaTipo != null, "TIPO AGENCIA ES REQUERIDO!");
            base.AddRule(() => Entidad, () => Entidad != null, "ENTIDAD ES REQUERIDO!");
            base.AddRule(() => SelectMunicipio, () => SelectMunicipio != null, "MUNICIPIO  ES REQUERIDO!");
        }
    }
}
