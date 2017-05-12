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
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class RegistroPersonalViewModel 
    {
        private void ValidarEmpleado() 
        {
            try 
            {
                //if (!validar)
                //    return;
                base.ClearRules();
                base.AddRule(() => EPaterno, () => !string.IsNullOrEmpty(EPaterno), "APELLIDO PATERNO ES REQUERIDO!");
                base.AddRule(() => EMaterno, () => !string.IsNullOrEmpty(EMaterno), "APELLIDO MATERNO ES REQUERIDO!");
                base.AddRule(() => ENombre, () => !string.IsNullOrEmpty(ENombre), "NOMBRE ES REQUERIDO!");
                //base.AddRule(() => ETipoVisitante, () => ETipoVisitante != -1, "TIPO VISITANTE ES REQUERIDO!");
                base.AddRule(() => ESexo, () => !string.IsNullOrEmpty(ESexo), "SEXO ES REQUERIDO!");
                //Fecha Nacimiento
                base.AddRule(() => EFechaNacimientoValid, () => EFechaNacimientoValid , " ");
                
                //base.AddRule(() => ESituacion, () => ESituacion != -1, "Situacion es requerido!");
                base.AddRule(() => ECURP, () => !string.IsNullOrEmpty(ECURP), "CURP ES REQUERIDO!");
                base.AddRule(() => ERFC, () => !string.IsNullOrEmpty(ERFC), "RFC ES REQUERIDO!");
                base.AddRule(() => ETelefonoFijo, () => ETelefonoFijo != null ? ETelefonoFijo.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
                base.AddRule(() => ETelefonoMovil, () => ETelefonoMovil != null ? ETelefonoMovil.Length >= 14 : false, "TELÉFONO MÓVIL ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
                base.AddRule(() => ECorreo, () => !string.IsNullOrEmpty(ECorreo) && new Correo().ValidarCorreo(ECorreo), "CORREO ELECTRÓNICO ES REQUERIDO!");

                base.AddRule(() => EPais, () => EPais != -1, "PAÍS ES REQUERIDO!");
                if (EPais == 82)//Mexico
                {
                    base.AddRule(() => EEstado, () => EEstado != -1, "ESTADO ES REQUERIDO!");
                    base.AddRule(() => EMunicipio, () => EMunicipio != -1, "MUNICIPIO ES REQUERIDO!");
                    if (EEstado == 2)//Baja California
                    {
                        base.AddRule(() => EColonia, () => EColonia != -1, "COLONIA ES REQUERIDA!");
                    }
                }
                base.AddRule(() => ECalle, () => !string.IsNullOrEmpty(ECalle), "CALLE ES REQUERIDA!");
                base.AddRule(() => ENoExterior, () => ENoExterior != null, "NO.EXTERIOR ES REQUERIDO!");
                base.AddRule(() => ECP, () => ECP != null, "CÓDIGO POSTAL ES REQUERIDO!");

                base.AddRule(() => EDiscapacidad, () => !string.IsNullOrEmpty(EDiscapacidad), "DISCAPACIDAD ES REQUERIDO!");
                if (EDiscapacidad == "S")
                {
                    base.AddRule(() => ETipoDiscapacidad, () => ETipoDiscapacidad != -1, "TIPO DE DISCAPACIDAD ES REQUERIDO!");
                }
                //Observación
                base.AddRule(() => ETipoEmpleado, () => ETipoEmpleado != -1, "TIPO DE EMPLEADO ES REQUERIDO!");
                base.AddRule(() => EAreaTrabajo, () => EAreaTrabajo != -1, "ÁREA DE TRABAJO DE EMPLEADO ES REQUERIDA!");
                base.AddRule(()=> SelectedCentroValue, ()=>SelectedCentroValue!=-1,"CENTRO ASIGNADO ES REQUERIDO!");

                RaisePropertyChanged("EPaterno");
                RaisePropertyChanged("EMaterno");
                RaisePropertyChanged("ENombre");
                RaisePropertyChanged("ETipoVisitante");
                RaisePropertyChanged("ESexo");
                RaisePropertyChanged("ESituacion");
                RaisePropertyChanged("ECURP");
                RaisePropertyChanged("ERFC");
                RaisePropertyChanged("ETelefonoFijo");
                RaisePropertyChanged("ETelefonoMovil");
                RaisePropertyChanged("ECorreo");
                RaisePropertyChanged("EPais");
                RaisePropertyChanged("EEstado");
                RaisePropertyChanged("EMunicipio");
                RaisePropertyChanged("EColonia");
                RaisePropertyChanged("ECalle");
                RaisePropertyChanged("ENoExterior");
                RaisePropertyChanged("ECP");
                RaisePropertyChanged("EDiscapacidad");
                RaisePropertyChanged("ETipoDiscapacidad");
                RaisePropertyChanged("ETipoEmpleado");
                RaisePropertyChanged("EAreaTrabajo");
                RaisePropertyChanged("SelectedCentroValue");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar empleado.", ex);
            }
        }
    }
}
