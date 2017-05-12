using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoMarcas_ModelosViewModel
    {

        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
            base.AddRule(() => SelectedFabricante, () => SelectedFabricante != null, "FABRICANTE ES REQUERIDO!");
        }

    }
}
