using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoComportamientoHomoViewModel 
    {
        private void LimpiarTipoVisita()
        {
            Descripcion = string.Empty;
            SelectedEstatus = null;
        }
        private void PopularEstatusCombo()
        {
            SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
        }
    }
}
