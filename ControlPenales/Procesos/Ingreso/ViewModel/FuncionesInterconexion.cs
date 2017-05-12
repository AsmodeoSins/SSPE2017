using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Office.Interop.Word;
using System.IO;

//using MvvmFramework;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        private void PopulateDatosGeneralesInterconexion() {
            if (SelectedInterconexion != null)
            {
                //ApellidoPaternoBuscar = SelectedInterconexion.PRIMERAPELLIDO;
                //ApellidoMaternoBuscar = SelectedInterconexion.SEGUNDOAPELLIDO;
                //NombreBuscar = SelectedInterconexion.NOMBRE;
                SelectSexo = !string.IsNullOrEmpty(SelectedInterconexion.SEXO) ? SelectedInterconexion.SEXO : "M";
                SelectEstadoCivil = SelectedInterconexion.ESTADOCIVILID != null ? SelectedInterconexion.ESTADOCIVILID : -1;
                if (SelectedInterconexion.TELEFONO != null)
                    if (string.IsNullOrEmpty(SelectedInterconexion.TELEFONO))
                        TextTelefono = SelectedInterconexion.TELEFONO;
                SelectNacionalidad = SelectedInterconexion.NACIONALIDADID != null ? SelectedInterconexion.NACIONALIDADID : Parametro.PAIS; //82;
                //DATOS DE NACIMIENTO
                SelectPaisNacimiento = SelectedInterconexion.NACIONALIDADID;
                SelectEntidadNacimiento = SelectedInterconexion.ESTADOORIGENID;
                SelectMunicipioNacimiento = SelectedInterconexion.MUNICIPIOORIGENID;
                TextFechaNacimiento = SelectedInterconexion.FECHANACIMIENTO;
            }
        }

        private async void GuardarNUC() { 
            if(Ingreso != null && SelectedInterconexion != null)
            {
                var nuc = new NUC();
                nuc.ID_CENTRO = Ingreso.ID_CENTRO;
                nuc.ID_ANIO = Ingreso.ID_ANIO;
                nuc.ID_IMPUTADO = Ingreso.ID_IMPUTADO;
                nuc.ID_INGRESO = Ingreso.ID_INGRESO;
                nuc.ID_NUC = SelectedInterconexion.EXPEDIENTEID.ToString();
                nuc.DESCR = string.Empty; 
                new cNuc().Insertar(nuc);
            }
        }
        
    }
}
