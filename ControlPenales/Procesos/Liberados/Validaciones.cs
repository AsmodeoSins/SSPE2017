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
    partial class RegistroLiberadoViewModel 
    {
        private void ValidarEmpleado() 
        {
            try 
            {
                if (!validar)
                    return;
                base.ClearRules();
                base.AddRule(() => EPaterno, () => !string.IsNullOrEmpty(EPaterno), "Apellido paterno es requerido!");
                base.AddRule(() => EMaterno, () => !string.IsNullOrEmpty(EMaterno), "Apellido materno es requerido!");
                base.AddRule(() => ENombre, () => !string.IsNullOrEmpty(ENombre), "Nombre es requerido!");
                //base.AddRule(() => ETipoVisitante, () => ETipoVisitante != -1, "Tipo visitante es requerido!");
                base.AddRule(() => ESexo, () => !string.IsNullOrEmpty(ESexo), "Sexo es requerido!");
                //Fecha Nacimiento
                base.AddRule(() => EFechaNacimientoValid, () => EFechaNacimientoValid , " ");
                
                //base.AddRule(() => ESituacion, () => ESituacion != -1, "Situacion es requerido!");
                base.AddRule(() => ECURP, () => !string.IsNullOrEmpty(ECURP), "CURP es requerido!");
                base.AddRule(() => ERFC, () => !string.IsNullOrEmpty(ERFC), "RFC es requerido!");
                base.AddRule(() => ETelefonoFijo, () => ETelefonoFijo != null ? ETelefonoFijo.Length >= 14 : false, "Teléfono es requerido y debe contener 10 dígitos!");
                base.AddRule(() => ETelefonoMovil, () => ETelefonoMovil != null ? ETelefonoMovil.Length >= 14 : false, "Teléfono móvil es requerido y debe contener 10 dígitos!");
                base.AddRule(() => ECorreo, () => !string.IsNullOrEmpty(ECorreo), "Correo electrónico es requerido!");

                base.AddRule(() => EPais, () => EPais != -1, "País es requerido!");
                if (EPais == 82)//Mexico
                {
                    base.AddRule(() => EEstado, () => EEstado != -1, "Estado es requerido!");
                    base.AddRule(() => EMunicipio, () => EMunicipio != -1, "Municipio es requerido!");
                    if (EEstado == 2)//Baja California
                    {
                        base.AddRule(() => EColonia, () => EColonia != -1, "Colonia es requerida!");
                    }
                }
                base.AddRule(() => ECalle, () => !string.IsNullOrEmpty(ECalle), "Calle es requerida!");
                base.AddRule(() => ENoExterior, () => ENoExterior != null, "No.exterior es requerido!");
                base.AddRule(() => ECP, () => ECP != null, "Código postal es requerido!");

                base.AddRule(() => EDiscapacidad, () => !string.IsNullOrEmpty(EDiscapacidad), "Discapacidad es requerido!");
                if (EDiscapacidad == "S")
                {
                    base.AddRule(() => ETipoDiscapacidad, () => ETipoDiscapacidad != -1, "Tipo de discapacidad es requerido!");
                }
                //Observación
               //base.AddRule(() => ETipoEmpleado, () => ETipoEmpleado != -1, "Tipo de empleado es requerido!");
               //base.AddRule(() => EAreaTrabajo, () => EAreaTrabajo != -1, "Área de trabajo de empleado es requerida!");

                OnPropertyValidateChanged("EPaterno");
                OnPropertyValidateChanged("EMaterno");
                OnPropertyValidateChanged("ENombre");
                //OnPropertyValidateChanged("ETipoVisitante");
                OnPropertyValidateChanged("ESexo");
                OnPropertyValidateChanged("ESituacion");
                OnPropertyValidateChanged("ECURP");
                OnPropertyValidateChanged("ERFC");
                OnPropertyValidateChanged("ETelefonoFijo");
                OnPropertyValidateChanged("ETelefonoMovil");
                OnPropertyValidateChanged("ECorreo");
                OnPropertyValidateChanged("EPais");
                OnPropertyValidateChanged("EEstado");
                OnPropertyValidateChanged("EMunicipio");
                OnPropertyValidateChanged("EColonia");
                OnPropertyValidateChanged("ECalle");
                OnPropertyValidateChanged("ENoExterior");
                OnPropertyValidateChanged("ECP");
                OnPropertyValidateChanged("EDiscapacidad");
                OnPropertyValidateChanged("ETipoDiscapacidad");
                //OnPropertyValidateChanged("ETipoEmpleado");
                //OnPropertyValidateChanged("EAreaTrabajo");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar empleado.", ex);
            }
        }
    }
}
