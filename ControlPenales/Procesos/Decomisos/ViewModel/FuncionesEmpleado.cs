using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales.Clases;
using System.Windows.Forms;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {
        #region Empleado
        private void LimpiarEmpleado()
        {
            NipE = null;
            NombreE = PaternoE = MaternoE = string.Empty;
            LstEmpleadoPop = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
            EmpleadoEmpty = false;
            ImagenEmpleadoPop = new Imagenes().getImagenPerson();
        }
        
        private void AgregarEmpleado() 
        {
            if (SelectedEmpleadoPop != null)
            {
                if (LstEmpleado == null)
                    LstEmpleado = new ObservableCollection<DECOMISO_PERSONA>();
                LstEmpleado.Add(new DECOMISO_PERSONA() {
                    ID_PERSONA = SelectedEmpleadoPop.ID_PERSONA,
                    ID_TIPO_PERSONA = 1, //EMPLEADO
                    PERSONA = SelectedEmpleadoPop
                });
            }
        }

        private async void EliminarEmpleado() {
            if (SelectedEmpleado != null)
            {
                if (await new Dialogos().ConfirmarEliminar("Confirmacion!", "¿Desea eliminar el empleado seleccionado?") == 1)
                { 
                    LstEmpleado.Remove(SelectedEmpleado);
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Notificacion!", "Favor de seleccionar un empleado");
        }
        #endregion
    }
}
