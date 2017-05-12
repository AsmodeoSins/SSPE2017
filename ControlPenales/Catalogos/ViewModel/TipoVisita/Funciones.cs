using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
namespace ControlPenales
{
    partial class CatalogoTipoVisitaViewModel
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
