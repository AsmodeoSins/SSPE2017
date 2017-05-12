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
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class SolicitudAtencionViewModel : ValidationViewModelBase
    {
        #region Validacones
        private void ValidacionSolicitud() {
            base.ClearRules();
            base.AddRule(() => SAreaTecnica, () => SAreaTecnica != -1, "ÁREA TÉCNICA ES REQUERIDA!");
            base.AddRule(() => SArea, () => SArea != -1, "ÁREA ES REQUERIDA!");
            base.AddRule(() => SActividad, () => !string.IsNullOrEmpty(SActividad), "ACTIVIDAD ES REQUERIDA!");
            //base.AddRule(() => SAutorizacion, () => !string.IsNullOrEmpty(SAutorizacion), "AUTORIZACIÓN ES REQUERIDA!");
            //base.AddRule(() => SOficialTraslada, () => !string.IsNullOrEmpty(SOficialTraslada), "OFICIAL QUE TRASLADA ES REQUERIDO!");
            //base.AddRule(() => SFecValid, () => SFecValid, "FECHA ES REQUERIDA!");
            
            OnPropertyChanged("SAreaTecnica");
            OnPropertyChanged("SArea");
            OnPropertyChanged("SActividad");
            //OnPropertyChanged("SAutorizacion");
            //OnPropertyChanged("SOficialTraslada");
            //OnPropertyChanged("SFecValid");
        }

        private bool ValidacionIngresos() {
            if (LstInternos != null)
            {
                if (LstInternos.Count > 0)
                    return true;
            }
            return false;
        }

        private void ValidacionTipoSolicitud()
        {
            base.AddRule(()=>SelectedAtencion_Tipo,()=>SelectedAtencion_Tipo!=-1,"TIPO DE ATENCIÓN ES REQUERIDO!");
            OnPropertyChanged("SelectedAtencion_Tipo");
        }

        private void QuitarValidacionTipoSolicitud()
        {
            base.RemoveRule("SelectedAtencion_Tipo");
            OnPropertyChanged("SelectedAtencion_Tipo");
        }
        #endregion
    }
}
