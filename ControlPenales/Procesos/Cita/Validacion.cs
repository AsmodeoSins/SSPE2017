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
    partial class CitaViewModel : ValidationViewModelBase
    {
        #region Validacones
        private void ValidacionSolicitud() {
            base.ClearRules();
            base.AddRule(() => AFechaValid, () => AFechaValid , "Fecha es requerida");
            base.AddRule(() => AHoraI, () => AHoraI.HasValue, "Hora inicio es requerida");
            base.AddRule(() => AHoraF, () => AHoraF.HasValue, "Hora fin requerida");
            base.AddRule(() => AHorasValid, () => AHorasValid, "La hora de inicio debe ser mayor a la hora fin");

            OnPropertyChanged("AFechaValid");
            OnPropertyChanged("AHoraI");
            OnPropertyChanged("AHoraF");
            OnPropertyChanged("AHorasValid");
        }

        #endregion
    }
}
